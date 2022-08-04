using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wissance.MossbauerLab.Watcher.Data.Entities;

namespace Wissance.MossbauerLab.Watcher.Data.Mapping
{
    internal static class EventMapping
    {
        public static void Map(this EntityTypeBuilder<EventEntity> builder)
        {
            builder.ToTable("Wissance.MossbauerLab.Event");
            builder.HasKey(p => p.Id);
        }
    }
}
