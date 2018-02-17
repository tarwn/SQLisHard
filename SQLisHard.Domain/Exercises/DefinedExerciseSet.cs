using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Domain.Exercises
{
    public class DefinedExerciseSet
    {
        public DefinedExerciseSet(string id)
        {
            Id = id;
            Finale = new DefinedFinale();
            Exercises = new List<DefinedExercise>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DefinedFinale Finale { get; set; }
        public List<DefinedExercise> Exercises { get; set; }
    }

    public class DefinedExercise
    {
        public DefinedExercise(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
        public string Title { get; set; }

        public string Query { get; set; }

        public string Pattern { get; set; }
        public string PatternTip { get; set; }

        public string Explanation { get; set; }
        public string Example { get; set; }
        public string Exercise { get; set; }
        public bool RequireExactColumnMatch { get; set; }
    }

    public class DefinedFinale
    {
        public string Title { get; set; }
        public string Details { get; set; }
    }
}
