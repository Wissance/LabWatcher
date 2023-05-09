using System;
using System.Collections.Generic;
using System.Text;

namespace Wissance.MossabuerLab.Watcher.Dto
{
    public class SpectrumSamplesInfoDto
    {
        public SpectrumSamplesInfoDto()
        {
        }

        public SpectrumSamplesInfoDto(int id, string name, string[] samples)
        {
            Id = id;
            Name = name;
            Samples = samples;
        }

        public int Id { get; }
        public string Name { get; set; }
        public string[] Samples { get; set; }
    }
}
