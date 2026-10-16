using System.ComponentModel.DataAnnotations;

namespace RaceDayPart2Final.models
{
    public class UserModel
    {
        public int UserID { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public string UserRole { get; set; }

        public string ProfilePictureUrl { get; set; }
    }
}
