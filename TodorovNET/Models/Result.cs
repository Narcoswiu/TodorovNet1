namespace TodorovNet.Models
{
    public class Result
    {
        public int Id { get; set; }
        public int ParticipantId { get; set; }
        public Participant? Participant { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public TimeSpan LapTime { get; set; } = TimeSpan.Zero;
        public bool IsFinal { get; set; }
    }
}
