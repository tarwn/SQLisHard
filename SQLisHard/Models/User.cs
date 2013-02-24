using SQLisHard.Core;
using SQLisHard.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLisHard.Models
{
	public class User
	{
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