
namespace Cinema_Ticket.Models
{
    public class Hall
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int SeatCount { get; set; }

        // FK
        public int CinemaId { get; set; }

    
        public Cinema? Cinema { get; set; }

  
        public ICollection<Ticket> Tickets { get; set; } 
            = new List<Ticket>();
    }
}
