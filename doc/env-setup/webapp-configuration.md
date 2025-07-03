# Notes on configuring the webapp
These are notes from the migration on critical configuration changes for the test app that also are applied to the legacy real app while modernizing.

## Old Setup

* Relied on secrets set in web.config during build from TeamCity configured secrets

## New Setup

* Secrets during build are in Github Actions secrets
    * Service user JSON for deployment
    * Connection String w/ Admin access to Core and Exercises databases
* Secrets for deployed app are in Web App environment settings maybe?
    * Connection String "Core" with Read/Write access to DB only
    * Connection String "Exercises" with Read access to DB only
    * Env variable "Email:Username"
    * Env variable "Email:Password"
