/// <reference path="constants.js" />

var SqlIsHardApp = SqlIsHardApp || {};

SqlIsHardApp.ViewModel = (function (ko, Finch, isDebug) {
    // Initialization
    var dataService = null,
        appInsightsInstance = null,
        initialize = function (actualDataService, textResource, appInsights) {
            dataService = actualDataService;
            appInsightsInstance = appInsights;
            currentQuery.queryText(textResource['QUERY_INITIAL_TEXT']);
            
            Finch.route("/exercises/:exerciseSet", function (args, childCallback) {
                //console.log("route /exercises/:exerciseSet");
                updateExercises(args.exerciseSet, childCallback);
            });

            Finch.route("[/exercises/:exerciseSet]/:exercise", function (args) {
                //console.log("route [/exercises/:exerciseSet]/:exercise");
                exercises().goToExercise(args.exercise);
                trackPageView(args.exerciseSet, args.exercise);
            });

            Finch.route("/", function () {
                //console.log("route /");
                Finch.call("/exercises/SELECT");
            });
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


    // private tracking functions
    function trackPageView(exerciseSet, exercise) {
        if (!appInsightsInstance) return;

        appInsightsInstance.trackPageView("Exercise " + exerciseSet + " " + exercise, window.location.hash, {
            app: 'SQLisHard',
            exerciseSet: exerciseSet,
            exercise: exercise
        });
    }

    function trackEvent(eventName, data) {
        if (!appInsightsInstance) return;

        data.app = 'SQLisHard';
        appInsightsInstance.trackEvent(eventName, data);
    }

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

                trackEvent('executeQuery', {
                    exerciseSet: exercises().id,
                    exercise: exercises().currentExercise().id,
                    completedSuccessfully: exerciseCompleted
                });

            });
        },
        updateExercises = function (exerciseSetName, callback) {
            dataService.exercises.getExerciseList(exerciseSetName,
                function (exerciseSet) {
                    exercises(exerciseSet);
                    if(callback)
                        callback();
                },
                function (error, rawData) {
                    exercises(SqlIsHardApp.Model.ExerciseSet({
                        Id: exerciseSetName,
                        Title: 'Unknown Set',
                        Summary: 'Selected exercise set "' + exerciseSetName + '" could not be found.',
                        Finale: {
                            Title: 'Completed Unknown Exercise Set',
                            Details: 'Details of this missing exercise set have been sent to Eli to look into.'
                        },
                        Exercises: [{
                            Id: 'Unknown',
                            Title: 'Exercises Not Found',
                            Explanation: 'Sorry, no exercises could be located for a set named "' + exerciseSetName + '". Eli has been notified with the details so he can look into it.',
                            Example: '',
                            ExerciseDescription: '<br/>Why not try the <a href="#/exercises/SELECT">SELECT exercises</a> instead?'
                        }]
                    }));
                    if (callback)
                        callback();
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

    // shortcut method b/c menus are currently li's and I don't want to mess with re-CSSing them right now
    var navigateTo = function (url) {
        console.log("going to " + url);
        Finch.navigate(url);
    };

    return {
        // init
        init: initialize,
        // properties
        exercises: exercises,
        exerciseSets: ['SELECT','AGGREGATE'],
        user: user,
        currentQuery: currentQuery,
        isDebug: isDebug,
        selectedResultsTab: selectedResultsTab,
        // methods
        executeQuery: executeQuery,
        updateExercises: updateExercises,
        selectResultsTab: selectResultsTab,
        selectMessagesTab: selectMessagesTab,
        navigateTo: navigateTo
    };
})(ko, Finch, false);