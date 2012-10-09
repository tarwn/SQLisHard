using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Domain.DatabaseExecution
{
    public interface IQueryEngine
    {
		QueryResult ExecuteQuery(Query query);
    }
}
