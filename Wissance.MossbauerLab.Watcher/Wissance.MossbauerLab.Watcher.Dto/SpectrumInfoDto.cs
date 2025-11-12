using System;
using System.Collections.Generic;
using System.Text;

namespace Wissance.MossbauerLab.Watcher.Dto
{
    public class SpectrumInfoDto
    {
        public SpectrumInfoDto()
        {
        }

        public SpectrumInfoDto(int id, string name, string description, DateTime measureStartDate, DateTime? first, DateTime? last, bool isArchived)
        {
            Id = id;
            Name = name;
            Description = description;
            MeasureStartDate = measureStartDate;
            First = first;
            Last = last;
            IsArchived = isArchived;
        }

        public int Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime MeasureStartDate { get; set; }
        public DateTime? First { get; set; }
        public DateTime? Last { get; set; }
        public bool IsArchived { get; set; }
    }
}
