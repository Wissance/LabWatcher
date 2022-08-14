using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Wissance.MossbauerLab.Watcher.Web.Store;

namespace Wissance.MossbauerLab.Watcher.Web.Jobs
{
    public class SpectraIndexerJob : IJob
    {
        public SpectraIndexerJob(IFileStoreService storeService/*, string spectraShare*/)
        {
            _storeService = storeService;
            _spectraShare = "Autosaves";
            //spectraShare;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            IList<string> children = await _storeService.GetChildrenAsync(_spectraShare, ".");
            if (children != null && children.Any())
            {
                // todoL umv: save to database 
            }
        }

        private readonly IFileStoreService _storeService;
        private readonly string _spectraShare;
    }
}
