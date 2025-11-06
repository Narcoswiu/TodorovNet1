namespace TodorovNet.Models.Dtos
{
    public class StandingRow
    {
        public int ParticipantId { get; set; }
        public int Number { get; set; }
        public string Name { get; set; } = "";
        public string Team { get; set; } = "";
        public string LapTime { get; set; } = "00:00:00";
        public int PenaltySeconds { get; set; }
        public string AdjustedLapTime { get; set; } = "00:00:00";
        public bool IsFinal { get; set; }
    }
}
