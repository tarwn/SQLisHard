Feature: BasicQueryExecution
	As an end user
	When I execute a query
	I expect to see the results displayed

@UI
Scenario: Execute basic successful query
	Given I am on the Lesson Page
	And I have entered a query of "SELECT TOP 10 * FROM dbo.Clients"
	When I Press Execute
	Then the query results are displayed
