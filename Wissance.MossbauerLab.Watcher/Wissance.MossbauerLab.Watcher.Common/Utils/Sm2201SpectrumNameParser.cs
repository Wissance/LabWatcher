using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Wissance.MossbauerLab.Watcher.Common.Data;

namespace Wissance.MossbauerLab.Watcher.Common.Utils
{
    public static class Sm2201SpectrumNameParser
    {
        public static Sm2201SpectrumNameData Parse(string fileName)
        {
            int channel = -1;
            string channelSubStr = fileName.Substring(0, 1);
            int.TryParse(channelSubStr, out channel);
            string spectrumTypeSubStr = fileName.Substring(1, 2);
            string dateTimeSubstr = fileName.Substring(2);
            DateTime measureStart = DateTime.ParseExact(dateTimeSubstr, "ddMMyy", CultureInfo.InvariantCulture);
            return new Sm2201SpectrumNameData(channel, spectrumTypeSubStr, measureStart);
        }
    }
}
