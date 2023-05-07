using System;
using System.Collections.Generic;
using System.Text;
using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.MossbauerLab.Watcher.Data.Entities
{
    public class SpectrumEntity : IModelIdentifiable<int>
    {
        public SpectrumEntity()
        {
        }

        public SpectrumEntity(string name, string description, string location, DateTime measureStartDate, DateTime? first, DateTime? last, bool isArchived)
        {
            Name = name;
            Description = description;
            Location = location;
            MeasureStartDate = measureStartDate;
            First = first;
            Last = last;
            IsArchived = isArchived;
        }

        public int Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime MeasureStartDate { get; set; }
        public DateTime? First { get; set; }
        public DateTime? Last { get; set; }
        public bool IsArchived { get; set; }
    }
}
