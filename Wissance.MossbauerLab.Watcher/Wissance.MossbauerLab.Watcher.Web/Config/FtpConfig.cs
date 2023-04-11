namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class FtpConfig
    {
        public string Host { get; set; }
        public UserCredentials UserCredentials { get; set; }
        public string ServerFolderPath { get; set; }
        public int ArchiveWhenFileIsOlderThanInDays { get; set; }
        public FtpConfig()
        {
            
        }
    }
    
}