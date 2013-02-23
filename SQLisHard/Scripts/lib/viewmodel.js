
var SqlIsHardApp = SqlIsHardApp || {};

SqlIsHardApp.ViewModel = (function (ko, $, isDebug) {
    // Service Dependencies
    var dataService = null,
        initializeDependencies = function (actualDataService) {
            dataService = actualDataService
        };

    // Private variables
    var exercises = ko.observable(SqlIsHardApp.Model.ExerciseSet(Constants.InitialExerciseData)),
        user = ko.observable(null),
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

    // Exercise Methods
    var
        executeQuery = function (limitResults) {
            currentQuery.queryResult(null);
            currentQuery.isRunning(true);

            dataService.exercises.executeQuery(currentQuery.toStatementDTO(limitResults), function (data) {
                currentQuery.isRunning(false);
                currentQuery.queryResult(SqlIsHardApp.Model.StatementResult(data));

                if (currentQuery.queryResult().completesExercise()) {
                    exercises().advanceExercise();
                }

                if (user() == null) {
                    loadUser();
                }
                else if (currentQuery.queryResult().completesExercise()) {
                    user().markExerciseAsCompleted(currentQuery().exerciseId);
                }
            });
        },
        updateExercises = function () {
            dataService.exercises.getExerciseList("SELECT",
                function (exerciseSet) {
                    exercises(exerciseSet);
                },
                function (error, rawData) {
                    alert("TODO: tell Eli he didn't handle the error for /exercises/list");
                });
        };

    // User methods
    var
        loadUser = function () {
            user(SqlIsHardApp.Model.User({ username: "TestUser", isGuest: true }));
        };

    return {
        // init
        init: initializeDependencies,
        // properties
        exercises: exercises,
        user: user,
        currentQuery: currentQuery,
        isDebug: isDebug,
        // methods
        executeQuery: executeQuery,
        updateExercises: updateExercises
    };
})(ko, jQuery, false);