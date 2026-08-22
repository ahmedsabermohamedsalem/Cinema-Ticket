using Cinema_Ticket.Models;

namespace Cinema_Ticket.Viewmodel
{
    public class movieModelVM
    {
        public Movie Movie { get; set; }

        public List<int> ActorIds { get; set; } = new List<int>();

        public IFormFile? ImageFile { get; set; }

        public List<IFormFile> SubImages { get; set; }
            = new List<IFormFile>();
    }
}