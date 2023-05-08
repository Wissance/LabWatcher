using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.MossabuerLab.Watcher.Dto;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Data.Entities;
using Wissance.MossbauerLab.Watcher.Web.Factories;
using Wissance.WebApiToolkit.Managers;

namespace Wissance.MossbauerLab.Watcher.Web.Managers
{
    public class SpectrumManager: EfModelManager<SpectrumEntity, SpectrumInfoDto, int>
    {
        public SpectrumManager(ModelContext dbContext, ILoggerFactory loggerFactory) 
            : base(dbContext, null, SpectrumFactory.Create, loggerFactory)
        {
        }
    }
}
