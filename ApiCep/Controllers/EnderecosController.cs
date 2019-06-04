using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApiCep.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.IO;
using System;
using Newtonsoft.Json;

namespace ApiCep.Controllers
{
    [Route("api/cep")]
    [ApiController]
    public class EnderecosController : ControllerBase
    {
        private readonly ApiCepContext _context;

        public EnderecosController(ApiCepContext context)
        {
            _context = context;
        }

        // GET: api/cep/98700000
        [HttpGet("{cep}")]
        public async Task<ActionResult<Endereco>> GetEndereco(string cep)
        {
            var endereco = await _context.Endereco.SingleOrDefaultAsync(c => c.Cep.Replace("-", "").Replace(".", "") == cep.Replace("-", "").Replace(".", ""));

            if (endereco == null || DateTime.Now > endereco.ValidadeConsulta)
            {
                var response = await ConsultaViaCep(cep);

                if(response == null || response.Contains("Erro:"))
                {
                    if (response.Contains("Erro:"))
                        return StatusCode(400, new Erro { Code = 400, Message = response });
                    if (endereco == null)
                        return StatusCode(400, new Erro { Code = 400, Message = "Ocorreu um erro ao buscar o CEP informado. Tente novamente mais tarde." });
                    return endereco;
                }

                if (endereco == null)
                    endereco = new Endereco();
                
                Endereco viaCepEndereco = JsonConvert.DeserializeObject<Endereco>(response);
                endereco.ValidadeConsulta = DateTime.Now.AddDays(10);
                endereco.Cep = viaCepEndereco.Cep;
                endereco.Logradouro = viaCepEndereco.Logradouro;
                endereco.Complemento = viaCepEndereco.Complemento;
                endereco.Bairro = viaCepEndereco.Bairro;
                endereco.Localidade = viaCepEndereco.Localidade;
                endereco.Uf = viaCepEndereco.Uf;
                endereco.Unidade = viaCepEndereco.Unidade;
                endereco.Ibge = viaCepEndereco.Ibge;
                endereco.Gia = viaCepEndereco.Gia;

                if (endereco.Id.Equals(0))
                    await _context.Endereco.AddAsync(endereco);
                await _context.SaveChangesAsync();
            }

            return endereco;
        }

        private async Task<string> ConsultaViaCep(string cep)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://viacep.com.br/ws/" + cep + "/json/");
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "GET";
            request.KeepAlive = true;
            request.AllowAutoRedirect = false;
            request.ContentType = "application/json; charset=utf-8";
            request.Timeout = 2500;

            try
            {
                WebResponse response = await request.GetResponseAsync();
                StreamReader sjson = new StreamReader(response.GetResponseStream());
                return sjson.ReadToEnd();
            }
            catch (WebException e)
            {
                if (e.Status.Equals(WebExceptionStatus.ProtocolError))
                    return "Erro: " + e.Message + " Verifique o CEP digitado e tente novamente.";
                return null;
            }
        }
    }
}
