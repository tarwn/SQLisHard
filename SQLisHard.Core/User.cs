using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Core
{
	public class User
	{
		public UserId Id { get; set; }
		public string Name { get;set; }
	}

	public class UserId 
	{
		public decimal Value { get; set; }
	}
}
