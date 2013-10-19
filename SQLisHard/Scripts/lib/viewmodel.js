/// <reference path="constants.js" />

var SqlIsHardApp = SqlIsHardApp || {};

SqlIsHardApp.ViewModel = (function (ko, isDebug) {
    // Initialization
    var dataService = null,
        initialize = function (actualDataService, textResource) {
            dataService = actualDataService;
            currentQuery.queryText(textResource['QUERY_INITIAL_TEXT']);
            updateExercises();
        };

    // Private variables
    var 
    exercises = ko.observable(SqlIsHardApp.Model.ExerciseSet(Constants.InitialExerciseData)),
    user = ko.observable(null),
    currentQuery = {
        queryText: ko.observable(""),
        isRunning: ko.observable(false),
        queryResult: ko.observable(null),
        toStatementDTO: function (limitResults) {
            return {
                exerciseSetId: exercises().id(),
                exerciseId: exercises().currentExercise().id,
                content: currentQuery.queryText,
                limitResults: limitResults
            };
        }
    },
    isDebug = ko.observable(isDebug || false),
    selectedResultsTab = ko.observable(Constants.ResultsTab.Results);

    // UI Methods
    var
    selectResultsTab = function (vm) {
        vm.selectedResultsTab(Constants.ResultsTab.Results);
    },
    selectMessagesTab = function (vm) {
        vm.selectedResultsTab(Constants.ResultsTab.Messages);
    };

    // Exercise Methods
    var
        executeQuery = function (limitResults) {
            currentQuery.queryResult(null);
            currentQuery.isRunning(true);
            var exerciseId = exercises().currentExercise().id;
            
            dataService.exercises.executeQuery(currentQuery.toStatementDTO(limitResults), function (data) {
                currentQuery.isRunning(false);
                currentQuery.queryResult(SqlIsHardApp.Model.StatementResult(data));
                
                var queryContent = currentQuery.queryResult().queryContent();
                if(queryContent != "")
                    currentQuery.queryText(queryContent);

                var exerciseCompleted = currentQuery.queryResult().completesExercise();

                if (currentQuery.queryResult().isError()) {
                    selectedResultsTab(Constants.ResultsTab.Messages);
                }
                else {
                    selectedResultsTab(Constants.ResultsTab.Results);
                }

                //if (exerciseCompleted) {
                //    exercises().advanceExercise();
                //}

                if (user() == null) {
                    loadUser(function () {
                        if (exerciseCompleted) {
                            user().markExerciseAsCompleted(exerciseId);
                        }
                    });
                }
                else if (exerciseCompleted) {
                    user().markExerciseAsCompleted(exerciseId);
                }

            });
        },
        updateExercises = function () {
            dataService.exercises.getExerciseList("SELECT",
                function (exerciseSet) {
                    exercises(exerciseSet);
                },
                function (error, rawData) {
                    alert("TODO: tell Eli he didn't handle the error for exercises.getExerciseList");
                });
        };

    // User methods
    var
        loadUser = function (postLoad) {
            dataService.users.getLoggedInUser(
                function (userModel) {
                    user(userModel);
                    postLoad();
                },
                function (error, rawData) {
                    alert("TODO: tell Eli he didn't handle the error for users.getLoggedInUser");
                });
        };

    return {
        // init
        init: initialize,
        // properties
        exercises: exercises,
        user: user,
        currentQuery: currentQuery,
        isDebug: isDebug,
        selectedResultsTab: selectedResultsTab,
        // methods
        executeQuery: executeQuery,
        updateExercises: updateExercises,
        selectResultsTab: selectResultsTab,
        selectMessagesTab: selectMessagesTab
    };
})(ko, false);