using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Domain.QueryEngine
{
    public class Query
    {
        public Query()
        {
            this.LimitResults = true;
        }

        public Query(Query anotherQuery)
        {
            this.Content = anotherQuery.Content;
            this.LimitResults = anotherQuery.LimitResults;
        }

        public string Content { get; set; }

        public bool LimitResults { get; set; }

    }
}
