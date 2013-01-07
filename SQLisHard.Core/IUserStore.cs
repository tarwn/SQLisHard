using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Core
{
    public interface UserStore
    {
		User GetNewGuestUser();
		User GetUser(UserId userId);
    }
}
