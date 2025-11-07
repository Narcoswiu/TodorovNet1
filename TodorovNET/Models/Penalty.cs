using System.ComponentModel.DataAnnotations;

namespace TodorovNet.Models
{
    public class Penalty
    {
        public int Id { get; set; }

        [Required] public int EventId { get; set; }
        [Required] public int ParticipantId { get; set; }
        [Required] public PenaltyType Type { get; set; }

        [Range(1, 24 * 3600)]
        public int Seconds { get; set; }

        [MaxLength(256)]
        public string? Reason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Event? Event { get; set; }
        public Participant? Participant { get; set; }
    }
}
