/**
 * Test utilities for common operations and helpers
 */
import { expect } from '@playwright/test';
import { ExercisePage } from './page-objects/exercise-page';

export class TestUtils {


  /**
   * Setup test database (placeholder for future implementation)
   */
  static async setupTestDatabase(): Promise<void> {
    // TODO: Implement database setup logic
    console.log('Setting up test database...');
  }

  /**
   * Cleanup test data (placeholder for future implementation)
   */
  static async cleanupTestData(): Promise<void> {
    // TODO: Implement test data cleanup logic
    console.log('Cleaning up test data...');
  }

  /**
   * Generate unique test identifier
   */
  static generateTestId(): string {
    return `test_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  }

  /**
   * Validate URL format
   */
  static isValidUrl(url: string): boolean {
    try {
      new URL(url);
      return true;
    } catch {
      return false;
    }
  }

  /**
   * Sanitize string for safe use in selectors
   */
  static sanitizeSelector(str: string): string {
    return str.replace(/[^a-zA-Z0-9-_]/g, '_');
  }

  /**
   * Verify exercise state matches expected
   */
  static async verifyExerciseState(exercisePage: ExercisePage, expectedId: string): Promise<void> {
    const currentId = await exercisePage.getCurrentExerciseId();
    expect(currentId).toBe(expectedId);
  }

  /**
   * Verify button visibility state
   */
  static async verifyButtonVisibility(button: import('@playwright/test').Locator, shouldBeVisible: boolean): Promise<void> {
    if (shouldBeVisible) {
      await expect(button).toBeVisible();
    } else {
      await expect(button).toBeHidden();
    }
  }

  /**
   * Verify navigation to next exercise
   */
  static async verifyNavigationToNextExercise(exercisePage: ExercisePage, previousId: string): Promise<void> {
    const currentId = await exercisePage.getCurrentExerciseId();
    expect(currentId).not.toBe(previousId);
  }

  /**
   * Track exercise progression (returns array of visited exercise IDs)
   */
  static async trackExerciseProgression(exercisePage: ExercisePage, exerciseIds: string[]): Promise<string[]> {
    const visited: string[] = [];
    for (const id of exerciseIds) {
      await exercisePage.selectExercise(id);
      const currentId = await exercisePage.getCurrentExerciseId();
      visited.push(currentId);
    }
    return visited;
  }

} 