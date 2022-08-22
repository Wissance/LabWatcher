using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Data
{
    public class SpectrumReadyData
    {
        public SpectrumReadyData()
        {
        }

        public SpectrumReadyData(string name, int channel, DateTime updated, byte[] spectrum)
        {
            Name = name;
            Channel = channel;
            Updated = updated;
            Spectrum = spectrum;
        }

        public string Name { get; set; }
        public int Channel { get; set; }
        public DateTime Updated { get; set; }
        public byte[] Spectrum { get; set; }
    }
}
