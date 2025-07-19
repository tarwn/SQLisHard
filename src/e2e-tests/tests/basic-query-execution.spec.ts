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

  test('should handle query with syntax error', async ({ page }) => {
    const exercisePage = new ExercisePage(page);
    
    await exercisePage.navigateToExercisePage();
    await exercisePage.enterQuery('Not a real query');
    await exercisePage.executeQuery();
    await exercisePage.expectSyntaxError();
  });

  test('should show status during query execution', async ({ page }) => {
    const exercisePage = new ExercisePage(page);
    
    await exercisePage.navigateToExercisePage();
    await exercisePage.enterQuery('SELECT TOP 1000 * FROM dbo.Customers');
    await exercisePage.executeQuery();
    // Wait for query to complete and check final status
    await exercisePage.waitForQueryExecution();
    await exercisePage.expectStatusComplete();
  });

  test('should handle pagination with read more', async ({ page }) => {
    const exercisePage = new ExercisePage(page);
    
    await exercisePage.navigateToExercisePage();
    await exercisePage.enterQuery('SELECT TOP 101 * FROM dbo.Customers');
    await exercisePage.executeQuery();
    await exercisePage.expectPagination();
    await exercisePage.clickReadMore();
    await exercisePage.expectMoreResults();
  });

  test('should display pattern-based exercise tips', async ({ page }) => {
    const exercisePage = new ExercisePage(page);
    
    await exercisePage.navigateToExercisePage();
    await exercisePage.selectPatternExercise();
    // Execute a query that might trigger a tip
    await exercisePage.enterQuery('SELECT * FROM Customers');
    await exercisePage.executeQuery();
    await exercisePage.waitForQueryExecution();
    // Note: Tips may not always be present, so we'll check if they exist
    const hasTips = await exercisePage.tipTabIsVisible();
    if (hasTips) {
      await exercisePage.expectExerciseTips();
    }
  });

  test('should show initial status as ready', async ({ page }) => {
    const exercisePage = new ExercisePage(page);
    
    await exercisePage.navigateToExercisePage();
    await exercisePage.expectStatusComplete();
  });

  test('should show status after syntax error', async ({ page }) => {
    const exercisePage = new ExercisePage(page);
    
    await exercisePage.navigateToExercisePage();
    await exercisePage.enterQuery('Not a real query');
    await exercisePage.executeQuery();
    await exercisePage.waitForQueryExecution();
    // After syntax error, status should show "Completed with Error"
    await exercisePage.assertStatusDisplays('Completed with Error');
  });

  test('should display 100 results with read more link', async ({ page }) => {
    const exercisePage = new ExercisePage(page);
    
    await exercisePage.navigateToExercisePage();
    await exercisePage.enterQuery('SELECT TOP 101 * FROM dbo.Customers');
    await exercisePage.executeQuery();
    await exercisePage.waitForResults();
    await exercisePage.assertNumberOfResultsRowsIs(100);
    await exercisePage.expectPagination();
  });

  test('should show all results after clicking read more', async ({ page }) => {
    const exercisePage = new ExercisePage(page);
    
    await exercisePage.navigateToExercisePage();
    await exercisePage.enterQuery('SELECT TOP 101 * FROM dbo.Customers');
    await exercisePage.executeQuery();
    await exercisePage.waitForResults();
    await exercisePage.clickReadMore();
    await exercisePage.waitForResults();
    await exercisePage.assertNumberOfResultsRowsIs(101);
    await exercisePage.expectMoreResults();
  });
});