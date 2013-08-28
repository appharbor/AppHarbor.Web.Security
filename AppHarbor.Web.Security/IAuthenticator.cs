
namespace AppHarbor.Web.Security
{
	using System.Security.Claims;

	public interface IAuthenticator
	{
		void SetCookie(ClaimsIdentity identity, bool persistent = false);
		void SignOut();
	}
}
