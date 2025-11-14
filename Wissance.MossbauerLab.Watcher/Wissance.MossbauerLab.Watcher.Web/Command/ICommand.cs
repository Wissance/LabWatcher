using System;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    public interface ICommand
    {
        public Task<bool> ExecuteAsync(string[] parameters);
    }
}