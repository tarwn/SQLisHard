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
Write-Host "Checking database exists...";
$result = Invoke-Sqlcmd -Query "SELECT [name] FROM [sys].[databases] WHERE [name] = N'$database'" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
if($result.name){
    Write-Host "Database already exists";
}
else{
    Write-Host "Creating Database: $database"

    Invoke-Sqlcmd -Query "CREATE DATABASE $database; ALTER DATABASE $database SET RECOVERY SIMPLE;" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "master" -ErrorAction Stop
    Write-Host "Created."
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

#updates tracking
try{
    Write-Host "Creating Update Tracking Table If Not Exists"
    Invoke-Sqlcmd -Query "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UpdateTracking') CREATE TABLE UpdateTracking (UpdateTrackingKey int IDENTITY(1,1) PRIMARY KEY, Name varchar(255) NOT NULL, Applied DateTime NOT NULL);" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -ErrorAction Stop
    Write-Host "Done"
}
catch{
    Write-Error "Powershell Script error: $_" -EA Stop
}

#database updates
$sb = New-Object -TypeName "System.Text.StringBuilder"
$fileUpdates = Get-ChildItem "$path\CoreDatabaseUpdates"
$datestamp = $(get-date -f "yyyy-MM-dd HH:mm")

$sb.AppendLine("/* SQL Core Updates - Updated $datestamp */")
$sb.AppendLine("BEGIN TRANSACTION")

foreach($file in $fileUpdates) 
{ 
    $name = ($file.Name)
    $namewe = ([System.IO.Path]::GetFileNameWithoutExtension($name))

    $sb.AppendLine("/* File: $name */")
    $sb.AppendLine("IF NOT EXISTS (SELECT 1 FROM UpdateTracking WHERE Name = '$namewe')")
    $sb.AppendLine("BEGIN")

    $sb.AppendLine("EXEC('")
    (Get-Content "$path\CoreDatabaseUpdates\$name") | % {$_ -replace "'", "''"} | $sb.AppendLine("$_")
    $sb.AppendLine("';")

    $sb.AppendLine("INSERT INTO UpdateTracking(Name, Applied) SELECT '$namewe', GETUTCDATE();")
    $sb.AppendLine("END")
}  

$sb.AppendLine("COMMIT TRANSACTION")

Set-Content -path "$path\CoreDatabaseUpdatesBatch.sql" $sb.ToString()

Write-Host "Done."