namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class JobsConfig
    {
        public JobsConfig()
        {

        }

        public JobsConfig(string defaultEventWatchSchedule, string defaultSpectraIndexerSchedule, string defaultSpectraNotifySchedule, string defaultFileArchSchedule)
        {
            DefaultEventWatchSchedule = defaultEventWatchSchedule;
            DefaultSpectraIndexerSchedule = defaultSpectraIndexerSchedule;
            DefaultSpectraNotifySchedule = defaultSpectraNotifySchedule;
            DefaultFileArchSchedule = defaultFileArchSchedule;
        }

        public string DefaultEventWatchSchedule { get; set; }
        public string DefaultSpectraIndexerSchedule { get; set; }
        public string DefaultSpectraNotifySchedule { get; set; }
        public string DefaultFileArchSchedule { get; set; }
    }
}
