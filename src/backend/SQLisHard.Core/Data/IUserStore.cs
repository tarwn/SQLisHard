using SQLisHard.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Core.Data
{
    public interface IUserStore
    {
		User GetNewGuestUser();
		User GetUser(UserId userId);
    }
}
