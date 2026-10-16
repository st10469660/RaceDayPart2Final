using System.ComponentModel.DataAnnotations;
using static RaceDayPart2Final.Controllers.AuthController;

namespace RaceDayPart2Final.models
{
    public class AuthModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public UserRole Role
        {
            get; set;
       }
    }
}
