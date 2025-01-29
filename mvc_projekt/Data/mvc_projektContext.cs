using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mvc_projekt.Models;

namespace mvc_projekt.Data
{
    public class mvc_projektContext : DbContext
    {
        public mvc_projektContext (DbContextOptions<mvc_projektContext> options)
            : base(options)
        {
        }

        public DbSet<mvc_projekt.Models.Kategoria> Kategoria { get; set; } = default!;
        public DbSet<mvc_projekt.Models.Status> Status { get; set; } = default!;
        public DbSet<mvc_projekt.Models.Zadanie> Zadanie { get; set; } = default!;
    }
}
