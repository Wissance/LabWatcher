using Wissance.MossbauerLab.Watcher.Common.Data.Storage;

namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class FtpArchConfig
    {

        public FtpArchConfig()
        {
            
        }

        public FtpArchConfig(FtpRequisites ftpSettings, string ftpArchRootDir, int transferThreshold)
        {
            FtpSettings = ftpSettings;
            FtpArchRootDir = ftpArchRootDir;
            TransferThreshold = transferThreshold;
        }

        public FtpRequisites FtpSettings { get; set; }
        public string FtpArchRootDir { get; set; }
        public int TransferThreshold { get; set; } 
    }
    
}