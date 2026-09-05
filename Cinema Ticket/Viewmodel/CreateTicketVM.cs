using Cinema_Ticket.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema_Ticket.Viewmodel
{
    public class CreateTicketVM
    {
        public int MovieId { get; set; }

        public Movie Movie { get; set; }

        public int HallId { get; set; }

        public int ShowId { get; set; }

        public IEnumerable<SelectListItem> Halls { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Shows { get; set; }
            = new List<SelectListItem>();
    }
}

