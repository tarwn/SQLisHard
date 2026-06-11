# SQLisHard

When I first wrote this system, it was 2012 and there were a lot of books, blog posts, and videos to point new developers at, but no interactive projects or exercises easily at hand. I built SQLisHard based on a small number of earlier sites, like TryRuby, and also used it as a testbed to try a number of different tools. Since then, more established sites were built for SQL, from learning platforms to leetcode problems to hiring platforms. 

This is the 2025 refresh.

From 2012 .Net Framework, Knockout.js, and so on, to modern versions of .Net, Svelte, and so on.

As always, the system will work, but the code and choices behind it may be experimental, so don't use this in production.

# Development

## Running the app

* backend: `dotnet watch --project ./src/backend/SQLisHard/SQLisHard.csproj`

## Running Tests

This project includes both backend unit tests and end-to-end tests using Playwright.

### Quick Start

```bash
# Backend unit tests
dotnet test ./src/backend/SQLisHard.sln

# Running the app for E2e Tests
cd src/backend/SQLisHard
dotnet run --urls=http://localhost:8012 --environment=Test

# E2E tests (requires app to be running)
cd src/e2e-tests
npm run test
```

### E2E Test Commands

```bash
# Run all tests
npm run test

# Run tests with browser visible
npm run test:headed

# Run tests in debug mode
npm run test:debug

# Run connectivity test only
npm run test:connectivity
```

For more detailed test configuration and advanced usage, see the [E2E Test Documentation](./src/e2e-tests/README.md).

## Setting up the app

Dependencies:

- SQL Server 2019 or newer
- .Net 9
- Node.js 22+ (for e2e tests)
- Chrome browser (for e2e tests)

Setup Dependencies:

- Powershell 7.4.x or newer for easy Windows setup scripts

**Windows Setup**

1. Database
    - Create an admin user with rights to create databases, users, etc. (`./Database/setup/AdminAccountSetup.sql`)
    - Run `./Database/setup/InstallModules.ps1` (possibly as administrator?)
    - Run `./Database/setup/CreateCoreDatabase.ps1 -s <server> -d <db name> -nu <new app user name> -np <new app user pwd> -au <admin username> -ap <admin password>`
        - Creates the new core db with the given name
        - Creates the SQL user for the app w/ appropriate rights
    - Run `./Database/setup/CreateExerciseDatabase.ps1 -s <server> -d <db name> -nu <new app user #2 name> -np <new app user #2 pwd> -au <admin username> -ap <admin pwd>`
        - Creates the new exercise (or sample) db
        - Creates the SQL user with very limited rights for the app to run user queries against
2. Configure DB Connection Strings
    1. Init user secrets
        - `cd ./src/backend/SQLisHard/`
        - `dotnet user-secrets init`
    2. Add Admin settings used for migrating each database automatically when running locally:
        - `dotnet user-secrets set "Migrations:Core:ConnectionString" "Server=localhost;Database=<core db name>;User Id=<admin username>;Password=<admin pw>;Encrypt=false"`
        - `dotnet user-secrets set "Migrations:Exercises:ConnectionString" "Server=localhost;Database=<exercises db name>;User Id=<admin username>;Password=<admin pw>;Encrypt=false"`
    3. Add Application settings for the backend to access DB's with limited users created above
        - `dotnet user-secrets set "ConnectionStrings:Core" "Server=localhost;Database=<core db name>;User Id=<app username>;Password=<app user pw>;Encrypt=false"`
        - `dotnet user-secrets set "ConnectionStrings:Exercises" "Server=localhost;Database=<exercises db name>;User Id=<app username>;Password=<app user pw>;Encrypt=false"`
3. Backend
    - `cd src/backend`
    - `dotnet restore`
    - `dotnet build`
4. Frontend
    - _No steps currently_
5. E2E Tests
    - `cd src/e2e-tests`
    - `npm install`
    - `npx playwright install chrome`
6. GOTO [Running the app](#running-the-app)

**Other Setup**

TBD

Licensing
==========

The code is available publicly to let people who are curious see how it works, learn from it, and potentially contribute (in either commentary or new code). No permission is granted to copy and reuse the code for commercial purposes. If you are interested in reusing the code for commercial purposes, contact me.
