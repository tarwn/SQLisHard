
var SqlIsHardApp = SqlIsHardApp || {};
SqlIsHardApp.Model = SqlIsHardApp.Model || {};

SqlIsHardApp.Model.ExerciseSet = function (data) {
    // private variables
    var id = ko.observable(data.Id || ""),
        title = ko.observable(data.Title || "Exercises"),
        summary = ko.observable(data.Summary || ""),
        currentExerciseIndex = ko.observable(0),
        finale = ko.observable(SqlIsHardApp.Model.Exercise(data.Finale));

    var rawExercises = [];
    for (var ex in data.Exercises)
        rawExercises[ex] = SqlIsHardApp.Model.Exercise(data.Exercises[ex]);
    var exercises = ko.observableArray(rawExercises);

    exercises.debug = ko.computed(function () {
        return currentExerciseIndex();
    }, this);

    // computed variables
    var currentExercise = ko.computed(function () {
        if (currentExerciseIndex() == Constants.ExercisePlaceHolder.FINALE)
            return finale();
        else
            return exercises()[currentExerciseIndex()];
    }, this);

    // methods
    var advanceExercise = function () {
        if (currentExerciseIndex() != Constants.ExercisePlaceHolder.FINALE && parseInt(currentExerciseIndex()) + 1 < exercises().length)
            currentExerciseIndex(parseInt(currentExerciseIndex()) + 1);
        else
            currentExerciseIndex(Constants.ExercisePlaceHolder.FINALE);
    };
    var previousExercise = function () {
        if (currentExerciseIndex() > 0)
            currentExerciseIndex(currentExerciseIndex() - 1);
        else
            currentExerciseIndex(0);
    };
    var goToExercise = function (id) {
        var rawExercises = exercises();
        for (var ex in rawExercises) {
            if (rawExercises[ex].id == id)
                currentExerciseIndex(ex);
        }
        // error or eat it?
    };

    return {
        id: id,
        title: title,
        summary: summary,
        exercises: exercises,
        currentExercise: currentExercise,
        advanceExercise: advanceExercise,
        previousExercise: previousExercise,
        goToExercise: goToExercise
    };
}

SqlIsHardApp.Model.Exercise = function (data) {
    var id = data.Id || "",
        title = data.Title || "",
        explanation = data.Explanation || data.Details || "",
        example = data.Example || null,
        exercise = data.ExerciseDescription || "";

    return {
        id: id,
        title: title,
        explanation: explanation,
        example: example,
        exercise: exercise
    };
}

SqlIsHardApp.Model.StatementResult = function (responseData) {
    // map incoming data
    var exerciseId = ko.observable(responseData.ExerciseId || ""),
        completesExercise = ko.observable(responseData.CompletesExercise || false),
        tip = ko.observable(responseData.Tip || null),
        queryContent = ko.observable(responseData.Content || ""),
        data = {
            headers: ko.observableArray(responseData.Data.Headers || []),
            rows: ko.observableArray(responseData.Data.Rows || [])
        },
        isSubsetOfRows = ko.observable(responseData.IsSubsetOfRows || false),
        totalRowCount = ko.observable(responseData.TotalRowCount || 0),
        errorMessage = ko.observable(responseData.ErrorMessage || ""),
        errorNumber = ko.observable(responseData.ErrorNumber || 0),
        executionStatus = ko.observable(responseData.ExecutionStatus || "");

    // computed fields
    var
        isError = ko.computed(function () {
            return executionStatus() != Constants.ExecutionStatus.SUCCESS
        }, this),
        isServerError = ko.computed(function () {
            return executionStatus() == Constants.ExecutionStatus.SERVER_ERROR;
        }, this);

    return {
        exerciseId: exerciseId,
        completesExercise: completesExercise,
        tip: tip,
        queryContent: queryContent,
        data: data,
        isSubsetOfRows: isSubsetOfRows,
        totalRowCount: totalRowCount,
        errorMessage: errorMessage,
        errorNumber: errorNumber,
        executionStatus: executionStatus,
        isError: isError,
        isServerError: isServerError
    };
}