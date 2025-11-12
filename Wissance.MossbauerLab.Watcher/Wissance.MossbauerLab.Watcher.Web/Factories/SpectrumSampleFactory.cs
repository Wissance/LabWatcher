using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wissance.MossbauerLab.Watcher.Dto;
using Wissance.MossbauerLab.Watcher.Data.Entities;

namespace Wissance.MossbauerLab.Watcher.Web.Factories
{
    internal static class SpectrumSampleFactory
    {
        public static SpectrumSamplesInfoDto Create(SpectrumEntity entity, string[] samples)
        {
            return new SpectrumSamplesInfoDto(entity.Id, entity.Name, samples);
        }
    }
}
