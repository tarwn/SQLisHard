using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Domain.QueryEngine
{
	public class Query
	{
        public Query() { }

        public Query(Query anotherQuery)
        {
            this.Content = anotherQuery.Content;
        }
		public string Content { get; set; }

	}
}
