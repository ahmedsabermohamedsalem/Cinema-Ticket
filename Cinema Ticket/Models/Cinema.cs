
using System.ComponentModel.DataAnnotations;

namespace Cinema_Ticket.Models
{
    public class Cinema
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Img { get; set; } = string.Empty;

        // Movies
        public ICollection<Movie> Movies { get; set; }
            = new List<Movie>();

        // Halls
        public ICollection<Hall> Halls { get; set; }
            = new List<Hall>();
    }
}

