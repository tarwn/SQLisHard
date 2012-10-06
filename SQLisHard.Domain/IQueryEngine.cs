using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Domain
{
    public interface IQueryEngine
    {
		QueryResult ExecuteQuery(Query query);
    }
}
