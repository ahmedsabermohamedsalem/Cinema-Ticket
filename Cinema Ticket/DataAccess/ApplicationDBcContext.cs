using Cinema_Ticket.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket.DataAccess
{
    public class ApplicationDBcContext : DbContext
    {

        public DbSet<Category> categories { get; set; }
        public DbSet<Cinema> cinemas { get; set; }
        public DbSet<Actor>  actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieActor> movieActors { get; set; }
        public DbSet<MovieImage> MovieImages { get; set; }










        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=.;initial catalog = CinemaTicketDB ;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;");
        }

    }
}
