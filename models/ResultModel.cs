namespace RaceDayPart2Final.models
{
    public class ResultModel
    {
            public int ResultID { get; set; }
            public int EnrolmentID { get; set; }
            public TimeOnly FinishTime { get; set; }
            public int FinishPosition { get; set; }
            public TimeOnly GunTime { get; set; }
            public TimeOnly ChipTime { get; set; }
            public int? OverallFinishers { get; set; }
            public int? CategoryPosition { get; set; }
    }
}


