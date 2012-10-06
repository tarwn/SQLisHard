using SQLisHard.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLisHard.Models
{
	public class Statement : Query
	{
		public string LessonId { get; set; }
	}
}