using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class ApplicationConfig
    {
        public ApplicationConfig()
        {

        }

        public ApplicationConfig(JobsConfig defaultJobsSettings, SmbConfig sm2201SmbSettings)
        {
            DefaultJobsSettings = defaultJobsSettings;
            Sm2201SmbSettings = sm2201SmbSettings;
        }

        public JobsConfig DefaultJobsSettings { get; set; }
        public SmbConfig Sm2201SmbSettings { get; set; }
    }
}
