using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Store
{
    public interface IFileStoreService
    {
        Task<IList<string>> GetChildrenAsync(string shareName, string parent);
        Task<byte[]> ReadAsync(string fileName);
    }
}
