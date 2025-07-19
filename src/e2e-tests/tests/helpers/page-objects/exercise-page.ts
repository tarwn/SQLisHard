import { Page, expect } from '@playwright/test';
import { BasePage } from './base-page';

/**
 * Exercise page object for SQL exercise interactions
 * Migrated from C# LessonPage.cs functionality
 */
export class ExercisePage extends BasePage {
  // Page elements
  private queryInput = this.page.locator('#queryInput');
  private queryExecutionButton = this.page.locator('#queryExecutionButton');
  private queryError = this.page.locator('#queryError');
  private tipDescription = this.page.locator('#tipDescription');
  private queryResults = this.page.locator('#queryResults');
  private queryStatus = this.page.locator('#queryStatus');
  private dataTable = this.page.locator('#dataTable');
  private moreResultsLink = this.page.locator('#moreResultsLink');
  private moreResultsLinkTotalCount = this.page.locator('#moreResultsLinkTotalCount');
  private exerciseTitle = this.page.locator('#exerciseTitle');
  private continueButton = this.page.locator('#continueButton');

  // Selectors for dynamic elements
  private resultRowsSelector = '#queryResults tbody tr';
  private exerciseListItemsSelector = '#exerciseList ul li';

  constructor(page: Page) {
    super(page);
  }

  // Page properties
  get defaultTitle(): string {
    return 'SQL Is Hard - Exercise';
  }

  get pageUrl(): string {
    return '/Exercise';
  }

  /**
   * Navigate to the exercise page
   */
  async navigateToExercisePage(): Promise<void> {
    await this.navigate(this.pageUrl);
    await this.waitForPageLoad();
    await this.assertPageTitle(this.defaultTitle);
  }

  /**
   * Check if query execution is not in progress
   */
  async isNotExecuting(): Promise<boolean> {
    return await this.queryExecutionButton.isEnabled();
  }

  /**
   * Check if results are available and successful
   */
  async resultsAreAvailableAndSuccessful(): Promise<boolean> {
    const resultsPresent = await this.isElementPresent('#queryResults');
    const dataTablePresent = await this.isElementPresent('#dataTable');
    return resultsPresent && dataTablePresent;
  }

  /**
   * Check if query error is displayed
   */
  async queryErrorIsDisplayed(): Promise<boolean> {
    return await this.isElementPresent('#queryError');
  }

  /**
   * Check if tip tab is visible
   */
  async tipTabIsVisible(): Promise<boolean> {
    return await this.isElementPresent('#tipDescription');
  }

  /**
   * Check if more results link is present
   */
  async moreResultsLinkIsPresent(): Promise<boolean> {
    return await this.isElementPresent('#moreResultsLink');
  }

  /**
   * Enter query text into the input field
   */
  async enterQuery(queryText: string): Promise<void> {
    await this.queryInput.clear();
    await this.queryInput.fill(queryText);
  }

  /**
   * Execute the current query
   */
  async executeQuery(): Promise<void> {
    await this.queryExecutionButton.click();
  }

  /**
   * Enter query and execute it, then wait for results
   */
  async executeQueryAndWaitForResults(queryText: string): Promise<void> {
    await this.enterQuery(queryText);
    await this.executeQuery();
    await this.waitForCondition(
      () => this.resultsAreAvailableAndSuccessful(),
      'Results to be displayed',
      5000
    );
  }

  /**
   * Assert that more results link reports total count
   */
  async assertMoreResultsLinkReportsTotalOf(expectedResultCount: number): Promise<void> {
    const actualText = await this.moreResultsLinkTotalCount.textContent();
    expect(actualText?.trim(), 'Total count in the More Results link').toBe(expectedResultCount.toString());
  }

  /**
   * Assert that more results link reports total greater than expected
   */
  async assertMoreResultsLinkReportsTotalGreaterThan(expectedResultCount: number): Promise<void> {
    const actualText = await this.moreResultsLinkTotalCount.textContent();
    const actualCount = parseInt(actualText || '0', 10);
    expect(actualCount, 'Total count should be greater than expected').toBeGreaterThan(expectedResultCount);
  }

  /**
   * Assert number of result rows matches expected count
   */
  async assertNumberOfResultsRowsIs(expectedResultCount: number): Promise<void> {
    const rows = this.page.locator(this.resultRowsSelector);
    const count = await rows.count();
    expect(count, 'Expected rows count should match actual count').toBe(expectedResultCount);
  }

