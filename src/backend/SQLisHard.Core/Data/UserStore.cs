using SQLisHard.Core.Models;
using System;

namespace SQLisHard.Core.Data
{
	public class UserStore : IUserStore
	{
		private string _connectionString;

		public UserStore(string connectionString)
		{
			_connectionString = connectionString;
		}

		public Models.User GetNewGuestUser()
		{
			using (var db = new PetaPoco.Database(_connectionString, "System.Data.SqlClient"))
			{
				var result = db.Insert("User", "Id", true, new { Name = String.Empty });
				return new User(new UserId(Convert.ToInt32(result)));
			}
		}

		public Models.User GetUser(Models.UserId userId)
		{
			using (var db = new PetaPoco.Database(_connectionString, "System.Data.SqlClient")) {
				var rawUser = db.Single<User>("where Id = @0", userId.Value);
				if (rawUser == null)
					throw new UserNotFoundException();
				else
					return rawUser;
			}
		}
	}
}
