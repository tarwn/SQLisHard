
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

/* Viewmodel */
SqlIsHardApp.ViewModel = (function (ko, $, api, isDebug) {
    // Private variables
    var exercises = ko.observable(SqlIsHardApp.ExerciseSet(Constants.InitialExerciseData)),
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

    var updateExercises = function ()
    {
        $.ajax(api.exercisesUrl, {
            type: "GET",
            contentType: "application/json",
            dataType: "json",
            success: function (result) {
                exercises(SqlIsHardApp.ExerciseSet(result));
            },
            error: function (xhr, status, error){
                alert("TODO: tell Eli he didn't handle the error for /exercises/list");
            }
        });
    }


    return {
        // properties
        exercises: exercises,
        user: user,
        currentQuery: currentQuery,
        isDebug: isDebug,
        // methods
        executeQuery: executeQuery,
        setExercises: setExercises,
        updateExercises: updateExercises
    };
})(ko, jQuery, SqlIsHardApp.Routes, false);