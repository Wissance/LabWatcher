namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class FtpConfig
    {

        public FtpConfig()
        {
            
        }

        public string Host { get; set; }
        public UserCredentials UserCredentials { get; set; }
        public string ServerFolderPath { get; set; }
        // TODO: UMV: Некорректное использование этого параметра здесь, как с настройками почты необходимо разделение на настройки FTP и настройки вокруг архивирования
        public int ArchiveWhenFileIsOlderThanInDays { get; set; } 
    }
    
}