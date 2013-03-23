Feature: ExerciseInteraction
	As a user
	I want good visual hints on the exercise state
	So I know what the status of my exercise is

@UI
Scenario: Exercise state starts with first exercise selected
	Given I am on the Exercise Page
	Then the 1st entry on the Exercise list is selected

@UI
Scenario: Exercise selection displays continue button when complete
	Given I am on the Exercise Page
	When I complete the first exercise
	Then the complete button is displayed

@UI
Scenario: Continue button moves to next exercise
	Given I am on the Exercise Page
	When I complete the first exercise
	And I press the Complete Button
	Then the 2nd entry on the Exercise list is selected