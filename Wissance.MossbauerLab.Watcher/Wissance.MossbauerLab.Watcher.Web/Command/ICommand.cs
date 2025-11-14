using System;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    public interface ICommand
    {
        public Task<Tuple<bool, string>> Execute(string[] parameters);
    }
}