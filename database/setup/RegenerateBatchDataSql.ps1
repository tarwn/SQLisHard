$CustomersContentPath = "$PSScriptRoot/../exercisedb/migrations/0002_CustomersData.sql";

Write-Host "Generating content script for dbo.Customers"
$girlsnames = ("<ns><n>" + [string]::Join("</n><n>",(Get-Content "$PSScriptRoot\data\girlsforenames.txt")) + "</n></ns>").Replace("'","''")
$boysnames =  ("<ns><n>" + [string]::Join("</n><n>",(Get-Content "$PSScriptRoot\data\boysforenames.txt")) + "</n></ns>").Replace("'","''")
$lastnames =  ("<ns><n>" + [string]::Join("</n><n>",(Get-Content "$PSScriptRoot\data\surnames.txt")) + "</n></ns>").Replace("'","''")

(Get-Content "$PSScriptRoot/data\BulkImportNames.AzureFriendly.sql") `
                | % {$_ -replace "{{GIRLSNAMES}}", $girlsnames} `
                | % {$_ -replace "{{BOYSNAMES}}", $boysnames} `
                | % {$_ -replace "{{LASTNAMES}}", $lastnames} `
                | Set-Content -path "$CustomersContentPath"
