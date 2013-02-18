
var SqlIsHardApp = SqlIsHardApp || {};

SqlIsHardApp.Routes = SqlIsHardApp.Routes || {};

SqlIsHardApp.init = function (baseUrl, statementPostUrl, exercisesUrl) {
	// Routes
	SqlIsHardApp.Routes.baseUrl = baseUrl;
	SqlIsHardApp.Routes.statementPostUrl = statementPostUrl;
	SqlIsHardApp.Routes.exercisesUrl = exercisesUrl;

	// infuser defaults for templates
	infuser.defaults.templatePrefix = "_";
	infuser.defaults.templateSuffix = ".tmpl.html";
	infuser.defaults.templateUrl = baseUrl + "templates";
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
Constants.InitialExerciseData = {
	Title: "Exercises",
	Summary: "Loading exercises...",
	Exercises: [{ Id: "", Title: "loading...", Details: "loading..." }],
	Finale: ""
};