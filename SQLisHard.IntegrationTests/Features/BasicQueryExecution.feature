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

@UI
@Unreleased
Scenario: Execute query with syntax error
	Given I am on the Lesson Page
	And I have entered a query of "Not a real query"
	When I Press Execute
	Then the query reports an error

@UI
@Unreleased
Scenario: Execute query with more than 100 results
	Given I am on the Lesson Page
	And I have entered a query of "SELECT TOP 101 * FROM dbo.Clients"
	When I Press Execute
	Then 100 query results are displayed with a link to read more
	And 100 result rows are displayed

@UI
@Unreleased
	Scenario: Get rest of results when query has more than 100
	Given I am on the Lesson Page
	And I have entered a query of "SELECT TOP 101 * FROM dbo.Clients"
	And I Press Execute
	And 100 query results are displayed with a link to read more
	When I click the read more link
	Then 101 result rows are displayed

