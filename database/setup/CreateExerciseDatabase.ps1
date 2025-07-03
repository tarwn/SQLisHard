Param(
    [parameter(Mandatory=$true)]
    [alias("s")]
    [string]
    $Server,
    [parameter(Mandatory=$true)]
    [alias("d")]
    [string]
    $Database,
    [parameter(Mandatory=$true)]
    [alias("nu")]
    [string]
    $NewUserName,
    [parameter(Mandatory=$true)]
    [alias("np")]
    [string]
    $NewPassword,
    [parameter(Mandatory=$true)]
    [alias("au")]
    [string]
    $AdminUserName,
    [parameter(Mandatory=$true)]
    [alias("ap")]
    [string]
    $AdminPassword,
    [parameter()]
    [string]
    $CreateOptions,
    [parameter()]
    [bool]
    $DeleteGeneratedContentAfter
    )

$path = (Get-Location).Path
$UpdatesFolder = [string]"$path\SampleDatabaseUpdates"

# Ensure SqlServer is loaded
if (!(Get-Module -Name SqlServer)) {
    Import-Module SqlServer
}

# ---------------------------------- Environment ---------------------------------------------
# Ensure database, security, etc are setup

#database
try{
    Write-Host "Checking if database exists...";
    $result = Invoke-SqlCmd -Query "SELECT [name] FROM [sys].[databases] WHERE [name] = N'$database'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -TrustServerCertificate -ErrorAction Stop
    if($result.name){
        #Write-Host "Deleting existing version of database";
        #Invoke-Sqlcmd -Query "ALTER DATABASE $database SET READ_ONLY WITH ROLLBACK IMMEDIATE" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
        #Invoke-Sqlcmd -Query "DROP DATABASE $database" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
        #Write-Host "Deleted existing database."
        Write-Host "Database already exists."
    }
    else{
        Write-Host "Creating database: 'CREATE DATABASE $database $CreateOptions'";
        Invoke-Sqlcmd -Query "CREATE DATABASE $database $CreateOptions" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -TrustServerCertificate -ErrorAction Stop
        Write-Host "Created."
    }
}
catch{
    Write-Error "Powershell Script error: $_" -EA Stop
}

#user
try{
    Write-Host "Creating User: $NewUserName"
    $result = Invoke-Sqlcmd -Query "SELECT [name] FROM sys.sql_logins WHERE name = '$NewUserName'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -TrustServerCertificate -ErrorAction Stop
    if($result.name){
        Write-Host "Login already exists"
    }
    else{
        Write-Host "Creating login..."
        Invoke-Sqlcmd -Query "CREATE LOGIN $NewUserName WITH PASSWORD = '$NewPassword'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -TrustServerCertificate -ErrorAction Stop
        Write-Host "Login Created."
    }
    $result = Invoke-Sqlcmd -Query "SELECT [name] FROM sys.sysusers WHERE name = '$NewUserName'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -TrustServerCertificate -ErrorAction Stop
    if($result.name){
        Write-Host "User already exists"
    }
    else{
        Write-Host "Creating user..."
        Invoke-Sqlcmd -Query "CREATE USER $NewUserName FOR LOGIN $NewUserName WITH DEFAULT_SCHEMA = dbo" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -TrustServerCertificate -ErrorAction Stop
        Invoke-Sqlcmd -Query "EXEC sp_addrolemember 'db_datareader','$NewUserName'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -TrustServerCertificate -ErrorAction Stop
        Write-Host "User Created."
    }
}
catch{
    Write-Error "Powershell Script error: $_" -EA Stop
}
