namespace AgendaLeaf.Models.ViewModels
{
    public class EventViewModel
    {
        public Event Event { get; set; }
        public ICollection<Guid> UsersId { get; set; }
    }
}
