using System.ComponentModel.DataAnnotations;

namespace AuthenticationExample.Web.ViewModels
{
	public class SessionViewModel
	{
		[Required]
		public string Username { get; set; }
		[Required]
		public string Password { get; set; }
		public string ReturnUrl { get; set; }
	}
}
