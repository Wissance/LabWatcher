using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.MossabuerLab.Watcher.Dto;
using Wissance.MossbauerLab.Watcher.Data.Entities;
using Wissance.WebApiToolkit.Managers;

namespace Wissance.MossbauerLab.Watcher.Web.Managers
{
    public class SpectrumManager: EfModelManager<SpectrumEntity, SpectrumInfoDto, int>
    {
        public SpectrumManager(DbContext dbContext, Func<SpectrumEntity, IDictionary<string, string>, bool> filterFunc, Func<SpectrumEntity, SpectrumInfoDto> createFunc, ILoggerFactory loggerFactory) 
            : base(dbContext, filterFunc, createFunc, loggerFactory)
        {
        }
    }
}
