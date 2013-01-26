using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace SQLisHard.Attributes.WebAPI
{
	/// <summary>
	/// This ensures any user attempting to access is already logged in or, if not, gets a new guest record and is allowed in
	/// </summary>
	public class RequiresUserOrGuestAttribute : AuthorizeAttribute
	{
		public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			RequiresUserOrGuestImplementation.EnsureUserIsAuthenticatedOrProvidedWithAGuestAccount();
		}
	}
}