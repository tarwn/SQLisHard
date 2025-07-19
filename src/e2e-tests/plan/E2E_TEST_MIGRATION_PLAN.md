# E2E Test Migration Plan: .Net/SpecFlow/Selenium → Playwright/TypeScript

## Overview

This document outlines the plan to migrate the existing E2E tests from the current .Net Framework + SpecFlow + Selenium stack to a modern Playwright + TypeScript solution. The migration will be done incrementally to ensure all tests continue to pass throughout the process.

## Current Test Structure

### Existing Tests
- **BasicQueryExecution.feature** (9 scenarios)
  - Execute basic successful query
  - Execute query with syntax error  
  - Status display before/after query execution
  - Pagination with "read more" functionality
  - Pattern-based exercise tips
- **ExerciseInteraction.feature** (3 scenarios)
  - Exercise selection state
  - Complete button visibility
  - Continue button navigation

### Current Technology Stack
- .Net Framework with SpecFlow for Gherkin syntax
- Selenium WebDriver for browser automation
- C# page object model with custom base classes
- XML-based configuration for test settings

## Migration Goals

- Replace Gherkin syntax with plain TypeScript tests
- Use Playwright for modern, reliable browser automation
- Maintain all existing test coverage
- Enable parallel test execution
- Improve test reliability and speed
- Support Chrome browser (with option to expand to others)

## Migration Strategy

### Phase 1: Infrastructure Setup

#### 1.1 Backend Server Configuration
- [x] Create test-specific `appsettings.Test.json` configuration
- [x] Ensure server can start/stop cleanly for test isolation using `dotnet run` with `--urls=http://localhost:8012` and `environment=Test`
- [x] Document the test start command in the top level README.md

#### 1.2 Playwright Project Setup
- [x] Create `/src/e2e-tests/` directory
- [x] Initialize npm project with TypeScript
- [x] Install Playwright dependencies:
  ```bash
  npm init -y
  npm install -D @playwright/test typescript
  npx playwright install chrome
  ```
- [x] Configure `playwright.config.ts` for Chrome-only testing
- [x] Set up TypeScript configuration (`tsconfig.json`)
- [x] Set up eslint 9 with recommended typescript settings
- [x] Create basic project structure:
  ```
  /src/e2e-tests/
  ├── package.json
  ├── playwright.config.ts
  ├── tsconfig.json
  ├── eslint
  ├── tests/
  │   ├── basic-query-execution.spec.ts
  │   ├── exercise-interaction.spec.ts
  │   └── helpers/
  │       ├── page-objects/
  │       │   └── exercise-page.ts
  │       └── test-utils.ts
  └── README.md
  ```

#### 1.3 Initial Test Infrastructure
- [x] Create base page object class (`base-page.ts`)
- [x] Create `exercise-page.ts` page object (migrate from C# `LessonPage.cs`)
- [x] Create test utilities for common operations
- [x] Set up test configuration management

### Phase 2: Basic Test Migration

#### 2.1 Simple Connectivity Test
- [x] Create initial test that verifies server is accessible
- [x] Test basic page load and title verification
- [x] Start the server in test mode
- [x] Verify npm scripts work:
  ```json
  {
    "scripts": {
      "test": "playwright test",
      "test:headed": "playwright test --headed",
      "test:debug": "playwright test --debug"
    }
  }
  ```

#### 2.2 First Test Scenario Migration
- [x] Migrate "Execute basic successful query" scenario
- [x] Implement basic page object methods:
  - Navigate to exercise page
  - Enter query text
  - Click execute button
  - Verify results display
- [x] Ensure test passes consistently

### Phase 3: Core Test Migration

#### 3.1 BasicQueryExecution Feature Migration
- [x] Migrate remaining BasicQueryExecution scenarios:
  - [x] Execute query with syntax error
  - [x] Status display verification
  - [x] Pagination with "read more" functionality
  - [x] Pattern-based exercise tips
- [x] Implement all required page object methods
- [x] Add proper error handling and timeouts
- [x] Ensure all tests pass before proceeding

#### 3.2 ExerciseInteraction Feature Migration
- [ ] Migrate ExerciseInteraction scenarios:
  - [ ] Exercise selection state verification
  - [ ] Complete button visibility
  - [ ] Continue button navigation
- [ ] Extend page object with exercise selection methods
- [ ] Add exercise state verification utilities

### Phase 4: Advanced Features & Optimization

#### 4.2 Parallel Execution
- [ ] Configure Playwright for parallel test execution
- [ ] Ensure test isolation between parallel runs
- [ ] Optimize test execution time

### Phase 5: Cleanup & Documentation

#### 5.1 Legacy Test Removal
- [x] Remove old .Net integration test project
- [x] Update solution file to remove old test references
- [x] Clean up unused dependencies
- [x] Update documentation

#### 5.2 Documentation Updates
- [ ] Update README.md with new test instructions
- [ ] Document test writing guidelines
- [ ] Create troubleshooting guide
- [ ] Update development setup instructions

## Implementation Details

### Page Object Model Migration

The existing C# page object will be migrated to TypeScript:

```typescript
// Current C# structure:
public class ExercisePage : PageBase {
    public IWebElement QueryInput => Driver.FindElement(By.Id("queryInput"));
    public IWebElement QueryExecutionButton => Driver.FindElement(By.Id("queryExecutionButton"));
    // ... methods
}

// New TypeScript structure:
export class ExercisePage extends BasePage {
    private queryInput = this.page.locator('#queryInput');
    private queryExecutionButton = this.page.locator('#queryExecutionButton');
    // ... methods
}
```

### Test Structure Migration

```typescript
// Instead of Gherkin:
// Given I am on the Exercise Page
// And I have entered a query of "SELECT TOP 10 * FROM dbo.Customers"
// When I Press Execute
// Then the query results are displayed

// Use TypeScript:
test('execute basic successful query', async ({ page }) => {
    const exercisePage = new ExercisePage(page);
    await exercisePage.navigate();
    await exercisePage.enterQuery('SELECT TOP 10 * FROM dbo.Customers');
    await exercisePage.executeQuery();
    await exercisePage.expectResultsDisplayed();
});
```

### Configuration Management

Replace XML configuration with TypeScript/JSON:

```typescript
// test-config.ts
export interface TestConfig {
    baseUrl: string;
    firstExerciseQuery: string;
    patternExerciseId: string;
    testDatabase: {
        core: string;
        exercises: string;
    };
}
```

## Success Criteria

- [ ] All existing test scenarios have TypeScript equivalents
- [ ] All tests pass consistently (no flaky tests)
- [ ] Test execution time is improved
- [ ] Tests can run in parallel
- [ ] Documentation is updated
- [ ] Legacy test code is removed

This plan provides an incremental path that allows for continuous testing and validation throughout the migration process. 