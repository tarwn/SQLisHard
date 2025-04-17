using SQLisHard.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Core.Interfaces
{
	public interface IUserIdentity
	{
		UserId Id { get; }
		string Name { get; }
	}
}
