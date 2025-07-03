Feature: BasicQueryExecution
	As an end user
	When I execute a query
	I expect to see the results displayed

@UI
Scenario: Execute basic successful query
	Given I am on the Exercise Page
	And I have entered a query of "SELECT TOP 10 * FROM dbo.Customers"
	When I Press Execute
	Then the query results are displayed

@UI
Scenario: Execute query with syntax error
	Given I am on the Exercise Page
	And I have entered a query of "Not a real query"
	When I Press Execute
	Then the query reports an error

@UI
Scenario: Status is set prior to running the query
	Given I am on the Exercise Page
	Then the query status displays "Ready"

@UI
Scenario: Execute basic successful query and see status update
	Given I am on the Exercise Page
	And I have entered a query of "SELECT TOP 10 * FROM dbo.Customers"
	When I Press Execute
	Then the query status displays "Ready"

@UI
Scenario: Execute query with syntax error and see status update
	Given I am on the Exercise Page
	And I have entered a query of "Not a real query"
	When I Press Execute
	Then the query status displays "Completed with error"

@UI
Scenario: Execute query with more than 100 results
	Given I am on the Exercise Page
	And I have entered a query of "SELECT TOP 101 * FROM dbo.Customers"
	When I Press Execute
	Then 100 query results are displayed with a link to read more
	And 100 result rows are displayed

@UI
Scenario: Get rest of results when query has more than 100
	Given I am on the Exercise Page
	And I have entered a query of "SELECT TOP 101 * FROM dbo.Customers"
	And I Press Execute
	And 100 query results are displayed with a link to read more
	When I click the read more link
	Then 101 result rows are displayed without the read more link

@UI
Scenario: Get tip when making an error for a pattern-based exercise
	Given I am on the Exercise Page
	And I have selected a pattern-based exercise
	And I have entered a query of "SELECT TOP 101 * FROM dbo.Customers"
	When I Press Execute
	Then a tip is shown for the pattern
