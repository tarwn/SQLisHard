Add-PSSnapin SqlServerCmdletSnapin100
Add-PSSnapin SqlServerProviderSnapin100

$path = (Get-Location).Path
$database = "SQLisHard_01"
$server = "localhost"
$readuser = "readuser"
$readpass = "12345"

#database
Write-Host "Creating Database: $database"
Invoke-Sqlcmd -Query "CREATE DATABASE $database; ALTER DATABASE $database SET RECOVERY SIMPLE;" -ServerInstance "$server" -Database "master"
Write-Host "Created."

#user
Write-Host "Creating User: $readuser"
Invoke-Sqlcmd -Query "IF NOT EXISTS (SELECT 1 FROM master.dbo.syslogins WHERE name = '$readuser') CREATE LOGIN $readuser WITH PASSWORD = '$readpass';" -ServerInstance "$server" -Database "master"
Invoke-Sqlcmd -Query "EXEC sp_defaultdb @loginame='$readuser', @defdb='$database'; CREATE USER readuser FOR LOGIN readuser WITH DEFAULT_SCHEMA = dbo; EXEC sp_addrolemember 'db_datareader','$readuser'" -ServerInstance "$server" -Database "$database"
Write-Host "Created."

#numbers table
Write-Host "Creating Table: dbo.Numbers"
Invoke-Sqlcmd -InputFile "$path\Data\numbers.sql" -ServerInstance "$server" -Database "$database"
Invoke-Sqlcmd -Query "SELECT COUNT(*) FROM dbo.Numbers;" -ServerInstance "$server" -Database "$database" -Username "$readuser" -Password "$readpass"
Write-Host "Created."

#clients table
Write-Host "Creating Table: dbo.Clients"
$old="%PATH%"
$new="$path\Data\"
(Get-Content "$path\Data\BulkImportNames.sql") | % {$_ -replace $old, $new} | Set-Content -path "$path\Data\BulkImportNamesRunnable.sql"
invoke-sqlcmd -inputfile "$path\Data\BulkImportNamesRunnable.sql" -serverinstance "$server" -database "$database" -QueryTimeout 120 
Invoke-Sqlcmd -Query "SELECT COUNT(*) FROM dbo.Clients;" -ServerInstance "$server" -Database "$database" -Username "$readuser" -Password "$readpass"
DEL "$path\Data\BulkImportNamesRunnable.sql"
Write-Host "Created."


Write-Host "Done."