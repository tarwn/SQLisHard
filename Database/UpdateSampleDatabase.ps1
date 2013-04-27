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

# For 2010 - load the modules
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
    try{
        Write-Host "Checking if database exists...";
        $result = Invoke-Sqlcmd -Query "SELECT [name] FROM [sys].[databases] WHERE [name] = N'$database'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
        if($result.name){
            #Write-Host "Deleting existing version of database";
            #Invoke-Sqlcmd -Query "ALTER DATABASE $database SET READ_ONLY WITH ROLLBACK IMMEDIATE" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
            #Invoke-Sqlcmd -Query "DROP DATABASE $database" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
            #Write-Host "Deleted existing database."
            Write-Host "Database already exists."
        }
        else{
            Write-Host "Creating database: 'CREATE DATABASE $database $CreateOptions'";
            Invoke-Sqlcmd -Query "CREATE DATABASE $database $CreateOptions" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
            Write-Host "Created."
        }
    }
    catch{
        Write-Error "Powershell Script error: $_" -EA Stop
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
            Invoke-Sqlcmd -Query "EXEC sp_addrolemember 'db_datareader','$NewUserName'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -ErrorAction Stop
            Write-Host "User Created."
        }
    }
    catch{
        Write-Error "Powershell Script error: $_" -EA Stop
    }

# ---------------------------------- Content Generation ---------------------------------------------
# Scripts to generate content dynamically and update the appropriate update script

    #generate customers table content
    $CustomersContentPath = "$UpdatesFolder\0002_CustomersData.sql"
    try{
        Write-Host "Generating content script for dbo.Customers"
        $girlsnames = ("<ns><n>" + [string]::Join("</n><n>",(Get-Content "$path\Data\girlsforenames.txt")) + "</n></ns>").Replace("'","''")
        $boysnames =  ("<ns><n>" + [string]::Join("</n><n>",(Get-Content "$path\Data\boysforenames.txt")) + "</n></ns>").Replace("'","''")
        $lastnames =  ("<ns><n>" + [string]::Join("</n><n>",(Get-Content "$path\Data\surnames.txt")) + "</n></ns>").Replace("'","''")
    
        (Get-Content "$path\Data\BulkImportNames.AzureFriendly.sql") `
                        | % {$_ -replace "{{GIRLSNAMES}}", $girlsnames} `
                        | % {$_ -replace "{{BOYSNAMES}}", $boysnames} `
                        | % {$_ -replace "{{LASTNAMES}}", $lastnames} `
                        | Set-Content -path "$CustomersContentPath"
    }
    catch{
        Write-Error "Powershell Script error: $_" -EA Stop
    }


# ---------------------------------- Apply Updates ---------------------------------------------

    Write-Host "Applying changes from $UpdatesFolder"

    # Apply database updates
    ApplyDatabaseUpdates $UpdatesFolder $Server $Database $AdminUserName $AdminPassword

    # Clean up generated content - mostly for local usage so we don't commit the files
    if($DeleteGeneratedContentAfter -eq $true){
        Write-Host "Cleaning up generated content to make commits safe"
        $GeneratedContentReplacement = "RAISERROR (N'Content for database update has not been generated',1,1)
"

        $GeneratedContentReplacement > $CustomersContentPath

        Write-Host "Generated content cleaned up"
    }

Write-Host "Done."