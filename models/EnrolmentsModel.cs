namespace RaceDayPart2Final.models
{
    public class EnrolmentsModel
    {
            public int EnrolmentID { get; set; }
            public int EventID { get; set; }
            public int ParticipantID { get; set; }
            public int? CategoryID { get; set; }
            public string Status { get; set; }
            public bool EntryFeePaid { get; set; }
            public string Notes { get; set; }
    }
}

