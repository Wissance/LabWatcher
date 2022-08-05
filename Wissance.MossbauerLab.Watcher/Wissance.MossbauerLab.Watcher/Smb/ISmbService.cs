using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Smb
{
    public interface ISmbService
    {
        Task<IList<string>> GetChildrenAsync(string parent);
        Task<byte[]> ReadAsync();
    }
}
