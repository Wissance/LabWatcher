using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wissance.MossbauerLab.Watcher.Data.Entities;

namespace Wissance.MossbauerLab.Watcher.Data.Mapping
{
    internal static class SpectrumMapping
    {
        public static void Map(this EntityTypeBuilder<SpectrumEntity> builder)
        {
            builder.ToTable("Wissance.MossbauerLab.Spectrum");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired();
            builder.HasIndex(p => p.Name);
            builder.Property(p => p.IsArchived).IsRequired().HasDefaultValue(false);
        }
    }
}
