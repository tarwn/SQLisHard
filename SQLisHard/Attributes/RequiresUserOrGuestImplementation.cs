using SQLisHard.Core.Models;
using SQLisHard.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace SQLisHard.Attributes
{
	public class RequiresUserOrGuestImplementation
	{
		public static void EnsureUserIsAuthenticatedOrProvidedWithAGuestAccount()
		{
			Guard.AgainstNullArgument("HttpContext.Current", HttpContext.Current);	// using IIS, this should never be the case, but I want to know if I am wrong

			var coreMembership = new CoreMembership();
			if (!HttpContext.Current.User.Identity.IsAuthenticated)
			{
				var user = coreMembership.CreateGuest();
				HttpContext.Current.User = user;
				FormsAuthentication.SetAuthCookie(user.UserIdentity.Id.ToString(), false);
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