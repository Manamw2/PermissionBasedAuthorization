using System.Security.Claims;

namespace NAID_Users.Extensions
{
    public static class ClaimsExtensions
    {
        public static string? GetUserName(this ClaimsPrincipal user)
        {
            var claim = user.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"));
            return claim == null ? null : claim.Value;
        }
        public static string? GetUserEmail(this ClaimsPrincipal user)
        {
            var claim = user.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/email"));
            return claim == null ? null : claim.Value;
        }
    }
}
