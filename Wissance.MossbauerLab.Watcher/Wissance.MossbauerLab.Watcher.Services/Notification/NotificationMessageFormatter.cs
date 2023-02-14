using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Wissance.MossbauerLab.Watcher.Common.Data;

namespace Wissance.MossbauerLab.Watcher.Services.Notification
{
    internal static class NotificationMessageFormatter
    {
        private const string AutosavedSpectraPlaceholder = "{savedSpectra}";
        // <!--<li>Спектр {msSpName} по каналу {msChNumber} сохранен {msSaveDate}</li>-->
        private const string SavedSpectrumDescriptionTemplate = "<li>Спектр {0} по каналу {1} сохранен {2}</li>";
        private const string CurrentSatePlaceholder = "{currDate}";
        private const string SavedSpectrumTelegramDescriptionTemplate = "- Спектр {0} по каналу {1} сохранен {2}";

        public static string FormatMailMessage(string template, IList<SpectrumReadyData> spectra)
        {
            string mailMessage = template.Replace(CurrentSatePlaceholder, DateTime.Now.ToString("yyyy-MM-dd:HH-mm-ss"));
            IList<string> lines = spectra.Select(s => string.Format(SavedSpectrumDescriptionTemplate, s.Name, s.Channel, s.RawInfo.LastWriteTime.ToString("yyyy-MM-dd:HH-mm-ss"))).ToList();
            string linesStr = string.Join(Environment.NewLine, lines);
            mailMessage = mailMessage.Replace(AutosavedSpectraPlaceholder, linesStr);

            return mailMessage;
        }
        public static string FormatTelegramMessage(string template, IList<SpectrumReadyData> spectra)
        {
            string mailMessage = template.Replace(CurrentSatePlaceholder, DateTime.Now.ToString("yyyy-MM-dd:HH-mm-ss"));
            IList<string> lines = spectra.Select(s => string.Format(SavedSpectrumTelegramDescriptionTemplate, s.Name, s.Channel, s.RawInfo.LastWriteTime.ToString("yyyy-MM-dd:HH-mm-ss"))).ToList();
            string linesStr = string.Join(Environment.NewLine, lines);
            mailMessage = mailMessage.Replace(AutosavedSpectraPlaceholder, linesStr);

            return mailMessage;
        }
    }
}
