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
    $AdminPassword,
    [parameter()]
    $CreateOptions)

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
    Write-Host "Checking if database exists...";
    $result = Invoke-Sqlcmd -Query "SELECT [name] FROM [sys].[databases] WHERE [name] = N'$database'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
    if($result.name){
        Write-Host "Deleting existing version of database";
        Invoke-Sqlcmd -Query "ALTER DATABASE $database SET READ_ONLY WITH ROLLBACK IMMEDIATE" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
        Invoke-Sqlcmd -Query "DROP DATABASE $database" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
        Write-Host "Deleted existing database."
    }

    Write-Host "Creating database: 'CREATE DATABASE $database $CreateOptions'";
    Invoke-Sqlcmd -Query "CREATE DATABASE $database $CreateOptions" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
    Write-Host "Created."
}
catch{
    Write-Error "Powershell Script error: $_" -EA Stop
}

#user
try{
    Write-Host "Creating User: $NewUserName"
    $result = Invoke-Sqlcmd -Query "SELECT [name] FROM master.dbo.syslogins WHERE name = '$NewUserName'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
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

    Invoke-Sqlcmd -Query "EXEC sp_defaultdb @loginame='$NewUserName', @defdb='$Database'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -ErrorAction Stop
}
catch{
    Write-Error "Powershell Script error: $_" -EA Stop
}

#numbers table
try{
    Write-Host "Creating Table: dbo.Numbers"
    Invoke-Sqlcmd -InputFile "$path\Data\numbers.sql" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -ErrorAction Stop
    Invoke-Sqlcmd -Query "SELECT COUNT(*) FROM dbo.Numbers;" -ServerInstance "$Server" -Username "$NewUserName" -Password "$NewPassword" -Database "$Database" -ErrorAction Stop
    Write-Host "Created."
}
catch{
    Write-Error "Powershell Script error: $_" -EA Stop
}

#clients table
try{
    Write-Host "Creating Table: dbo.Clients"
    $girlsnames = ("<ns><n>" + [string]::Join("</n><n>",(Get-Content "$path\Data\girlsforenames.txt")) + "</n></ns>").Replace("'","''")
    $boysnames =  ("<ns><n>" + [string]::Join("</n><n>",(Get-Content "$path\Data\boysforenames.txt")) + "</n></ns>").Replace("'","''")
    $lastnames =  ("<ns><n>" + [string]::Join("</n><n>",(Get-Content "$path\Data\surnames.txt")) + "</n></ns>").Replace("'","''")
    
    (Get-Content "$path\Data\BulkImportNames.AzureFriendly.sql") `
                    | % {$_ -replace "{{GIRLSNAMES}}", $girlsnames} `
                    | % {$_ -replace "{{BOYSNAMES}}", $boysnames} `
                    | % {$_ -replace "{{LASTNAMES}}", $lastnames} `
                    | Set-Content -path "$path\Data\BulkImportNamesRunnable.sql"

    invoke-sqlcmd -inputfile "$path\Data\BulkImportNamesRunnable.sql" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -QueryTimeout 120  -ErrorAction Stop
    Invoke-Sqlcmd -Query "SELECT COUNT(*) FROM dbo.Clients;"  -ServerInstance "$Server" -Username "$NewUserName" -Password "$NewPassword" -Database "$Database" -ErrorAction Stop
    DEL "$path\Data\BulkImportNamesRunnable.sql"
    Write-Host "Created."
}
catch{
    Write-Error "Powershell Script error: $_" -EA Stop
}


Write-Host "Done."