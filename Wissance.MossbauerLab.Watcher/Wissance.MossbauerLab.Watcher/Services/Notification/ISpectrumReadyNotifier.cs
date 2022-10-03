using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wissance.MossbauerLab.Watcher.Web.Data;

namespace Wissance.MossbauerLab.Watcher.Web.Services.Notification
{
    public interface ISpectrumReadyNotifier
    {
        Task<bool> NotifySpectrumSavedAsync(IList<SpectrumReadyData> spectra);
    }
}
