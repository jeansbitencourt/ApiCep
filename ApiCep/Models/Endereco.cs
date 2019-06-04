using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCep.Models
{
    [Table("cep")]
    public class Endereco
    {
        [JsonIgnore]
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("cep")]
        public string Cep { get; set; }
        [Column("logradouro")]
        public string Logradouro { get; set; }
        [Column("complemento")]
        public string Complemento { get; set; }
        [Column("bairro")]
        public string Bairro { get; set; }
        [Required]
        [Column("localidade")]
        public string Localidade { get; set; }
        [Required]
        [Column("uf")]
        public string Uf { get; set; }
        [Column("unidade")]
        public string Unidade { get; set; }
        [Column("ibge")]
        public string Ibge { get; set; }
        [Column("gia")]
        public string Gia { get; set; }
        [Required]
        [Column("validade_consulta")]
        public DateTime ValidadeConsulta { get; set; }
    }
}
