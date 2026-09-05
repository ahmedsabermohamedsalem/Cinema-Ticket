using Cinema_Ticket.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket.DataAccess
{
    public class ApplicationDBcContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBcContext(
            DbContextOptions<ApplicationDBcContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<MovieImage> MovieImages { get; set; }
        public DbSet<ApplicationUserOtp> ApplicationUserOtps { get; set; }



        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(
                "Data Source=.;initial catalog=CinemaTicketDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;");
        }
    }
}