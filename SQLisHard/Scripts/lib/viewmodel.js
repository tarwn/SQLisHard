
var SqlIsHardApp = SqlIsHardApp || {};

SqlIsHardApp.Routes = SqlIsHardApp.Routes || {};

SqlIsHardApp.init = function (statementPostUrl) {
    // Routes
    SqlIsHardApp.Routes.statementPostUrl = statementPostUrl;

    // infuser defaults for templates
    infuser.defaults.templatePrefix = "_";
    infuser.defaults.templateSuffix = ".tmpl.html";
    infuser.defaults.templateUrl = "/templates";
}

/* Constants */
var Constants = Constants || {};
Constants.ExecutionStatus = {
    SERVER_ERROR: 2,
    ERROR: 1,
    SUCCESS: 0
};
Constants.ExercisePlaceHolder = {
    FINALE: -100
}

/* Objects */
SqlIsHardApp.StatementResult = function (responseData) {
    // map incoming data
    var exerciseId = ko.observable(responseData.ExerciseId || ""),
        completesExercise = ko.observable(responseData.CompletesExercise || false),
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
    var isError = ko.computed(function () {
            return executionStatus() != Constants.ExecutionStatus.SUCCESS
        }, this),
        isServerError = ko.computed(function () {
            return executionStatus() == Constants.ExecutionStatus.SERVER_ERROR;
        }, this);

    return {
        exerciseId: exerciseId,
        completesExercise: completesExercise,
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

SqlIsHardApp.ExerciseSet = function (data) {
    // private variables
    var title = ko.observable(data.Title || "Exercises"),
        summary = ko.observable(data.Summary || ""),
        currentExerciseIndex = ko.observable(0),
        exercises = ko.observableArray(data.Exercises || []),
        finale = ko.observable(data.Finale);

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

/* Viewmodel */
SqlIsHardApp.ViewModel = (function (ko, $, api, isDebug) {
    // Private variables
    var exercises = ko.observable(),
        user = ko.observable(),
        currentQuery = {
            queryText: ko.observable(""),
            isRunning: ko.observable(false),
            queryResult: ko.observable(null),
            toStatementDTO: function (limitResults) {
                return {
                    exerciseId: exercises().currentExercise().id,
                    content: currentQuery.queryText,
                    limitResults: limitResults
                };
            }
        },
        isDebug = ko.observable(isDebug || false);

    // Methods
    var executeQuery = function (limitResults) {
        currentQuery.queryResult(null);
        currentQuery.isRunning(true);
        $.ajax(api.statementPostUrl, {
            type: "POST", contentType: "application/json", dataType: "json",
            data: ko.toJSON(currentQuery.toStatementDTO(limitResults)),
            success: function (result) {
                currentQuery.isRunning(false);
                currentQuery.queryResult(SqlIsHardApp.StatementResult(result));
            },
            error: function (xhr, status, error) {
                currentQuery.isRunning(false);
                currentQuery.queryResult(SqlIsHardApp.StatementResult({
                    Data: { Headers: [], Rows: []},
                    ErrorMessage: error,
                    ExecutionStatus: Constants.ExecutionStatus.SERVER_ERROR
                }));
            }
        });
    };

    var setExercises = function (exerciseSetData) {
        exercises(SqlIsHardApp.ExerciseSet(exerciseSetData));
    };


    return {
        // properties
        exercises: exercises,
        user: user,
        currentQuery: currentQuery,
        isDebug: isDebug,
        // methods
        executeQuery: executeQuery,
        setExercises: setExercises
    };
})(ko, jQuery, SqlIsHardApp.Routes, false);