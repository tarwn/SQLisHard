using SQLisHard.Attributes.WebAPI;
using SQLisHard.Core;
using SQLisHard.Core.Data;
using SQLisHard.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SQLisHard.Controllers
{
	public class UserController : ApiController
	{
        private HistoryStore _historyStore;

        public UserController() :
            this(new HistoryStore(ConfigurationManager.ConnectionStrings["CoreDatabase"].ConnectionString))
        { }

        public UserController(HistoryStore historyStore)
        {
            _historyStore = historyStore;
        }

        [RequiresUserOrGuest]
		public User GetLoggedInUser()
		{
			var httpUser = (UserPrincipal)HttpContext.Current.User;
			var user = new User(httpUser.UserIdentity);

            var completedExercises = _historyStore.GetCompletedExercises(user.Id);
            user.CompletedExercises = completedExercises;

            return user;
		}

	}
}