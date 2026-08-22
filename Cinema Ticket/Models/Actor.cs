using System.ComponentModel.DataAnnotations;

namespace Cinema_Ticket.Models
{
    public class Actor
    {




        public int ID { get; set; }


        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; }
        public string Img { get; set; }


        public ICollection<Movie> Movies { get; set; }
    = new List<Movie>();







    }
}
