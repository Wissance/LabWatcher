using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wissance.MossabuerLab.Watcher.Dto;
using Wissance.MossbauerLab.Watcher.Data.Entities;

namespace Wissance.MossbauerLab.Watcher.Web.Factories
{
    internal static class SpectrumFactory
    {
        public static SpectrumInfoDto Create(SpectrumEntity entity)
        {
            return new SpectrumInfoDto(entity.Id, entity.Name, entity.Description, entity.MeasureStartDate, entity.First, entity.Last, entity.IsArchived);
        }
    }
}
