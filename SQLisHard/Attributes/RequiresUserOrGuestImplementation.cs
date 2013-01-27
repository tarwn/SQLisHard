using SQLisHard.Core;
using SQLisHard.Core.Models;
using SQLisHard.General;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace SQLisHard.Attributes
{
	// contains all the direct access to HttpContext that I will one day regret

	public class RequiresUserOrGuestImplementation
	{
		public static void EnsureUserIsAuthenticatedOrProvidedWithAGuestAccount()
		{
			Guard.AgainstNullArgument("HttpContext.Current", HttpContext.Current);	// using IIS, this should never be the case, but I want to know if I am wrong

			var coreMembership = new CoreMembership(ConfigurationManager.ConnectionStrings["CoreDatabase"].ConnectionString);
			if (!HttpContext.Current.User.Identity.IsAuthenticated)
			{
				var user = coreMembership.CreateGuest();
				HttpContext.Current.User = user;
				FormsAuthentication.SetAuthCookie(user.UserIdentity.Id.ToString(), false);
				coreMembership.CaptureSession(user.UserIdentity.Id, HttpContext.Current.Request.UserAgent, HttpContext.Current.Request.UserHostAddress);
			}
			else
			{
				var id = new UserId(Convert.ToInt32(HttpContext.Current.User.Identity.Name));
				var user = coreMembership.GetUser(id);
				HttpContext.Current.User = user;
			}
		}
	}
}