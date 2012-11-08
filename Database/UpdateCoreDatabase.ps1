Param(
    [parameter(Mandatory=$true)]
    [alias("s")]
    $Server,
    [parameter(Mandatory=$true)]
    [alias("d")]
    $Database,
    [parameter(Mandatory=$true)]
    [alias("nu")]
    $NewUserName,
    [parameter(Mandatory=$true)]
    [alias("np")]
    $NewPassword,
    [parameter(Mandatory=$true)]
    [alias("au")]
    $AdminUserName,
    [parameter(Mandatory=$true)]
    [alias("ap")]
    $AdminPassword)

try{    
    if ( (Get-PSSnapin -Name SqlServerCmdletSnapin100 -ErrorAction SilentlyContinue) -eq $null ){
        Add-PSSnapin SqlServerCmdletSnapin100
        Add-PSSnapin SqlServerProviderSnapin100
    }
}
catch{
    Write-Error "Powershell Script error: $_" -EA Stop
}


$path = (Get-Location).Path

#database
try{
    Write-Host "Creating Database: $database"

    Invoke-Sqlcmd -Query "CREATE DATABASE $database; ALTER DATABASE $database SET RECOVERY SIMPLE;" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
    Write-Host "Created."
}
catch{
    Write-Error "Error while creating database, assuming it already exists: $_"
}

#user
try{
    Write-Host "Creating User: $NewUserName"
    Invoke-Sqlcmd -Query "IF NOT EXISTS (SELECT 1 FROM master.dbo.syslogins WHERE name = '$NewUserName') CREATE LOGIN $NewUserName WITH PASSWORD = '$NewPassword';" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
    Invoke-Sqlcmd -Query "IF NOT EXISTS (SELECT 1 FROM sys.sysusers WHERE name = '$NewUserName') CREATE USER $NewUserName FOR LOGIN $NewUserName WITH DEFAULT_SCHEMA = dbo;" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -ErrorAction Stop
    Invoke-Sqlcmd -Query "EXEC sp_defaultdb @loginame='$NewUserName', @defdb='$Database'; EXEC sp_addrolemember 'db_datareader','$NewUserName'; EXEC sp_addrolemember 'db_datawriter','$NewUserName'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -ErrorAction Stop
    Write-Host "Created."
}
catch{
    Write-Error "Powershell Script error: $_" -EA Stop
}

#TODO : add execution of changes in change folder

Write-Host "Done."