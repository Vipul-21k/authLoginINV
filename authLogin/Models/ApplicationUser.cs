using System.ComponentModel.DataAnnotations;

namespace authLogin.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public bool Is2FAEnabled { get; set; } = false;

        public string? OTP { get; set; }

        public DateTime? OTPExpiry { get; set; }

        public string? TwoFactorSecretKey { get; set; }

      
        public bool Is2FASetupCompleted { get; set; } = false;
    }
}