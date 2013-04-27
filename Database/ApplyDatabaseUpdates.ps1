

function ApplyDatabaseUpdates
{
    param (
        [parameter(Mandatory=$true)]
        [string]
        $UpdatesFolder,
        [parameter(Mandatory=$true)]
        [string]
        $Server,
        [parameter(Mandatory=$true)]
        [string]
        $Database,
        [parameter(Mandatory=$true)]
        [string]
        $AdminUserName,
        [parameter(Mandatory=$true)]
        [string]
        $AdminPassword
    )

    $path = (Get-Location).Path

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
    $outputPath = "$path\UpdatesBatch.sql"
    $stream = [System.IO.StreamWriter] "$outputPath"
    $fileUpdates = Get-ChildItem "$UpdatesFolder"
    $datestamp = $(get-date -f "yyyy-MM-dd HH:mm")

    $stream.WriteLine("/* SQL Core Updates - Updated $datestamp */")
    $stream.WriteLine("BEGIN TRANSACTION")

    foreach($file in $fileUpdates) 
    { 
        $name = ($file.Name)
        $namewe = ([System.IO.Path]::GetFileNameWithoutExtension($name))

        $stream.WriteLine("")
        $stream.WriteLine("/* File: $name */")
        $stream.WriteLine("IF NOT EXISTS (SELECT 1 FROM UpdateTracking WHERE Name = '$namewe')")
        $stream.WriteLine("BEGIN")

        $stream.WriteLine("`tPrint 'Applying Update: $namewe'")
        $stream.WriteLine("`tEXEC('")
        (Get-Content "$UpdatesFolder\$name") | % {$_ -replace "'", "''"} | % {$stream.WriteLine("`t`t$_")}
        $stream.WriteLine("`t');")

        $stream.WriteLine("`tINSERT INTO UpdateTracking(Name, Applied) SELECT '$namewe', GETUTCDATE();")
        $stream.WriteLine("END")
    }  

    $stream.WriteLine("COMMIT TRANSACTION")
    $stream.Close()
    Write-Host "Update Script Created."

    Write-Host "Running updates..."

    Invoke-SqlCmd -InputFile "$outputPath" -ServerInstance "$Server" -Username "$AdminUserName" -Password "$AdminPassword" -Database "$Database" -Verbose -ErrorAction Stop

    Remove-Item "$outputPath"

    Write-Host "Updates completed."
}