namespace Cinema_Ticket.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        public int ShowId { get; set; }
        public Show Show { get; set; } = null!;

        public int HallId { get; set; }
        public Hall Hall { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public string SeatNumber { get; set; } = null!;

        public decimal Price { get; set; }
    }
}