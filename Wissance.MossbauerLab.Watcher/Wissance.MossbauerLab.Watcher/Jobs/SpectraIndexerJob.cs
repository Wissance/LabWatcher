using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Wissance.MossbauerLab.Watcher.Web.Smb;

namespace Wissance.MossbauerLab.Watcher.Web.Jobs
{
    public class SpectraIndexerJob : IJob
    {
        public SpectraIndexerJob(ISmbService smbService/*, string spectraShare*/)
        {
            _smbService = smbService;
            _spectraShare = "Autosaves";
            //spectraShare;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            IList<string> children = await _smbService.GetChildrenAsync(_spectraShare, ".");
            if (children != null && children.Any())
            {
                // todoL umv: save to database 
            }
        }

        private readonly ISmbService _smbService;
        private readonly string _spectraShare;
    }
}
