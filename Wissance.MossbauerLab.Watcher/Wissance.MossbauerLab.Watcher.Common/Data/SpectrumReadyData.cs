using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Common.Data
{
    public class SpectrumReadyData
    {
        public SpectrumReadyData()
        {
        }

        public SpectrumReadyData(string name, int channel, DateTime updated, byte[] spectrum, FileInfo rawInfo)
        {
            Name = name;
            Channel = channel;
            Updated = updated;
            Spectrum = spectrum;
            RawInfo = rawInfo;
        }

        public string Name { get; set; }
        public int Channel { get; set; }
        public DateTime Updated { get; set; }
        public byte[] Spectrum { get; set; }
        public FileInfo RawInfo { get; set; }
    }
}
