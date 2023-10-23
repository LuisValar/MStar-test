using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Teste_MStar.Models;

namespace Teste_MStar.Context
{
    public class MStarContext : DbContext
    {
        public MStarContext(DbContextOptions<MStarContext> options) :base(options)
        { 
        }

        public DbSet<Products> Products { get; set; }
    }
}