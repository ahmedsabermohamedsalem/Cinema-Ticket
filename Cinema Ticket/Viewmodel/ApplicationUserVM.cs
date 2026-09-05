using System.ComponentModel.DataAnnotations;

namespace Cinema_Ticket.ViewModel
{
    public class ApplicationUserVM
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Adderss { get; set; }
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
    }
}
