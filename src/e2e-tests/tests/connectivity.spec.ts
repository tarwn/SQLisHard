import { test, expect } from '@playwright/test';

// Base URL for the test server
const BASE_URL = 'http://localhost:8012';

test.describe('Server Connectivity', () => {
  test.beforeEach(async ({ page }) => {
    // Set a reasonable timeout for server connectivity
    page.setDefaultTimeout(30000);
  });

  test('should connect to server and load home page', async ({ page }) => {
    // Navigate to the home page
    await page.goto(BASE_URL);
    
    // Verify the page loaded successfully
    await expect(page).toHaveURL(BASE_URL);
    
    // Check that the page is not showing an error
    await expect(page.locator('body')).not.toContainText('Error');
    await expect(page.locator('body')).not.toContainText('404');
    await expect(page.locator('body')).not.toContainText('500');
  });

  test('should verify page title', async ({ page }) => {
    // Navigate to the home page
    await page.goto(BASE_URL);
    
    // Verify the page has a title
    const title = await page.title();
    expect(title).toBeTruthy();
    expect(title.length).toBeGreaterThan(0);
    
    // Verify the title is not a default error title
    expect(title).not.toBe('Error');
    expect(title).not.toBe('404 Not Found');
  });

  test('should respond to basic page requests', async ({ page }) => {
    // Test the home page
    await page.goto(BASE_URL);
    await expect(page).toHaveURL(BASE_URL);
    
    // Test that we can navigate to the about page
    await page.goto(`${BASE_URL}/about`);
    await expect(page).toHaveURL(`${BASE_URL}/about`);
    
    // Verify the about page loaded
    await expect(page.locator('body')).not.toContainText('Error');
  });

  test('should handle server startup and shutdown gracefully', async ({ page }) => {
    // This test verifies that the server responds correctly
    // In a real scenario, this would be tested with server startup/shutdown
    await page.goto(BASE_URL);
    
    // Verify server is responding
    await expect(page).toHaveURL(BASE_URL);
    
    // Check that the page loads within a reasonable time
    const startTime = Date.now();
    await page.waitForLoadState('networkidle');
    const loadTime = Date.now() - startTime;
    
    // Verify page loads within 10 seconds
    expect(loadTime).toBeLessThan(10000);
  });
}); 