import { test } from '@playwright/test';
import { ExercisePage } from './helpers/page-objects/exercise-page';
import { testConfig } from './_config';


test.describe('ExerciseInteraction', () => {
  test('should verify exercise selection state', async ({ page }) => {
    const exercisePage = new ExercisePage(page);
    await exercisePage.navigateToExercisePage();
    await exercisePage.selectExercise(testConfig.exercises.alternate.id);
    await exercisePage.expectExerciseSelected(testConfig.exercises.alternate.id);
    await exercisePage.expectContinueButtonHidden();
  });

  test('should show complete button when exercise is completed', async ({ page }) => {
    const exercisePage = new ExercisePage(page);
    await exercisePage.navigateToExercisePage();
    await exercisePage.selectExercise(testConfig.exercises.alternate.id);
    await exercisePage.enterQuery(testConfig.exercises.alternate.solution);
    await exercisePage.executeQuery();
    await exercisePage.expectResultsDisplayed();
    await exercisePage.expectContinueButtonVisible();
  });

  test('should navigate to next exercise when continue is clicked', async ({ page }) => {
    const exercisePage = new ExercisePage(page);
    await exercisePage.navigateToExercisePage();
    await exercisePage.selectExercise(testConfig.exercises.alternate.id);
    await exercisePage.enterQuery(testConfig.exercises.alternate.solution);
    await exercisePage.executeQuery();
    await exercisePage.expectContinueButtonVisible();
    await exercisePage.clickContinueButton();
    await exercisePage.expectNavigationToNextExercise(testConfig.exercises.alternate.id);
  });
}); 