# Troubleshooting Guide

This guide helps you resolve common issues with the SQLisHard E2E test suite.

## Table of Contents

1. [Common Test Failures](#common-test-failures)
2. [Debugging Techniques](#debugging-techniques)
3. [Environment Setup Issues](#environment-setup-issues)
4. [Performance Optimization](#performance-optimization)
5. [CI/CD Troubleshooting](#cicd-troubleshooting)

## Common Test Failures

### Intermittent Test Failures

#### Symptoms
- Tests pass sometimes and fail other times
- Timeout errors
- Element not found errors
- Race condition errors

#### Causes
- Network latency
- Database response time variations
- Browser rendering delays
- Resource contention

#### Solutions

1. **Add Explicit Waits**
   ```typescript
   // Instead of immediate assertion
   await expect(page.locator('.results')).toBeVisible();
   
   // Wait for element to be ready
   await page.waitForSelector('.results', { state: 'visible' });
   await expect(page.locator('.results')).toBeVisible();
   ```

2. **Use Retry Logic**
   ```typescript
   import { retryOperation } from './helpers/test-utils';
   
   await retryOperation(async () => {
     await page.click('#execute-button');
     await expect(page.locator('.results')).toBeVisible();
   });
   ```

3. **Increase Timeouts**
   ```typescript
   // In playwright.config.ts
   export default defineConfig({
     use: {
       actionTimeout: 10000,
       navigationTimeout: 30000,
     },
   });
   ```

### Element Not Found Errors

#### Symptoms
- `Element not found` errors
- `Timeout waiting for selector` errors
- Tests fail on specific selectors

#### Causes
- UI changes breaking selectors
- Dynamic content loading
- Incorrect selector strategy

#### Solutions

1. **Use Stable Selectors**
   ```typescript
   // Avoid text-based selectors
   await page.click('text=Execute Query');
   
   // Use data-testid attributes
   await page.click('[data-testid="execute-button"]');
   ```

2. **Wait for Dynamic Content**
   ```typescript
   // Wait for content to load
   await page.waitForSelector('.results-table', { state: 'visible' });
   await page.waitForLoadState('networkidle');
   ```

3. **Verify Selector Strategy**
   ```typescript
   // Check if element exists before interaction
   const element = page.locator('#query-input');
   await expect(element).toBeVisible();
   await element.fill('SELECT * FROM Customers');
   ```

### Database Connection Issues

#### Symptoms
- Database connection errors
- Query execution failures
- Test data not available

#### Causes
- Test database not running
- Incorrect connection strings
- Database permissions issues

#### Solutions

1. **Verify Database Setup**
   ```bash
   # Check if SQL Server is running
   sqlcmd -S localhost -Q "SELECT @@VERSION"
   
   # Verify test database exists
   sqlcmd -S localhost -d TestCoreDB -Q "SELECT COUNT(*) FROM Users"
   ```

2. **Check Connection Strings**
   ```bash
   # Verify user secrets are set correctly
   cd src/backend/SQLisHard
   dotnet user-secrets list
   ```

3. **Test Database Connectivity**
   ```bash
   # Test connection from backend
   cd src/backend/SQLisHard
   dotnet run --urls=http://localhost:8012 --environment=Test
   ```

## Debugging Techniques

### Debug Mode

Run tests in debug mode to step through execution:

```bash
npm run test:debug
```

This will:
- Open browser in headed mode
- Pause execution at breakpoints
- Allow manual inspection of page state

### Screenshots and Videos

Screenshots and videos are automatically captured on failure:

```bash
# View test artifacts
ls -la test-results/
ls -la playwright-report/
```

### Console Logging

Add logging to understand test flow:

```typescript
test('debug test execution', async ({ page }) => {
  console.log('Starting test...');
  
  await page.goto('/exercise');
  console.log('Navigated to exercise page');
  
  await page.fill('#query-input', 'SELECT * FROM Customers');
  console.log('Entered query');
  
  await page.click('#execute-button');
  console.log('Clicked execute button');
  
  // Wait for results
  await page.waitForSelector('.results-table');
  console.log('Results displayed');
});
```

### Browser Developer Tools

Use browser developer tools during debug mode:

```typescript
test('inspect page state', async ({ page }) => {
  await page.goto('/exercise');
  
  // Open developer tools
  await page.evaluate(() => {
    debugger;
  });
  
  // Continue with test...
});
```

### Network Monitoring

Monitor network requests to debug API issues:

```typescript
test('monitor API calls', async ({ page }) => {
  // Listen for network requests
  page.on('request', request => {
    console.log('Request:', request.method(), request.url());
  });
  
  page.on('response', response => {
    console.log('Response:', response.status(), response.url());
  });
  
  await page.goto('/exercise');
  // ... test logic
});
```

## Environment Setup Issues

### Node.js Issues

#### Symptoms
- `npm install` fails
- Playwright installation errors
- Version compatibility issues

#### Solutions

1. **Verify Node.js Version**
   ```bash
   node --version  # Should be 18+
   npm --version
   ```

2. **Clear npm Cache**
   ```bash
   npm cache clean --force
   rm -rf node_modules package-lock.json
   npm install
   ```

3. **Install Playwright Browsers**
   ```bash
   npx playwright install chrome
   ```

### .NET Issues

#### Symptoms
- Backend won't start
- Database migration errors
- Configuration issues

#### Solutions

1. **Verify .NET Version**
   ```bash
   dotnet --version  # Should be 6+
   ```

2. **Restore Dependencies**
   ```bash
   cd src/backend/SQLisHard
   dotnet restore
   dotnet build
   ```

3. **Check User Secrets**
   ```bash
   dotnet user-secrets list
   ```

### Database Issues

#### Symptoms
- Connection string errors
- Migration failures
- Permission denied errors

#### Solutions

1. **Verify SQL Server**
   ```bash
   # Check if SQL Server is running
   sqlcmd -S localhost -Q "SELECT @@VERSION"
   ```

2. **Check Database Permissions**
   ```sql
   -- Verify user has necessary permissions
   SELECT IS_MEMBER('db_owner') as IsOwner;
   SELECT IS_MEMBER('db_datareader') as CanRead;
   SELECT IS_MEMBER('db_datawriter') as CanWrite;
   ```

3. **Reset Test Databases**
   ```bash
   # Run database reset scripts
   cd database/coredb
   sqlcmd -S localhost -i reset.sql
   
   cd ../exercisedb
   sqlcmd -S localhost -i reset.sql
   ```

## Performance Optimization

### Test Execution Speed

#### Optimize Test Structure

1. **Parallel Execution**
   ```typescript
   // playwright.config.ts
   export default defineConfig({
     workers: 4,  // Run 4 tests in parallel
   });
   ```

2. **Reduce Setup Time**
   ```typescript
   // Use beforeEach instead of beforeAll when possible
   test.beforeEach(async ({ page }) => {
     // Minimal setup per test
   });
   ```

3. **Optimize Selectors**
   ```typescript
   // Use efficient selectors
   await page.locator('[data-testid="button"]').click();  // Fast
   await page.locator('div > div > div > button').click(); // Slow
   ```

#### Browser Optimization

1. **Headless Mode**
   ```typescript
   // playwright.config.ts
   export default defineConfig({
     use: {
       headless: true,  // Faster execution
     },
   });
   ```

2. **Disable Unnecessary Features**
   ```typescript
   // playwright.config.ts
   export default defineConfig({
     use: {
       // Disable features not needed for tests
       javaScriptEnabled: true,
       bypassCSP: false,
     },
   });
   ```

### Resource Management

#### Memory Usage

1. **Clean Up Resources**
   ```typescript
   test.afterEach(async ({ page }) => {
     // Clear browser storage
     await page.context().clearCookies();
     await page.evaluate(() => localStorage.clear());
   });
   ```

2. **Limit Concurrent Tests**
   ```typescript
   // playwright.config.ts
   export default defineConfig({
     workers: 2,  // Reduce if memory is limited
   });
   ```

#### Network Optimization

1. **Mock External Services**
   ```typescript
   // Mock external API calls
   await page.route('**/api/external', route => {
     route.fulfill({ status: 200, body: '{"data": "mocked"}' });
   });
   ```

2. **Disable Images and CSS**
   ```typescript
   // playwright.config.ts
   export default defineConfig({
     use: {
       // Block unnecessary resources
       extraHTTPHeaders: {
         'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8',
       },
     },
   });
   ```

## CI/CD Troubleshooting

### GitHub Actions Issues

#### Common Problems

1. **Browser Installation**
   ```yaml
   # Ensure browsers are installed
   - name: Install Playwright Browsers
     run: npx playwright install chrome
   ```

2. **Database Setup**
   ```yaml
   # Set up test database
   - name: Setup Test Database
     run: |
       # Add database setup commands
   ```

3. **Environment Variables**
   ```yaml
   # Set required environment variables
   env:
     BASE_URL: http://localhost:8012
     NODE_ENV: test
   ```

#### Debug CI/CD Issues

1. **Enable Debug Logging**
   ```yaml
   - name: Run Tests with Debug
     run: |
       DEBUG=pw:api npm run test
   ```

2. **Upload Test Artifacts**
   ```yaml
   - name: Upload Test Results
     uses: actions/upload-artifact@v2
     if: failure()
     with:
       name: playwright-report
       path: playwright-report/
   ```

### Docker Issues

#### Container Setup

1. **Dockerfile for Tests**
   ```dockerfile
   FROM mcr.microsoft.com/playwright:v1.54.1
   
   WORKDIR /app
   COPY package*.json ./
   RUN npm install
   
   COPY . .
   RUN npx playwright install chrome
   
   CMD ["npm", "run", "test"]
   ```

2. **Docker Compose for Full Stack**
   ```yaml
   version: '3.8'
   services:
     sqlserver:
       image: mcr.microsoft.com/mssql/server:2019-latest
       environment:
         SA_PASSWORD: YourStrong@Passw0rd
         ACCEPT_EULA: Y
     
     backend:
       build: ./src/backend
       depends_on:
         - sqlserver
       environment:
         - ASPNETCORE_ENVIRONMENT=Test
     
     tests:
       build: ./src/e2e-tests
       depends_on:
         - backend
   ```

### Performance in CI/CD

#### Optimize CI/CD Performance

1. **Caching**
   ```yaml
   - name: Cache Dependencies
     uses: actions/cache@v2
     with:
       path: |
         node_modules
         ~/.npm
       key: ${{ runner.os }}-node-${{ hashFiles('**/package-lock.json') }}
   ```

2. **Parallel Jobs**
   ```yaml
   strategy:
     matrix:
       test-group: [unit, integration, e2e]
     fail-fast: false
   ```

3. **Test Sharding**
   ```yaml
   - name: Run Tests in Shards
     run: |
       npx playwright test --shard=${{ matrix.shard }}/${{ matrix.shards }}
   ```

## Getting Help

### Before Asking for Help

1. **Check this troubleshooting guide**
2. **Review test logs and artifacts**
3. **Try running tests in debug mode**
4. **Verify environment setup**
5. **Check for recent changes**

### Useful Commands

```bash
# Verify environment
node --version
npm --version
dotnet --version
npx playwright --version

# Check test setup
cd src/e2e-tests
npm run test:connectivity

# Debug specific test
npx playwright test --debug --grep "test name"

# Generate test report
npx playwright show-report
```

### Common Error Messages

| Error | Cause | Solution |
|-------|-------|----------|
| `Element not found` | Selector changed or element not loaded | Update selector or add wait |
| `Timeout waiting for selector` | Page loading slowly | Increase timeout or add explicit wait |
| `Connection refused` | Server not running | Start backend server |
| `Database connection failed` | Database not accessible | Check connection strings and permissions |
| `Playwright browser not found` | Browser not installed | Run `npx playwright install chrome` |

This troubleshooting guide should help you resolve most issues with the SQLisHard E2E test suite. If you encounter issues not covered here, please check the logs and artifacts for more specific error information. 