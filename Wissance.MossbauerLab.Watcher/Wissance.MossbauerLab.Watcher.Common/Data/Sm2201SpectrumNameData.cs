using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Common.Data
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
}
