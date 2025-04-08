using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Core.Data
{
	public class SessionStore : ISessionStore
	{
		private string _connectionString;

		public SessionStore(string connectionString)
		{
			_connectionString = connectionString;
		}

		public void CaptureSession(Models.Session session)
		{
			using (var db = new Database(_connectionString, "System.Data.SqlClient"))
			{
				db.Insert("Session", "Id", true, session);
			}
		}
	}
}
