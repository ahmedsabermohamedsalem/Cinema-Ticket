using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema_Ticket.Models
{
    public class ApplicationUserOtp
    {
        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }
        public string OTP { get; set; }
        public bool IsValid { get; set; }
        public DateTime ValidTo { get; set; }
        public DateTime CreatedAt { get; set; }

        public ApplicationUserOtp()
        {

        }
        public ApplicationUserOtp(string otp, string userId)
        {
            Id = Guid.NewGuid().ToString();
            ApplicationUserId = userId;
            OTP = otp;
            CreatedAt = DateTime.UtcNow;
            IsValid = true;
            ValidTo = DateTime.UtcNow.AddMinutes(15);
        }

    }
}