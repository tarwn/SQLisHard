import { test } from '@playwright/test';
import { ExercisePage } from './helpers/page-objects/exercise-page';

test.describe('Basic Query Execution', () => {
  test('should execute basic successful query', async ({ page }) => {
    const exercisePage = new ExercisePage(page);
    
    // Given I am on the Exercise Page
    await exercisePage.navigateToExercisePage();
    
    // And I have entered a query of "SELECT TOP 10 * FROM dbo.Customers"
    await exercisePage.enterQuery('SELECT TOP 10 * FROM dbo.Customers');
    
    // When I Press Execute
    await exercisePage.executeQuery();
    
    // Then the query results are displayed
    await exercisePage.waitForResults();
    await exercisePage.expectResultsDisplayed();
  });
}); 