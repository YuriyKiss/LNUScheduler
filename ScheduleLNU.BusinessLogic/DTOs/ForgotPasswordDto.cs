using System.ComponentModel.DataAnnotations;

namespace ScheduleLNU.BusinessLogic.DTOs
{
    public class ForgotPasswordDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}
