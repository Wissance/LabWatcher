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

        public ApplicationConfig(JobsConfig defaultJobsSettings, SmbConfig spectraSmbSetting)
        {
            DefaultJobsSettings = defaultJobsSettings;
            SpectraSmbSettings = spectraSmbSetting;
        }

        public JobsConfig DefaultJobsSettings { get; set; }
        public SmbConfig SpectraSmbSettings { get; set; }
    }
}
