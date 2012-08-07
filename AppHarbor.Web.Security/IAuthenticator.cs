
namespace AppHarbor.Web.Security
{
	public interface IAuthenticator
	{
		void SetCookie(string username, bool persistent = false, byte[] tag = null);
		void SignOut();
	}
}
