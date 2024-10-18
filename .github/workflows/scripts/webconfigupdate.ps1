param (        
    [string]$DBConnectionStringParamName,
    [hashtable]$ConfigParams
)

$WebConfigTemplate = ".\Web.config.template"
$WebConfigFile = "$pwd\Web.config"

[xml]$xml = Get-Content -Path $WebConfigTemplate

foreach ($key in $ConfigParams.Keys) {
    $value = $ConfigParams[$key]
    # Write-Host "Parameter Name: $key, Value: $value"

    # The DB connection string param is handled differently than the rest
    if ($key -eq "DBConnectionString") {

        $existingSetting = $xml.configuration.connectionStrings.add | Where-Object { $_.name -eq $DBConnectionStringParamName }

        if ($existingSetting) {
            $existingSetting.connectionString = $value
        }
        else {
            $newElement = $xml.CreateElement("add")
            $newElement.SetAttribute("name", $DBConnectionStringParamName)
            $newElement.SetAttribute("connectionString", $value)    
            $xml.configuration.connectionStrings.AppendChild($newElement) | Out-Null
        }
    }
    
    else {

        $existingSetting = $xml.configuration.appSettings.add | Where-Object { $_.key -eq $key }
        
        if ($existingSetting) {        
            $existingSetting.value = $value
            Write-Host "Updated key '$key' with value '$value'."
        } else {              
            $newElement = $xml.CreateElement("add")
            $newElement.SetAttribute("key", $key)
            $newElement.SetAttribute("value", $value)
            $xml.configuration.appSettings.AppendChild($newElement) | Out-Null
            Write-Host "Added new key '$key' with value '$value'."
        }
    }
}

try {
    $xml.Save($WebConfigFile)
}
catch {
    Write-Host "An error occurred: $_"

}
