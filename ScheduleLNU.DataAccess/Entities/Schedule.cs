using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleLNU.DataAccess.Entities
{
    public class Schedule : BaseEntity
    {
        public string Title { get; set; }

        [ForeignKey("Student")]
        public string StudentId { get; set; }

        public Student Student { get; set; }

        public List<Event> Events { get; set; }
    }
}
