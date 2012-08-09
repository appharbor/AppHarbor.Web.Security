
namespace AppHarbor.Web.Security
{
	public interface IAuthenticator
	{
		void SetCookie(string username, bool persistent = false, string[] roles = null, byte[] tag = null);
		void SignOut();
	}
}
