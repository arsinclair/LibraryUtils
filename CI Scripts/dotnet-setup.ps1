$dotnetExists = $false
$sdkExists = $false

# SDK includes Runtime and Hosting Bundle; SDK is required to build and publish the app, otherwise can use Hosting Bundle
# https://dotnetcli.azureedge.net/dotnet/Sdk/5.0.200/dotnet-sdk-5.0.200-win-x64.exe
# https://dotnetcli.azureedge.net/dotnet/aspnetcore/Runtime/5.0.3/aspnetcore-runtime-5.0.3-win-x64.exe

# https://download.visualstudio.microsoft.com/download/pr/dff39ddb-b399-43c5-9af0-04875134ce04/1c449bb9ad4cf75ec616482854751069/dotnet-hosting-5.0.3-win.exe

try {
    if(Get-Command dotnet) { $dotnetExists = $true }
} 
catch [System.Management.Automation.CommandNotFoundException] { }

# Validate Runtime Presence `(dir (Get-Command dotnet).Path.Replace('dotnet.exe', 'shared\Microsoft.NETCore.App')).Name`
if ($dotnetExists) {
    try {
        if ((((dir (Get-Command dotnet).Path.Replace('dotnet.exe', 'sdk')).Name) -like '5.0.200').Count -ne 0) {
            $sdkExists = $true
        }
    }
    catch { }
}

if (!$sdkExists) { 
    wget "https://dotnetcli.azureedge.net/dotnet/Sdk/5.0.200/dotnet-sdk-5.0.200-win-x64.exe" -outfile "dotnet-setup.exe";
    $pathvargs = { .\dotnet-setup.exe /install /quiet /norestart };
    Invoke-Command -ScriptBlock $pathvargs
}

$runtimeExists = Get-WebGlobalModule | where-object { $_.name.ToLower() -eq "aspnetcoremodule" }
if (!$runtimeExists) {
    wget "https://download.visualstudio.microsoft.com/download/pr/dff39ddb-b399-43c5-9af0-04875134ce04/1c449bb9ad4cf75ec616482854751069/dotnet-hosting-5.0.3-win.exe" -outfile "dotnet-hosting-setup.exe";
    $pathvargs = { .\dotnet-hosting-setup.exe /install /quiet /norestart };
    Invoke-Command -ScriptBlock $pathvargs
}
catch { }
