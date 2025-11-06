using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodorovNet.Models
{
    public class Penalty
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int ParticipantId { get; set; }

        [Required]
        public PenaltyType Type { get; set; }

        // Наказание в секунди (прибавя се към времето)
        [Range(1, 24 * 3600)]
        public int Seconds { get; set; }

        [MaxLength(256)]
        public string? Reason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [MaxLength(64)]
        public string? CreatedBy { get; set; }

        // Навигации
        public Event? Event { get; set; }
        public Participant? Participant { get; set; }
    }
}
