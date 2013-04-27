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
    $AdminPassword)

$path = (Get-Location).Path
$UpdatesFolder = [string]"$path\CoreDatabaseUpdates"

# For SQL 2008 - load the modules
try{    
    if ( (Get-PSSnapin -Name SqlServerCmdletSnapin100 -ErrorAction SilentlyContinue) -eq $null -and (Get-PSSnapin -Registered -Name SqlServerCmdletSnapin100 -ErrorAction SilentlyContinue) -ne $null){
        Add-PSSnapin SqlServerCmdletSnapin100 -ErrorAction SilentlyContinue
        Add-PSSnapin SqlServerProviderSnapin100 -ErrorAction SilentlyContinue
    }
}
catch{
    Write-Error "Powershell Script error: $_" -EA Stop
}

# include the generic update function
. .\ApplyDatabaseUpdates.ps1


# ---------------------------------- Environment ---------------------------------------------
# Ensure database, security, etc are setup

    #database
        Write-Host "Checking database exists...";
        $result = Invoke-Sqlcmd -Query "SELECT [name] FROM [sys].[databases] WHERE [name] = N'$database'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
        if($result.name){
            Write-Host "Database already exists";
        }
        else{
            Write-Host "Creating Database: $database"

            Invoke-Sqlcmd -Query "CREATE DATABASE $database" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
            Invoke-Sqlcmd -Query "ALTER DATABASE $database SET RECOVERY SIMPLE" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
            Write-Host "Created."
        }

    #user
    try{
        Write-Host "Creating User: $NewUserName"
        $result = Invoke-Sqlcmd -Query "SELECT [name] FROM sys.sql_logins WHERE name = '$NewUserName'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
        if($result.name){
            Write-Host "Login already exists"
        }
        else{
            Write-Host "Creating login..."
            Invoke-Sqlcmd -Query "CREATE LOGIN $NewUserName WITH PASSWORD = '$NewPassword'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
            Write-Host "Login Created."
        }

        $result = Invoke-Sqlcmd -Query "SELECT [name] FROM sys.sysusers WHERE name = '$NewUserName'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -ErrorAction Stop
        if($result.name){
            Write-Host "User already exists"
        }
        else{
            Write-Host "Creating user..."
            Invoke-Sqlcmd -Query "CREATE USER $NewUserName FOR LOGIN $NewUserName WITH DEFAULT_SCHEMA = dbo" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -ErrorAction Stop
            Invoke-Sqlcmd -Query "EXEC sp_addrolemember 'db_datareader','$NewUserName'; EXEC sp_addrolemember 'db_datawriter','$NewUserName'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -ErrorAction Stop
            Write-Host "User Created."
        }
    }
    catch{
        Write-Error "Powershell Script error: $_" -EA Stop
    }

# ---------------------------------- Apply Updates ---------------------------------------------

    Write-Host "Applying changes from $UpdatesFolder"

    #Apply the updates
    ApplyDatabaseUpdates $UpdatesFolder $Server $Database $AdminUserName $AdminPassword

Write-Host "Done."