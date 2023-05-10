namespace AgendaLeaf.Models
{
    public class EventParticipant
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Event Event { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
