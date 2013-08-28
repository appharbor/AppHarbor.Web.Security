
namespace AppHarbor.Web.Security
{
    using System.Security.Principal;

    public static class AuthenticatorExtensions
    {
        public static void SetCookie(this IAuthenticator authenticator, string username)
        {
            authenticator.SetCookie(new System.Security.Claims.ClaimsIdentity(new GenericIdentity(username, "cookie")));
        }
    }
}
