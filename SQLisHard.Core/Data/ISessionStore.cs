using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Core.Data
{
	public interface ISessionStore
	{
		void CaptureSession(Models.Session session);
	}
}
