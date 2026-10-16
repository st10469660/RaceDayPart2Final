using System.ComponentModel.DataAnnotations;

namespace RaceDayPart2Final.models
{
    public class EventModel
    {
        
        public int EventID { get; set; }

        [Required]
        public string EventName { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public decimal Distance { get; set; }

        [Required]
        public string EventType { get; set; } // Run | Walk | Cycle

        public int? MaxParticipants { get; set; }
        public decimal? EntryFee { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
  