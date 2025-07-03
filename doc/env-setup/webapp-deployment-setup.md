The deployment works via a github action from the `master` branch (tbd: will rename to `main`).

# Initial configuration

* Uses the [Manually set up a Github Actions workflow](https://learn.microsoft.com/en-us/azure/app-service/deploy-github-actions?tabs=userlevel%2Caspnetcore#manually-set-up-a-github-actions-workflow)
    * [Generate deployment credentials](https://learn.microsoft.com/en-us/azure/app-service/deploy-github-actions?tabs=userlevel%2Caspnetcore#generate-deployment-credentials) using the Service principal option 
    * example with test app when building first pass on action:
    ```
    az ad sp create-for-rbac --name "sqlishard-test" --role contributor \
                            --scopes /subscriptions/<subscription-id>/resourceGroups/<group-name>/providers/Microsoft.Web/sites/<app-name> \
                            --json-auth
    ```
* Add `AZURE_CREDENTIALS` as a github secret with the JSON output from the prior step
* Update the git action workflow to perform the build/publish/deploy using those values


