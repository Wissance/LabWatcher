using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wissance.MossbauerLab.Watcher.Data.Entities;
using Wissance.MossbauerLab.Watcher.Data.Mapping;

namespace Wissance.MossbauerLab.Watcher.Data
{
    public class ModelContext : DbContext, IModelContext
    {
        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }

        public int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SpectrumEntity>().Map();
        }
        
        public DbSet<SpectrumEntity> Spectra { get; }
    }
}
