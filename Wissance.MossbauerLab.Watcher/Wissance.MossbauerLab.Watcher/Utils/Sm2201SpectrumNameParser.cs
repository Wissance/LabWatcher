using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Utils
{
    public class Sm2201SpectrumNameData
    {
        public Sm2201SpectrumNameData()
        {
        }

        public Sm2201SpectrumNameData(int channel, string oneLetterSpectrumType, DateTime measureStart)
        {
            Channel = channel;
            OneLetterSpectrumType = oneLetterSpectrumType;
            MeasureStart = measureStart;
        }

        public int Channel { get; set; }
        public string OneLetterSpectrumType { get; set; }
        public DateTime MeasureStart { get; set; }
    }

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
