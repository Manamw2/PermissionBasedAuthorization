using NAID_Users.Models;

namespace NAID_Users.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser appUser, string role);
    }
}
