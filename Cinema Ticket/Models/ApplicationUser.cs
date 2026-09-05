using Microsoft.AspNetCore.Identity;

namespace Cinema_Ticket.Models
{
    public class ApplicationUser: IdentityUser

    {

        public string Name { get; set; }
        public string address { get; set; }



    }
}
