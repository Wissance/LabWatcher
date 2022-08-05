using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class JobsConfig
    {
        public JobsConfig()
        {

        }

        public JobsConfig(string defaultEventWatchSchedule, string defaultSpectraIndexerSchedule, string defaultSpectraNotifySchedule)
        {
            DefaultEventWatchSchedule = defaultEventWatchSchedule;
            DefaultSpectraIndexerSchedule = defaultSpectraIndexerSchedule;
            DefaultSpectraNotifySchedule = defaultSpectraNotifySchedule;
        }

        public string DefaultEventWatchSchedule { get; set; }
        public string DefaultSpectraIndexerSchedule { get; set; }
        public string DefaultSpectraNotifySchedule { get; set; }
    }
}
