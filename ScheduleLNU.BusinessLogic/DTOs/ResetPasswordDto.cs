using System.ComponentModel.DataAnnotations;

namespace ScheduleLNU.BusinessLogic.DTOs
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmedPassword { get; set; }
    }
}
