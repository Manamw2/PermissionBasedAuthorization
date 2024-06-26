using System.ComponentModel.DataAnnotations;

namespace NAID_Users.Dtos.User
{
    public class RegisterDto
    {
        [Required]
        public string? UserName { set; get; }
        [Required]
        [EmailAddress]
        public string? Email { set; get; }
        [Required]
        public string? Password { set; get; }
    }
}
