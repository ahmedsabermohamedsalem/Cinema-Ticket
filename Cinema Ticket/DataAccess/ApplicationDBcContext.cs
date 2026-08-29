using Cinema_Ticket.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket.DataAccess
{
    public class ApplicationDBcContext : DbContext
    {
        public ApplicationDBcContext(
            DbContextOptions<ApplicationDBcContext> options)
            : base(options)
        {
        }

        public DbSet<Category> categories { get; set; }
        public DbSet<Cinema> cinemas { get; set; }
        public DbSet<Actor> actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieActor> movieActors { get; set; }
        public DbSet<MovieImage> MovieImages { get; set; }
    }
}