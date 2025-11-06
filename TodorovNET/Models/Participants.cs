namespace TodorovNet.Models
{
    public class Participant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Number { get; set; }
        public string Team { get; set; } = string.Empty;

        public ICollection<Result> Results { get; set; } = new List<Result>();
    }
}
