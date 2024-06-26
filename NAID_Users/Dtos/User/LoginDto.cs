using System.ComponentModel.DataAnnotations;

namespace NAID_Users.Dtos.User
{
    public class LoginDto
    {
        [Required]
        public string UserNameOrEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
