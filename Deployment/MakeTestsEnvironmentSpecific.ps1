Param(
        [parameter(Mandatory=$true)]
    [alias("a")]
    $IISAppName,
    [parameter(Mandatory=$true)]
    $URL,
    [parameter(Mandatory=$true)]
    [alias("s")]
    $DBServer,
    [parameter(Mandatory=$true)]
    [alias("ds")]
    $DBSName,
    [parameter(Mandatory=$true)]
    [alias("us")]
    $DBSUsername,
    [parameter(Mandatory=$true)]
    [alias("ps")]
    $DBSPassword,
    [parameter(Mandatory=$true)]
    [alias("dc")]
    $DBCName,
    [parameter(Mandatory=$true)]
    [alias("uc")]
    $DBCUsername,
    [parameter(Mandatory=$true)]
    [alias("pc")]
    $DBCPassword)

$path = (Get-Location).Path

# Integration Test Configuration
(Get-Content "$path\TestRun.Template.config") `
    | % {$_ -replace "{{URL}}", $URL} `
    | % {$_ -replace "{{IIS APP NAME}}", $IISAppName} `
    | % {$_ -replace "{{DB SERVER}}", $DBServer} `
    | % {$_ -replace "{{DB SAMPLE}}", $DBSName} `
    | % {$_ -replace "{{DB SAMPLE READ USERNAME}}", $DBSUsername} `
    | % {$_ -replace "{{DB SAMPLE READ PASSWORD}}", $DBSPassword} `
    | % {$_ -replace "{{DB CORE}}", $DBCName} `
    | % {$_ -replace "{{DB CORE USERNAME}}", $DBCUsername} `
    | % {$_ -replace "{{DB CORE PASSWORD}}", $DBCPassword} `
    | Set-Content -path "$path\..\SQLisHard.IntegrationTests\bin\Release\Configs\TestRun.config"