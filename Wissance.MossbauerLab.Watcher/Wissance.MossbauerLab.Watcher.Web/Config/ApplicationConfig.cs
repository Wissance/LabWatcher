
namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class ApplicationConfig
    {
        public ApplicationConfig()
        {

        }

        public ApplicationConfig(string laboratoryComputer, JobsConfig defaultJobsSettings, SpectraStoreConfig sm2201SpectraStoreSettings, 
            NotificationConfig notificationSettings, FtpArchConfig ftpArchSettings, string connStr)
        {
            LaboratoryComputer = laboratoryComputer;
            DefaultJobsSettings = defaultJobsSettings;
            Sm2201SpectraStoreSettings = sm2201SpectraStoreSettings;
            NotificationSettings = notificationSettings;
            FtpArchSettings = ftpArchSettings;
            ConnStr = connStr;
        }

        public string LaboratoryComputer { get; set; }
        public JobsConfig DefaultJobsSettings { get; set; }
        public SpectraStoreConfig Sm2201SpectraStoreSettings { get; set; }
        public NotificationConfig NotificationSettings { get; set; }
        public string ConnStr { get; set; }
        public FtpArchConfig FtpArchSettings { get; set; }
    }
}
