# E2E Tests with Playwright

This project uses Playwright with TypeScript for end-to-end testing. The test suite provides comprehensive coverage of the SQLisHard application functionality.

## Quick Start

```bash
# Install dependencies
npm install
npx playwright install chrome

# Start the test server (in another terminal)
cd ../backend/SQLisHard
dotnet run --urls=http://localhost:8012 --environment=Test

# Run tests
npm run test
```

## Test Commands

- `npm run test` — Run all tests
- `npm run test:headed` — Run tests with browser visible
- `npm run test:debug` — Run tests in debug mode
- `npm run test:connectivity` — Run connectivity test only
- `npm run lint` — Lint test code

## Configuration

The base URL for tests can be configured using the `BASE_URL` environment variable:

```bash
# Use default localhost:8012
npm run test

# Use custom base URL (Unix/Linux/macOS)
BASE_URL=http://localhost:3000 npm run test

# Use production URL (Unix/Linux/macOS)
BASE_URL=https://your-app.com npm run test
```

```powershell
# Use custom base URL (Windows PowerShell)
$env:BASE_URL="http://localhost:3000"; npm run test

# Use production URL (Windows PowerShell)
$env:BASE_URL="https://your-app.com"; npm run test
```

## Project Structure

```
src/e2e-tests/
├── playwright.config.ts    # Playwright configuration (Chrome-only)
├── tsconfig.json          # TypeScript configuration
├── package.json           # Dependencies and scripts
├── tests/                 # Test specifications
│   ├── connectivity.spec.ts           # Basic connectivity tests
│   ├── basic-query-execution.spec.ts  # Query execution tests
│   └── helpers/           # Test utilities and page objects
│       ├── test-utils.ts              # Common test utilities
│       └── page-objects/              # Page object models
│           ├── base-page.ts           # Base page object
│           └── exercise-page.ts       # Exercise page object
```

## Writing Tests

### Test Structure

Tests are written using the Page Object Model pattern for maintainability and reusability:

```typescript
import { test, expect } from '@playwright/test';
import { ExercisePage } from './helpers/page-objects/exercise-page';

test('should execute query successfully', async ({ page }) => {
  const exercisePage = new ExercisePage(page);
  await exercisePage.navigate();
  await exercisePage.enterQuery('SELECT * FROM Customers');
  await exercisePage.executeQuery();
  await exercisePage.expectResultsDisplayed();
});
```

### Page Object Model

Page objects encapsulate page-specific logic and provide a clean API for test interactions:

```typescript
// Example page object method
export class ExercisePage extends BasePage {
  async enterQuery(query: string): Promise<void> {
    await this.page.fill('#query-input', query);
  }

  async executeQuery(): Promise<void> {
    await this.page.click('#execute-button');
  }

  async expectResultsDisplayed(): Promise<void> {
    await expect(this.page.locator('.results-table')).toBeVisible();
  }
}
```

### Test Utilities

Common test utilities are available in `tests/helpers/test-utils.ts`:

```typescript
import { waitForPageLoad } from './helpers/test-utils';

test('should handle page load', async ({ page }) => {
  await page.goto('/exercise');
  await waitForPageLoad(page);
  // ... test logic
});
```

## Troubleshooting Guide

### Common Issues

#### Tests Fail Intermittently
- **Cause**: Race conditions or timing issues
- **Solution**: Add explicit waits, use `waitFor` methods, or increase timeouts
- **Prevention**: Use page object methods that handle waiting internally

#### Server Connection Issues
- **Cause**: Test server not running or wrong port
- **Solution**: Ensure test server is running on port 8012 with `--environment=Test`
- **Verification**: Run `npm run test:connectivity` to test basic connectivity

#### Database Issues
- **Cause**: Test database not properly configured
- **Solution**: Verify test database setup and connection strings
- **Check**: Ensure test environment uses separate test databases

#### Browser Issues
- **Cause**: Chrome not installed or outdated
- **Solution**: Run `npx playwright install chrome` to install/update Chrome
- **Alternative**: Use `npm run test:headed` to see browser behavior

### Debugging Techniques

#### Debug Mode
```bash
npm run test:debug
```
This opens the browser in headed mode and pauses execution for manual inspection.

#### Screenshots and Videos
- Screenshots are automatically captured on test failure
- Videos are retained on failure for analysis
- Check `test-results/` directory for artifacts

#### Console Logs
```typescript
// Add logging to tests
test('debug test', async ({ page }) => {
  console.log('Navigating to page...');
  await page.goto('/exercise');
  console.log('Page loaded');
});
```

### Performance Optimization

#### Parallel Execution
Tests run in parallel by default. To run sequentially:
```bash
npx playwright test --workers=1
```

#### Test Isolation
- Each test runs in a fresh browser context
- Database state is reset between tests
- No shared state between test runs

## Development Setup

### Prerequisites
- Node.js 18+
- .NET 6+
- SQL Server 2019+
- Chrome browser

### Installation Steps

1. **Install Node.js dependencies**
   ```bash
   cd src/e2e-tests
   npm install
   ```

2. **Install Playwright browsers**
   ```bash
   npx playwright install chrome
   ```

3. **Configure test environment**
   ```bash
   # Set up test databases (see main README for details)
   # Configure connection strings for test environment
   ```

4. **Start test server**
   ```bash
   cd ../backend/SQLisHard
   dotnet run --urls=http://localhost:8012 --environment=Test
   ```

### Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `BASE_URL` | `http://localhost:8012` | Base URL for test server |
| `NODE_ENV` | `test` | Node environment |

### CI/CD Integration

The test suite is designed to run in CI/CD environments:

```yaml
# Example GitHub Actions workflow
- name: Run E2E Tests
  run: |
    cd src/e2e-tests
    npm install
    npx playwright install chrome
    npm run test
```

## Best Practices

### Test Writing
- Use descriptive test names that explain the scenario
- Keep tests focused on a single feature or user journey
- Use page objects to encapsulate page interactions
- Add appropriate assertions to verify expected behavior

### Maintenance
- Update page objects when UI changes
- Keep test data consistent and isolated
- Review and update tests when features change
- Use TypeScript for better type safety and IDE support

### Performance
- Minimize test execution time
- Use efficient selectors (prefer data-testid attributes)
- Avoid unnecessary waits and timeouts
- Run tests in parallel when possible

## Contributing

When adding new tests:
1. Follow the existing page object pattern
2. Add appropriate TypeScript types
3. Include error handling and edge cases
4. Update this documentation if needed
5. Ensure tests pass consistently

## Support

For issues with the test suite:
1. Check the troubleshooting guide above
2. Review test logs and artifacts
3. Run tests in debug mode for investigation
4. Check the main project README for setup issues 