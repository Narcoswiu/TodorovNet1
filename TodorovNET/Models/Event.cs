namespace TodorovNet.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public ICollection<Participant> Participants { get; set; } = new List<Participant>();
    }
}
