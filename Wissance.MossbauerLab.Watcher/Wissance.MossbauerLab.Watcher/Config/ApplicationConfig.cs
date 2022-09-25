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

        public ApplicationConfig(JobsConfig defaultJobsSettings, SpectraStoreConfig sm2201SpectraStoreSettings, NotificationConfig notificationSettings, string connStr)
        {
            DefaultJobsSettings = defaultJobsSettings;
            Sm2201SpectraStoreSettings = sm2201SpectraStoreSettings;
            NotificationSettings = notificationSettings;
            ConnStr = connStr;
        }

        public JobsConfig DefaultJobsSettings { get; set; }
        public SpectraStoreConfig Sm2201SpectraStoreSettings { get; set; }
        public NotificationConfig NotificationSettings { get; set; }
        public string ConnStr { get; set; }
    }
}
