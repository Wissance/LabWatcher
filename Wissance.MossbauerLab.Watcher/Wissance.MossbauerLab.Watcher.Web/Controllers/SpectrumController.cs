using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wissance.MossabuerLab.Watcher.Dto;
using Wissance.MossbauerLab.Watcher.Data.Entities;
using Wissance.MossbauerLab.Watcher.Web.Managers;
using Wissance.WebApiToolkit.Controllers;

namespace Wissance.MossbauerLab.Watcher.Web.Controllers
{
    public class SpectrumController : BasicReadController<SpectrumInfoDto, SpectrumEntity, int>
    {
        public SpectrumController(SpectrumManager manager)
        {
            Manager = manager;
            _manager = manager;
        }

        private SpectrumManager _manager;
    }
}
