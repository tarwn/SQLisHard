using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Domain.ExerciseEvaluator
{
    public interface IExerciseResultEvaluator
    {
        StatementResult Evaluate(Statement statement);
    }
}
