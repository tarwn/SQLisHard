# Test Documentation

This document provides comprehensive guidance for working with the SQLisHard E2E test suite.

## Table of Contents

1. [Test Architecture](#test-architecture)
2. [Writing Tests](#writing-tests)
3. [Page Object Model](#page-object-model)
4. [Test Utilities](#test-utilities)
5. [Configuration Management](#configuration-management)
6. [Best Practices](#best-practices)
7. [Common Patterns](#common-patterns)

## Test Architecture

### Overview

The test suite follows a layered architecture:

```
┌─────────────────────────────────────┐
│           Test Specs                │  ← Business logic and scenarios
├─────────────────────────────────────┤
│         Page Objects                │  ← UI interaction layer
├─────────────────────────────────────┤
│         Test Utilities              │  ← Common helper functions
├─────────────────────────────────────┤
│         Playwright API              │  ← Browser automation
└─────────────────────────────────────┘
```

### Key Components

- **Test Specs**: Business-focused test scenarios
- **Page Objects**: Encapsulated UI interactions
- **Test Utilities**: Reusable helper functions
- **Configuration**: Environment-specific settings

## Writing Tests

### Test Structure

Tests should follow this structure:

```typescript
import { test, expect } from '@playwright/test';
import { ExercisePage } from './helpers/page-objects/exercise-page';

test.describe('Query Execution', () => {
  test('should execute valid SQL query', async ({ page }) => {
    // Arrange
    const exercisePage = new ExercisePage(page);
    await exercisePage.navigate();
    
    // Act
    await exercisePage.enterQuery('SELECT * FROM Customers');
    await exercisePage.executeQuery();
    
    // Assert
    await exercisePage.expectResultsDisplayed();
    await exercisePage.expectRowCount(5);
  });
});
```

### Test Naming Conventions

- Use descriptive names that explain the scenario
- Follow the pattern: `should [expected behavior] when [condition]`
- Group related tests using `test.describe()`

Examples:
```typescript
test('should display error for invalid SQL syntax', async ({ page }) => {
test('should show results when query returns data', async ({ page }) => {
test('should handle empty result set gracefully', async ({ page }) => {
```

### Test Organization

Organize tests by feature or user journey:

```typescript
test.describe('Exercise Workflow', () => {
  test.describe('Query Execution', () => {
    test('should execute basic SELECT query', async ({ page }) => {
    test('should handle complex JOIN queries', async ({ page }) => {
  });
  
  test.describe('Error Handling', () => {
    test('should show syntax error for invalid SQL', async ({ page }) => {
    test('should handle database connection errors', async ({ page }) => {
  });
});
```

## Page Object Model

### Overview

Page objects encapsulate page-specific logic and provide a clean API for test interactions. They follow the Single Responsibility Principle and promote code reuse.

### Base Page Object

All page objects extend the `BasePage` class:

```typescript
import { Page, expect } from '@playwright/test';

export abstract class BasePage {
  constructor(protected page: Page) {}

  async navigate(): Promise<void> {
    await this.page.goto(this.getUrl());
  }

  protected abstract getUrl(): string;

  protected async waitForPageLoad(): Promise<void> {
    await this.page.waitForLoadState('networkidle');
  }

  protected async expectElementVisible(selector: string): Promise<void> {
    await expect(this.page.locator(selector)).toBeVisible();
  }
}
```

### Exercise Page Object

The `ExercisePage` extends `BasePage` and provides exercise-specific functionality:

```typescript
export class ExercisePage extends BasePage {
  // Selectors
  private readonly queryInput = '#query-input';
  private readonly executeButton = '#execute-button';
  private readonly resultsTable = '.results-table';
  private readonly errorMessage = '.error-message';

  protected getUrl(): string {
    return '/exercise';
  }

  async enterQuery(query: string): Promise<void> {
    await this.page.fill(this.queryInput, query);
  }

  async executeQuery(): Promise<void> {
    await this.page.click(this.executeButton);
    await this.waitForPageLoad();
  }

  async expectResultsDisplayed(): Promise<void> {
    await this.expectElementVisible(this.resultsTable);
  }

  async expectErrorDisplayed(): Promise<void> {
    await this.expectElementVisible(this.errorMessage);
  }

  async getRowCount(): Promise<number> {
    const rows = await this.page.locator(`${this.resultsTable} tbody tr`).count();
    return rows;
  }
}
```

### Page Object Best Practices

1. **Encapsulate Selectors**: Keep selectors private and provide public methods
2. **Handle Waiting**: Include appropriate waits in page object methods
3. **Return Values**: Return meaningful data from page object methods
4. **Error Handling**: Include error scenarios in page objects
5. **Type Safety**: Use TypeScript interfaces for return types

## Test Utilities

### Common Utilities

The `test-utils.ts` file provides common helper functions:

```typescript
import { Page } from '@playwright/test';

export async function waitForPageLoad(page: Page): Promise<void> {
  await page.waitForLoadState('networkidle');
}

export async function waitForElement(page: Page, selector: string): Promise<void> {
  await page.waitForSelector(selector, { state: 'visible' });
}

export async function retryOperation<T>(
  operation: () => Promise<T>,
  maxAttempts: number = 3,
  delay: number = 1000
): Promise<T> {
  let lastError: Error;
  
  for (let attempt = 1; attempt <= maxAttempts; attempt++) {
    try {
      return await operation();
    } catch (error) {
      lastError = error as Error;
      if (attempt < maxAttempts) {
        await new Promise(resolve => setTimeout(resolve, delay));
      }
    }
  }
  
  throw lastError!;
}
```

### Custom Assertions

Create custom assertions for common patterns:

```typescript
export async function expectQueryResults(page: Page, expectedRows: number): Promise<void> {
  const actualRows = await page.locator('.results-table tbody tr').count();
  expect(actualRows).toBe(expectedRows);
}

export async function expectErrorVisible(page: Page, expectedMessage?: string): Promise<void> {
  const errorElement = page.locator('.error-message');
  await expect(errorElement).toBeVisible();
  
  if (expectedMessage) {
    await expect(errorElement).toContainText(expectedMessage);
  }
}
```

## Configuration Management

### Environment Variables

The test suite uses environment variables for configuration:

```typescript
// playwright.config.ts
export default defineConfig({
  use: {
    baseURL: process.env.BASE_URL || 'http://localhost:8012',
    timeout: parseInt(process.env.TIMEOUT || '30000'),
  },
});
```

### Test Data Management

Manage test data through configuration:

```typescript
export const TestData = {
  validQueries: {
    simpleSelect: 'SELECT * FROM Customers',
    withWhere: 'SELECT * FROM Customers WHERE Country = "Germany"',
    withJoin: 'SELECT c.CustomerName, o.OrderID FROM Customers c JOIN Orders o ON c.CustomerID = o.CustomerID',
  },
  invalidQueries: {
    syntaxError: 'SELECT * FROM Customers WHERE',
    tableNotFound: 'SELECT * FROM NonExistentTable',
  },
} as const;
```

### Configuration Files

Use separate configuration files for different environments:

```typescript
// config/test.config.ts
export const testConfig = {
  baseUrl: process.env.BASE_URL || 'http://localhost:8012',
  timeout: 30000,
  retries: 2,
  screenshotOnFailure: true,
  videoOnFailure: true,
};
```

## Best Practices

### Test Design

1. **Single Responsibility**: Each test should verify one specific behavior
2. **Independence**: Tests should not depend on each other
3. **Deterministic**: Tests should produce consistent results
4. **Fast**: Keep test execution time minimal
5. **Maintainable**: Use clear, readable code

### Selector Strategy

1. **Prefer data-testid**: Use `data-testid` attributes for test selectors
2. **Avoid text-based selectors**: Text content can change frequently
3. **Use semantic selectors**: Prefer meaningful class names over generic ones
4. **Keep selectors stable**: Avoid selectors that change with styling

### Error Handling

1. **Graceful degradation**: Handle expected errors appropriately
2. **Meaningful assertions**: Provide clear error messages
3. **Retry logic**: Use retry mechanisms for flaky operations
4. **Timeout management**: Set appropriate timeouts for different operations

### Performance

1. **Parallel execution**: Run tests in parallel when possible
2. **Resource cleanup**: Clean up resources after tests
3. **Efficient selectors**: Use efficient CSS selectors
4. **Minimize waits**: Use explicit waits instead of fixed delays

## Common Patterns

### Setup and Teardown

```typescript
test.describe('Exercise Tests', () => {
  let exercisePage: ExercisePage;

  test.beforeEach(async ({ page }) => {
    exercisePage = new ExercisePage(page);
    await exercisePage.navigate();
  });

  test.afterEach(async () => {
    // Clean up test data if needed
  });
});
```

### Data-Driven Tests

```typescript
const testCases = [
  { query: 'SELECT * FROM Customers', expectedRows: 5 },
  { query: 'SELECT * FROM Customers WHERE Country = "Germany"', expectedRows: 2 },
  { query: 'SELECT * FROM Customers WHERE Country = "France"', expectedRows: 1 },
];

testCases.forEach(({ query, expectedRows }) => {
  test(`should return ${expectedRows} rows for query: ${query}`, async ({ page }) => {
    const exercisePage = new ExercisePage(page);
    await exercisePage.navigate();
    await exercisePage.enterQuery(query);
    await exercisePage.executeQuery();
    await exercisePage.expectRowCount(expectedRows);
  });
});
```

### Conditional Tests

```typescript
test('should handle large result sets', async ({ page }) => {
  test.skip(process.env.SKIP_PERFORMANCE_TESTS === 'true', 'Performance tests disabled');
  
  const exercisePage = new ExercisePage(page);
  await exercisePage.navigate();
  await exercisePage.enterQuery('SELECT * FROM LargeTable');
  await exercisePage.executeQuery();
  // ... test logic
});
```

### Custom Fixtures

```typescript
import { test as base } from '@playwright/test';
import { ExercisePage } from './helpers/page-objects/exercise-page';

type TestFixtures = {
  exercisePage: ExercisePage;
};

export const test = base.extend<TestFixtures>({
  exercisePage: async ({ page }, use) => {
    const exercisePage = new ExercisePage(page);
    await exercisePage.navigate();
    await use(exercisePage);
  },
});

export { expect } from '@playwright/test';
```

This documentation provides a comprehensive guide for working with the SQLisHard E2E test suite. Follow these patterns and best practices to maintain high-quality, maintainable tests. 