using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleLNU.DataAccess.Entities
{
    public class EventStyle : BaseEntity
    {
        public string Title { get; set; }

        public string ForeColor { get; set; }

        public string BackColor { get; set; }

        [ForeignKey("Student")]
        public string StudentId { get; set; }

        public Student Student { get; set; }
    }
}
