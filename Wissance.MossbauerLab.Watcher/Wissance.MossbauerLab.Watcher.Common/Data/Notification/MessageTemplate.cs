using System;
using System.Collections.Generic;
using System.Text;

namespace Wissance.MossbauerLab.Watcher.Common.Data.Notification
{
    public class MessageTemplate
    {
        public MessageTemplate()
        {
        }

        public MessageTemplate(bool hasNegativeCase, string positiveCase, string negativeCase)
        {
        }
        /// <summary>
        ///    Means that NegativeCase does not exists
        /// </summary>
        public bool HasNegativeCase { get; set; }
        /// <summary>
        ///    PositiveCase means that event (SpectrometerEvent) occurred and some condition is TRUE (i.e. SpectrumSaved occurred and spectra are NOT empty)
        /// </summary>
        public string PositiveCase { get; set; }
        /// <summary>
        ///    PositiveCase means that event (SpectrometerEvent) occurred and some condition is FALSE (i.e. SpectrumSaved occurred and spectra are NOT empty)
        /// </summary>
        public string NegativeCase { get; set; }
    }
}
