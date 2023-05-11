using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Wissance.MossbauerLab.Watcher.Common.Data;

namespace Wissance.MossbauerLab.Watcher.Services.Notification
{
    public interface ISpectrumMeasureEventsNotifier
    {
        Task<bool> NotifySpectrumSavedAsync(IList<SpectrumReadyData> spectra);
    }
}
