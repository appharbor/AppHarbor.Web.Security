using System.ComponentModel.DataAnnotations;

namespace AuthenticationExample.Web.ViewModels
{
	public class UserInputModel
	{
		[Required]
		[RegularExpression(@"^[A-Za-z0-9]*$", ErrorMessage = "Username may only contain letters and numbers")]
		public string Username { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 6, ErrorMessage = "Your password must be at least {2} characters long")]
		public string Password { get; set; }
	}
}
