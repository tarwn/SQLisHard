if (!(Get-Module -ListAvailable -Name SqlServer)) {
    Write-Host "Installing SqlServer module..."
    Install-PSResource -Name SqlServer -Force
}
Import-Module SqlServer
