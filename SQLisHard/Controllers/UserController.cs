using SQLisHard.Attributes.WebAPI;
using SQLisHard.Core;
using SQLisHard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SQLisHard.Controllers
{
	public class UserController : ApiController
	{
		[RequiresUserOrGuest]
		public User GetLoggedInUser()
		{
			var user = (UserPrincipal)HttpContext.Current.User;
			return new User(user.UserIdentity);
		}

	}
}