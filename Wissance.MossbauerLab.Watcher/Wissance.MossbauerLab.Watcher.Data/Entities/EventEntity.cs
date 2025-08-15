using System;
using System.Collections.Generic;
using System.Text;
using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.MossbauerLab.Watcher.Data.Entities
{
    public enum EventType
    {
        NetworkLoss = 1,
        PowerLoss = 2,
        Maintenance = 3
    }

    public class EventEntity : IModelIdentifiable<int>
    {
        public EventEntity()
        {
        }

        public EventEntity(EventType eventType, DateTime start, DateTime? finish)
        {
            Type = eventType;
            Start = start;
            Finish = finish;
        }

        public int Id { get; set; }
        public EventType Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime? Finish { get; set; }
    }
}
