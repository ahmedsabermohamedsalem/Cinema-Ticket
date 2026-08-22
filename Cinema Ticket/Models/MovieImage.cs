namespace Cinema_Ticket.Models
{
    public class MovieImage
    {






        public int Id { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;


        public string Img { get; set; }


  

    }
}
