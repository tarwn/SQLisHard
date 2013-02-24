
var SqlIsHardApp = SqlIsHardApp || {};
SqlIsHardApp.Model = SqlIsHardApp.Model || {};

SqlIsHardApp.Model.User = function (data) {
    var username = ko.observable(data.Username || ""),
        displayName = ko.observable(data.DisplayName || ""),
        isGuest = ko.observable(data.IsGuest || true),
        completedExercises = ko.observableArray(data.CompletedExercises || []);

    return {
        username: username,
        displayName: displayName,
        isGuest: isGuest,
        completedExercises: completedExercises
    };
};