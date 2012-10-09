using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Domain.LessonEvaluator
{
    public interface ILessonResultEvaluator
    {
        StatementResult Evaluate(Statement statement);
    }
}
