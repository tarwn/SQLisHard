/// <reference path="../model/exercises.js" />
/// <reference path="../model/user.js" />

var SqlIsHardApp = SqlIsHardApp || {};
SqlIsHardApp.Services = SqlIsHardApp.Services || {};

SqlIsHardApp.Services.CreateDefaultDataServices = function (ko, jQuery, routes) {
    return{
        exercises: SqlIsHardApp.Services.ExerciseService(ko, jQuery, routes),
        users: SqlIsHardApp.Services.UserService(ko, jQuery, routes)
    }
};

SqlIsHardApp.Services.ExerciseService = function (ko, $, routes) {

    var getExerciseList = function (exerciseSet, successCallback, errorCallback) {
        $.ajax(routes.exercisesUrl, {
            type: "GET",
            //            data: { exerciseSet: exerciseSet },
            contentType: "application/json",
            dataType: "json",
            success: function (result) {
                successCallback(SqlIsHardApp.Model.ExerciseSet(result));
            },
            error: function (xhr, status, error) {
                errorCallback(error, {});
            }
        });
    };

    var executeQuery = function (statementDto, responseCallback) {
        $.ajax(routes.statementPostUrl, {
            type: "POST", contentType: "application/json", dataType: "json",
            data: ko.toJSON(statementDto),
            success: responseCallback,
            error: function (xhr, status, error) {
                responseCallback({
                    Data: { Headers: [], Rows: [] },
                    ErrorMessage: error,
                    ExecutionStatus: Constants.ExecutionStatus.SERVER_ERROR,
                    CompletesExercise: false
                });
            }
        });
    };

    return {
        getExerciseList: getExerciseList,
        executeQuery: executeQuery
    };
};

SqlIsHardApp.Services.UserService = function (ko, $, routes) {

    var getLoggedInUser = function (successCallback, errorCallback) {
        $.ajax(routes.userUrl, {
            type: "GET",
            contentType: "application/json",
            dataType: "json",
            success: function (result) {
                successCallback(SqlIsHardApp.Model.User(result));
            },
            error: function (xhr, status, error) {
                errorCallback(error, {});
            }
        });
    };

    return {
        getLoggedInUser: getLoggedInUser
    };
};