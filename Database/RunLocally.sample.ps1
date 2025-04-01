# 1) Copy this file to RunLocally.ps1
# 2) Open RunLocally.ps1 + substitute meaningful values for the variables below (update web.config connection strings also)
# 3) [Cross your fingers and] Run it 

# Install and import SqlServer if not already present
if (!(Get-Module -ListAvailable -Name SqlServer)) {
    Write-Host "Installing SqlServer module..."
    Install-PSResource -Name SqlServer -Force
}
Import-Module SqlServer

$DbServer = "localhost"
$DbAdminUsername = "admin"
$DbAdminPassword = "password"

$DbSampleDatabase = "SampleDB"
$DbSampleReadUsername = "readuser"
$DbSampleReadPassword = "password"

$DbCoreDatabase = "CoreDB"
$DbCoreUsername = "coreuser"
$DbCorePassword = "password"

.\UpdateSampleDatabase.ps1 -s $DbServer -d $DbSampleDatabase -nu $DbSampleReadUsername -np $DbSampleReadPassword -au $DbAdminUsername -ap $DbAdminPassword -DeleteGeneratedContentAfter $true

.\UpdateCoreDatabase.ps1 -s $DbServer -d $DbCoreDatabase -nu $DbCoreUsername -np $DbCorePassword -au $DbAdminUsername -ap $DbAdminPassword
