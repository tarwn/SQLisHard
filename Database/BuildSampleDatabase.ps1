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
    
if ( (Get-PSSnapin -Name SqlServerCmdletSnapin100 -ErrorAction SilentlyContinue) -eq $null ){
    Add-PSSnapin SqlServerCmdletSnapin100
    Add-PSSnapin SqlServerProviderSnapin100
}

$path = (Get-Location).Path

#database
try{
    Write-Host "Creating Database: $database"
    Invoke-Sqlcmd -Query "CREATE DATABASE $database; ALTER DATABASE $database SET RECOVERY SIMPLE;" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
    Write-Host "Created."
}
catch{
    Write-Error "Script error: $_" -EA Stop
}

#user
try{
    Write-Host "Creating User: $readuser"
    Invoke-Sqlcmd -Query "IF NOT EXISTS (SELECT 1 FROM master.dbo.syslogins WHERE name = '$NewUserName') CREATE LOGIN $NewUserName WITH PASSWORD = '$NewPassword';" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
    Invoke-Sqlcmd -Query "EXEC sp_defaultdb @loginame='$NewUserName', @defdb='$Database'; CREATE USER $NewUserName FOR LOGIN $NewUserName WITH DEFAULT_SCHEMA = dbo; EXEC sp_addrolemember 'db_datareader','$NewUserName'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -ErrorAction Stop
    Write-Host "Created."
}
catch{
    Write-Error "Script error: $_" -EA Stop
}

#numbers table
try{
    Write-Host "Creating Table: dbo.Numbers"
    Invoke-Sqlcmd -InputFile "$path\Data\numbers.sql" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -ErrorAction Stop
    Invoke-Sqlcmd -Query "SELECT COUNT(*) FROM dbo.Numbers;" -ServerInstance "$Server" -Username "$NewUserName" -Password "$NewPassword" -Database "$Database" -ErrorAction Stop
    Write-Host "Created."
}
catch{
    Write-Error "Script error: $_" -EA Stop
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
    Write-Error "Script error: $_" -EA Stop
}


Write-Host "Done."