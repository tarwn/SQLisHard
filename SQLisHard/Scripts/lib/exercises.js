
SqlIsHardApp.ExerciseSet = function (data) {
    // private variables
    var title = ko.observable(data.Title || "Exercises"),
        summary = ko.observable(data.Summary || ""),
        currentExerciseIndex = ko.observable(0),
        finale = ko.observable(SqlIsHardApp.Exercise(data.Finale));

    var rawExercises = [];
    for (var ex in data.Exercises)
        rawExercises[ex] = SqlIsHardApp.Exercise(data.Exercises[ex]);
    var exercises = ko.observableArray(rawExercises);

    // computed variables
    var currentExercise = ko.computed(function () {
        if (currentExerciseIndex() == Constants.ExercisePlaceHolder.FINALE)
            return finale();
        else
            return exercises()[currentExerciseIndex()];
    }, this);

    // methods
    var advanceExercise = function () {
        if (currentExerciseIndex() != Constants.ExercisePlaceHolder.FINALE && currentExerciseIndex() + 1 < exercises().length)
            currentExerciseIndex(currentExerciseIndex() + 1);
        else
            currentExerciseIndex(Constants.ExercisePlaceHolder.FINALE);
    };

    return {
        title: title,
        summary: summary,
        exercises: exercises,
        currentExercise: currentExercise,
        advanceExercise: advanceExercise
    };
}

SqlIsHardApp.Exercise = function (data) {
    var id = data.Id || "",
        title = data.Title || "",
        details = data.Details || "";

    return {
        id: id,
        title: title,
        details: details
    };
}