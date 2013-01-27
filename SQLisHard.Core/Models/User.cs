using SQLisHard.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Core.Models
{
	public class User : IUserIdentity
	{
		public User() { }

		public User(UserId userId)
		{
			Id = userId.Value;
		}

		public int Id { get; set;}
		public string Name { get; set; }

		UserId IUserIdentity.Id { get { return new UserId(Id); } }
	}

	public class UserId
	{
		public UserId() { }

		public UserId(int value)
		{
			Value = value;
		}
		public int Value { get; set; }

		public override string ToString()
		{
			return Value.ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj is UserId)
			{
				return this.Value.Equals(((UserId)obj).Value);
			}
			return false;
		}
	}
}
