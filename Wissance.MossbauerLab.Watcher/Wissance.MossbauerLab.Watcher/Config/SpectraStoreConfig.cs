using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class UserCredentials
    {
        public UserCredentials()
        {

        }

        public UserCredentials(string user, string password)
        {
            User = user;
            Password = password;
        }

        public string User { get; set; }
        public string Password { get; set; }
    }

    public class SpectraStoreConfig
    {
        public SpectraStoreConfig()
        {
        }

        public SpectraStoreConfig(string address, string domain, string folder, UserCredentials userCredentials)
        {
            Address = address;
            Domain = domain;
            Folder = folder;
            UserCredentials = userCredentials;
        }

        public string Address { get; set; }
        public string Domain { get; set; }
        public string Folder { get; set; }
        public UserCredentials UserCredentials { get; set; }
    }
}
