using SQLisHard.Core;
using SQLisHard.Core.Interfaces;
using SQLisHard.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLisHard.Models
{
	public class User
	{
        public UserId Id { get; set; }
		public string UserName { get; set; }
		public string DisplayName { get;set; }
		public bool IsGuestUser { get; set; }
		public List<string> CompletedExercises { get; set; }

		public User() { }

		public User(IUserIdentity userIdentity)
		{
			if (userIdentity is GuestUser)
			{
				var guest = (GuestUser)userIdentity;
                Id = guest.Id;
				UserName = "";
				DisplayName = guest.Name;
				IsGuestUser = true;
				CompletedExercises = new List<string>();
			}
			else
			{
				throw new NotImplementedException("Non-Guest users not yet implemented");
			}
		}
	}
}