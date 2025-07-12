# E2E Tests with Playwright

## Setup

```bash
npm install
npx playwright install chrome
```

## Scripts

- `npx playwright test` — Run all tests
- `npx tsc --noEmit` — Type check
- `npx eslint . --ext .ts,.js` — Lint code

## Usage

- Tests are in `tests/`
- Only Chrome is configured for testing

## Project Structure

- `playwright.config.ts` — Playwright config (Chrome-only)
- `tsconfig.json` — TypeScript config
- `.eslintrc.js` — ESLint config
- `tests/` — Test specs and helpers 