Param(
    [parameter(Mandatory=$true)][alias("a")]	$IISAppName,
    [parameter(Mandatory=$true)]		$URL,
    [parameter(Mandatory=$true)][alias("s")]    $DBServer,
    [parameter(Mandatory=$true)][alias("ds")]   $DBSName,
    [parameter(Mandatory=$true)][alias("us")]   $DBSUsername,
    [parameter(Mandatory=$true)][alias("ps")]   $DBSPassword,
    [parameter(Mandatory=$true)][alias("dc")]   $DBCName,
    [parameter(Mandatory=$true)][alias("uc")]   $DBCUsername,
    [parameter(Mandatory=$true)][alias("pc")]   $DBCPassword,
    [parameter(Mandatory=$true)]		$SMTPFrom,
    [parameter(Mandatory=$true)]		$SMTPHost,
    [parameter(Mandatory=$true)]		$SMTPPort,
    [parameter(Mandatory=$true)]		$SMTPUsername,
    [parameter(Mandatory=$true)]		$SMTPPassword,
    [parameter(Mandatory=$true)]		$SMTPEnableSsl,
    [parameter(Mandatory=$true)]		$ApplicationVersion,
    [parameter(Mandatory=$true)]		$EnvironmentName)

$path = (Get-Location).Path

# MS Deploy Parameters Files
(Get-Content "$path\DeploymentParams.Template.xml") `
    | % {$_ -replace "{{URL}}", $URL} `
    | % {$_ -replace "{{IIS APP NAME}}", $IISAppName} `
    | % {$_ -replace "{{DB SERVER}}", $DBServer} `
    | % {$_ -replace "{{DB SAMPLE}}", $DBSName} `
    | % {$_ -replace "{{DB SAMPLE READ USERNAME}}", $DBSUsername} `
    | % {$_ -replace "{{DB SAMPLE READ PASSWORD}}", $DBSPassword} `
    | % {$_ -replace "{{DB CORE}}", $DBCName} `
    | % {$_ -replace "{{DB CORE USERNAME}}", $DBCUsername} `
    | % {$_ -replace "{{DB CORE PASSWORD}}", $DBCPassword} `
    | % {$_ -replace "{{SMTP.FROM}}", $SMTPFrom} `
    | % {$_ -replace "{{SMTP.HOST}}", $SMTPHost} `
    | % {$_ -replace "{{SMTP.PORT}}", $SMTPPort} `
    | % {$_ -replace "{{SMTP.USERNAME}}", $SMTPUsername} `
    | % {$_ -replace "{{SMTP.PASSWORD}}", $SMTPPassword} `
    | % {$_ -replace "{{SMTP.ENABLESSL}}", $SMTPEnableSsl} `
    | % {$_ -replace "{{APPLICATION.VERSION}}", $ApplicationVersion} `
    | % {$_ -replace "{{ENVIRONMENT.NAME}}", $EnvironmentName} `
    | Set-Content -path "$path\DeploymentParams.xml"



