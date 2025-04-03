/// <reference path="../lib/jasmine-1.3.1/jasmine.js" />
/// <reference path="../lib/jasmine-1.3.1/jasmine-html.js" />
/// <reference path="../../lib/constants.js" />
/// <reference path="../../lib/model/exercises.js" />
/// <reference path="../../lib/model/user.js" />
/// <reference path="../../lib/services/services.js" />
/// <reference path="../../lib/viewmodel.js" />

var getMockDataService = function () {
    return {
        exercises: mock(SqlIsHardApp.Services.ExerciseService(null, null, null)),
        users: mock(SqlIsHardApp.Services.UserService(null, null, null))
    }
};

describe("SqlIsHard.ViewModel", function () {
    
    //it("Initializes the query entry window with helpful text", function () {
    //    var testServices = getMockDataService();
    //    var viewmodel = SqlIsHardApp.ViewModel;
        
    //    viewmodel.init(testServices, Constants.Text);

    //    expect(viewmodel.currentQuery.queryText()).toEqual(Constants.Text["QUERY_INITIAL_TEXT"]);
    //});

    //it("Requests an updated exercise list on initialization", function () {
    //    var testServices = getMockDataService();
    //    var viewmodel = SqlIsHardApp.ViewModel;

    //    viewmodel.init(testServices, Constants.Text);

    //    expect(testServices.exercises.getExerciseList).toHaveBeenCalled();
    //});

    //describe("QueryExecution", function () {

    //    it("Displays error message when the query returns an error", function () {
    //        var testServices = getMockDataService();
    //        when(testServices.exercises.executeQuery).isCalledWith(argThat(isAnyValue), argThat(isAnyValue), argThat(isAnyValue)).thenCall(2, 
    //        var viewmodel = SqlIsHardApp.ViewModel;
    //        viewmodel.init(testServices, Constants.Text);

    //        viewmodel.executeQuery(true);

    //        expect(testServices.exercises.getExerciseList).toHaveBeenCalled();
    //    });

    //});
});