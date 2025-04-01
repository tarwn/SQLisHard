# SQLisHard

When I first wrote this system, it was 2012 and there were a lot of books, blog posts, and videos to point new developers at, but no interactive projects or exercises easily at hand. I built SQLisHard based on a small number of earlier sites, like TryRuby, and also used it as a testbed to try a number of different tools. Since then, more established sites were built for SQL, from learning platforms to leetcode problems to hiring platforms. 

This is the 2025 refresh.

From 2012 .Net Framework, Knockout.js, and so on, to modern versions of .Net, Svelte, and so on.

As always, the system will work, but the code and choices behind it may be experimental, so don't use this in production.

# Development

## Running the app

_This is the version before we start the modernization, lightly updated to run w/ `dotnet` CLI`_

* Use Visual Studio for now, run `SQLisHard.csproj` project w/ IISExpress

## Setting up the app

_This is the version before we start the modernization, lightly updated to run w/ PS 7.4.x._

To run this locally, you will need:

- SQL Server 2019 or newer
- .Net 4.something
- Powershell 7.4.x or newer

**Setup**

1. Create a database user as your "admin" user, with rights to create databaes, users, etc.
2. Run the setup script to create databases, users, + apply initial DB migrations:
    * `cd Database`
    * Copy `Runlocally.sample.ps1` to `RunLocally.ps1` and enter real values
    * Run `./RunLocally.ps1` to setup the databases, which will also create the app DB users w/ limited rights
3. Open `SQLisHard.sln` in Visual Studio and let it install what it needs
4. Set SQLisHard project as the startup project, using IISExpress

Licensing
==========

The code is available publicly to let people who are curious see how it works, learn from it, and potentially contribute (in either commentary or new code). No permission is granted to copy and reuse the code for commercial purposes. If you are interested in reusing the code for commercial purposes, contact me.
