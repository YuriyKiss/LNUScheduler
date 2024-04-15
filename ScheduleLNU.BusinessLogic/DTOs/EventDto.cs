using System;
using System.Collections.Generic;
using ScheduleLNU.DataAccess.Entities;

namespace ScheduleLNU.BusinessLogic.DTOs
{
    public class EventDto
    {
        public int Id { get; set; }

        public int ScheduleId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Place { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int StyleId { get; set; }

        public EventStyle Style { get; set; }

        public List<Link> Links { get; set; }
    }
}
