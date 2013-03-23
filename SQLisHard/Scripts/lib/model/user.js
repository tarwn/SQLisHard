
var SqlIsHardApp = SqlIsHardApp || {};
SqlIsHardApp.Model = SqlIsHardApp.Model || {};

SqlIsHardApp.Model.User = function (data) {
    var username = ko.observable(data.Username || ""),
        displayName = ko.observable(data.DisplayName || ""),
        isGuest = ko.observable(data.IsGuest || true),
        completedExercises = ko.observableArray(data.CompletedExercises || []);

    var markExerciseAsCompleted = function (exerciseId) {
        if (completedExercises.indexOf(exerciseId) == -1)
            completedExercises.push(exerciseId);
    };

    var hasCompleted = function (exerciseId) {
        return completedExercises.indexOf(exerciseId) > -1;
    };

    return {
        username: username,
        displayName: displayName,
        isGuest: isGuest,
        completedExercises: completedExercises,
        markExerciseAsCompleted: markExerciseAsCompleted,
        hasCompleted: hasCompleted
    };
};