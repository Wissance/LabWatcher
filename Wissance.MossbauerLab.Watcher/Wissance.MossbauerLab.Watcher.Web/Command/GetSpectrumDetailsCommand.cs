using System;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    public class GetSpectrumDetailsCommand : ICommand
    {
        public Task<Tuple<bool, string>> Execute(string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}