using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiCep.Models
{
    public class ApiCepContext : DbContext
    {
        public ApiCepContext (DbContextOptions<ApiCepContext> options)
            : base(options)
        {
        }

        public DbSet<ApiCep.Models.Endereco> Endereco { get; set; }
    }
}
