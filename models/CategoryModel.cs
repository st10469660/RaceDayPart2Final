using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDayPart2Final.models
{
    public class CategoryModel
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
        public int EventID { get; internal set; }
        public int CategoryID { get; internal set; }
        public string? CategoryName { get; internal set; }
        public decimal? Distance { get; internal set; }
        public string? StartTime { get; internal set; }
        public decimal? EntryFee { get; internal set; }
        public int SortOrder { get; internal set; }
        public string? Age { get; internal set; }
    }
}

    