/// <reference path="services/routes.js" />
/// <reference path="viewmodel.js" />
/// <reference path="constants.js" />

var SqlIsHardApp = SqlIsHardApp || {};

SqlIsHardApp.init = function (ko, jQuery, infuser, baseUrl, statementPostUrl, exercisesUrl, services) {
	// infuser defaults for templates
	infuser.defaults.templatePrefix = "_";
	infuser.defaults.templateSuffix = ".tmpl.html";
	infuser.defaults.templateUrl = baseUrl + "templates";

    // Service Routes
	SqlIsHardApp.Services.Routes.baseUrl = baseUrl;
	SqlIsHardApp.Services.Routes.statementPostUrl = statementPostUrl;
	SqlIsHardApp.Services.Routes.exercisesUrl = exercisesUrl;

    // Initialize viewmodel
	services = services || SqlIsHardApp.Services.CreateDefaultDataServices(ko, jQuery, SqlIsHardApp.Services.Routes);
	SqlIsHardApp.ViewModel.init(services, Constants.Text);
}
