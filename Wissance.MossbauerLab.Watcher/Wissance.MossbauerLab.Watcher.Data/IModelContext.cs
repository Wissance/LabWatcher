using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wissance.MossbauerLab.Watcher.Data.Entities;

namespace Wissance.MossbauerLab.Watcher.Data
{
    public interface IModelContext
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        DbSet<SpectrumEntity> Spectra { get; }
        DbSet<EventEntity> Events { get; }
    }
}
