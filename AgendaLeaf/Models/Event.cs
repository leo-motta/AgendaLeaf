namespace AgendaLeaf.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date {  get; set; }
        public string Type { get; set; }
        public Guid OwnerId { get; set; }
        public User Owner { get; set; }
        public ICollection<EventParticipant> Participants { get; set; }

    }
}