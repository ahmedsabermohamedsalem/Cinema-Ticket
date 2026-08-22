using System.ComponentModel.DataAnnotations;

namespace Cinema_Ticket.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Duration { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string? MainImg { get; set; }

        public int CategoryId { get; set; }

        public int CinemaId { get; set; }

     

        public Category Category { get; set; } = null!;

        public Cinema Cinema { get; set; } = null!;

        public ICollection<MovieActor> MovieActors { get; set; }
            = new List<MovieActor>();

        public ICollection<MovieImage> MovieImages { get; set; }
            = new List<MovieImage>();
    }
}