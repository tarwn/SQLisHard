# E2E Tests with Playwright

## Setup

```bash
npm install
npx playwright install chrome
```

## Configuration

The base URL for tests can be configured using the `BASE_URL` environment variable:

```bash
# Use default localhost:8012
npx playwright test

# Use custom base URL (Unix/Linux/macOS)
BASE_URL=http://localhost:3000 npx playwright test

# Use production URL (Unix/Linux/macOS)
BASE_URL=https://your-app.com npx playwright test
```

```powershell
# Use custom base URL (Windows PowerShell)
$env:BASE_URL="http://localhost:3000"; npx playwright test

# Use production URL (Windows PowerShell)
$env:BASE_URL="https://your-app.com"; npx playwright test
```

## Scripts

- `npx playwright test` — Run all tests
- `npx tsc --noEmit` — Type check
- `npx eslint . --ext .ts,.js` — Lint code

## Usage

- Tests are in `tests/`
- Only Chrome is configured for testing
- Base URL defaults to `http://localhost:8012` if not specified

## Project Structure

- `playwright.config.ts` — Playwright config (Chrome-only)
- `tsconfig.json` — TypeScript config
- `.eslintrc.js` — ESLint config
- `tests/` — Test specs and helpers 