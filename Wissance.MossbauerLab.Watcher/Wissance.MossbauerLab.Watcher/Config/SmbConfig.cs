using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class SmbUserCredentials
    {
        public SmbUserCredentials()
        {

        }

        public SmbUserCredentials(string user, string password)
        {
            User = user;
            Password = password;
        }

        public string User { get; set; }
        public string Password { get; set; }
    }

    public class SmbConfig
    {
        public SmbConfig()
        {
        }

        public SmbConfig(string address, string domain, string folder, SmbUserCredentials userCredentials)
        {
            Address = address;
            Domain = domain;
            Folder = folder;
            UserCredentials = userCredentials;
        }

        public string Address { get; set; }
        public string Domain { get; set; }
        public string Folder { get; set; }
        public SmbUserCredentials UserCredentials { get; set; }
    }
}
