import { Page, Locator, expect } from '@playwright/test';

/**
 * Base page object class providing common functionality for all page objects
 */
export class BasePage {
  protected page: Page;
  protected baseUrl: string;

  constructor(page: Page, baseUrl: string = '') {
    this.page = page;
    this.baseUrl = baseUrl;
  }

  /**
   * Navigate to a specific URL
   */
  async navigate(url: string): Promise<void> {
    const fullUrl = url.startsWith('http') ? url : `${this.baseUrl}${url}`;
    await this.page.goto(fullUrl);
  }

  /**
   * Wait for an element to be present and visible
   */
  async waitForElement(selector: string, timeout: number = 5000): Promise<Locator> {
    const locator = this.page.locator(selector);
    await locator.waitFor({ state: 'visible', timeout });
    return locator;
  }

  /**
   * Wait for page to load completely
   */
  async waitForPageLoad(): Promise<void> {
    await this.page.waitForLoadState('networkidle');
  }

  /**
   * Wait for a condition to be true with timeout
   */
  async waitForCondition(condition: () => Promise<boolean>, description: string, timeout: number = 5000): Promise<void> {
    const startTime = Date.now();
    while (Date.now() - startTime < timeout) {
      if (await condition()) {
        return;
      }
      await this.page.waitForTimeout(100);
    }
    throw new Error(`Timed out while waiting for: ${description}`);
  }

  /**
   * Check if an element is present and visible
   */
  async isElementPresent(selector: string): Promise<boolean> {
    try {
      const locator = this.page.locator(selector);
      await locator.waitFor({ state: 'visible', timeout: 1000 });
      return true;
    } catch {
      return false;
    }
  }

  /**
   * Assert that an element is present and visible
   */
  async assertElementPresent(selector: string, description: string): Promise<void> {
    const isPresent = await this.isElementPresent(selector);
    expect(isPresent, `Element '${description}' should be present`).toBe(true);
  }

  /**
   * Assert that text matches expected value
   */
  async assertElementText(selector: string, expectedText: string, description: string): Promise<void> {
    const locator = this.page.locator(selector);
    const actualText = await locator.textContent();
    expect(actualText?.trim(), `Text for '${description}' should match`).toBe(expectedText);
  }

  /**
   * Assert that count matches expected value
   */
  async assertElementCount(selector: string, expectedCount: number, description: string): Promise<void> {
    const locator = this.page.locator(selector);
    const count = await locator.count();
    expect(count, `Count for '${description}' should match`).toBe(expectedCount);
  }

  /**
   * Get the current page title
   */
  async getPageTitle(): Promise<string> {
    return await this.page.title();
  }

  /**
   * Assert that page title matches expected value
   */
  async assertPageTitle(expectedTitle: string): Promise<void> {
    const actualTitle = await this.getPageTitle();
    expect(actualTitle, 'Page title should match').toBe(expectedTitle);
  }

  /**
   * Wait for navigation to complete
   */
  async waitForNavigation(): Promise<void> {
    await this.page.waitForURL('**');
  }

  /**
   * Get current URL
   */
  async getCurrentUrl(): Promise<string> {
    return this.page.url();
  }
} 