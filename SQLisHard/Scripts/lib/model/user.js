
var SqlIsHardApp = SqlIsHardApp || {};
SqlIsHardApp.Model = SqlIsHardApp.Model || {};

SqlIsHardApp.Model.User = function (data) {
    var username = ko.observable(data.Username || ""),
        isGuest = ko.observable(data.isGuest || true),
        completedExercises = ko.observableArray(data.CompletedExercises || []);

    return {
        username: username,
        isGuest: isGuest,
        completedExercises: completedExercises
    };
};