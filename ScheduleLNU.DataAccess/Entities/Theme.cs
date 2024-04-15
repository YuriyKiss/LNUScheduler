using System.ComponentModel.DataAnnotations;

namespace ScheduleLNU.DataAccess.Entities
{
    public class Theme : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string ForeColor { get; set; }

        public string BackColor { get; set; }

        public string Font { get; set; }

        public string FontSize { get; set; }
    }
}
