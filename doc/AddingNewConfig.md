

To add a new configuration value:

1) Add the key and a default value to SQLisHard/web.config
2) Add a setParameter element to Deployment/parameters.xml with a new token like {{SampleToken}}
3) Add a powershell param and replace line to Deployment/MakeParamsEnvironmentSpecific.ps1 to replace token with param value when called
4) Add a parameter entry in SQLisHard/parameters.xml for msdeploy to add the new parameter to the web.config
5) In TeamCity: For both builds, go to the Build Steps, and edit the Create Environment Configurations to add the new powershell parameter and specify a new TeamCity parameter
6) in TeamCity: For both builds, Open the Build Parameters, the new name from the prior line should be there missing a value, fill it in
