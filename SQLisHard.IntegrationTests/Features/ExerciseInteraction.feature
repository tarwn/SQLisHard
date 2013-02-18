Feature: ExerciseInteraction
	As a user
	I want good visual hints on the exercise state
	So I know what the status of my exercise is

@UI
Scenario: Exercise state starts with first exercise selected
	Given I am on the Exercise Page
	Then the 1st entry on the Exercise list is selected

	@UI
Scenario: Exercise selection advances when exercise completed
	Given I am on the Exercise Page
	When I complete the first exercise
	Then the 2nd entry on the Exercise list is selected