  /**
   * Assert that status displays expected text
   */
  async assertStatusDisplays(expectedStatus: string): Promise<void> {
    const actualStatus = await this.queryStatus.textContent();
    expect(actualStatus?.trim().toLowerCase(), 'Query status should match expected value')
      .toBe(expectedStatus.toLowerCase());
  }

  /**
   * Assert that the nth exercise is selected
   */
  async assertNthExerciseIsSelected(indexOfSelectedEntry: number): Promise<void> {
    const exerciseList = this.page.locator(this.exerciseListItemsSelector);
    const count = await exerciseList.count();
    
    expect(count, `List should have at least ${indexOfSelectedEntry} items`).toBeGreaterThanOrEqual(indexOfSelectedEntry);
    
    const selectedIndex = indexOfSelectedEntry - 1;
    const selectedItem = exerciseList.nth(selectedIndex);
    const className = await selectedItem.getAttribute('class');
    
    expect(className, `The indexed item should be selected`).toBe('selected');
  }

  /**
   * Check if the nth exercise is selected
   */
  async isNthExerciseIsSelected(indexOfSelectedEntry: number): Promise<boolean> {
    const exerciseList = this.page.locator(this.exerciseListItemsSelector);
    const count = await exerciseList.count();
    
    if (count < indexOfSelectedEntry) {
      return false;
    }
    
    const selectedIndex = indexOfSelectedEntry - 1;
    const selectedItem = exerciseList.nth(selectedIndex);
    const className = await selectedItem.getAttribute('class');
    
    return className === 'selected';
  }

  /**
   * Check if exercises have loaded
   */
  async exercisesHaveLoaded(): Promise<boolean> {
    const exerciseList = this.page.locator(this.exerciseListItemsSelector);
    const count = await exerciseList.count();
    return count > 1; // starts with a 'Loading...' item
  }

  /**
   * Select an exercise by ID
   */
  async selectExercise(exerciseId: string): Promise<void> {
    const exerciseLink = this.page.locator(`li[data-exerciseid='${exerciseId}']`);
    await this.assertElementPresent(`li[data-exerciseid='${exerciseId}']`, `Exercise Link for '${exerciseId}'`);
    await exerciseLink.click();
  }

  /**
   * Check if current exercise matches the given ID
   */
  async isCurrentExercise(exerciseId: string): Promise<boolean> {
    await this.assertElementPresent('#exerciseTitle', 'Exercise Title');
    const dataExerciseId = await this.exerciseTitle.getAttribute('data-exerciseid');
    return dataExerciseId === exerciseId;
  }

  /**
   * Assert that continue button is visible
   */
  async assertCompleteButtonVisible(): Promise<void> {
    await this.assertElementPresent('#continueButton', 'ContinueButton');
  }

  /**
   * Wait for results to be displayed
   */
  async waitForResults(): Promise<void> {
    await this.waitForCondition(
      () => this.resultsAreAvailableAndSuccessful(),
      'Query results to be displayed',
      10000
    );
  }

  /**
   * Wait for query execution to complete
   */
  async waitForQueryExecution(): Promise<void> {
    await this.waitForCondition(
      () => this.isNotExecuting(),
      'Query execution to complete',
      10000
    );
  }

  /**
   * Get the current exercise title
   */
  async getExerciseTitle(): Promise<string> {
    const title = await this.exerciseTitle.textContent();
    return title?.trim() || '';
  }

  /**
   * Get the current exercise ID
   */
  async getCurrentExerciseId(): Promise<string> {
    const exerciseId = await this.exerciseTitle.getAttribute('data-exerciseid');
    return exerciseId || '';
  }

  /**
   * Get the query status text
   */
  async getQueryStatus(): Promise<string> {
    const status = await this.queryStatus.textContent();
    return status?.trim() || '';
  }

  /**
   * Get the number of result rows
   */
  async getResultRowCount(): Promise<number> {
    const rows = this.page.locator(this.resultRowsSelector);
    return await rows.count();
  }

  /**
   * Get the total count from more results link
   */
  async getMoreResultsTotalCount(): Promise<number> {
    const text = await this.moreResultsLinkTotalCount.textContent();
    return parseInt(text || '0', 10);
  }
} 