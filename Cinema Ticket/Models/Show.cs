namespace Cinema_Ticket.Models
{
   
        public class Show
        {
            public int Id { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

            public ICollection<Ticket> Tickets { get; set; }
      = new List<Ticket>();
        }
    }

