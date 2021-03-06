# SDK includes Runtime and Hosting Bundle; SDK is required to build and publish the app, otherwise can use Hosting Bundle
# https://dotnetcli.azureedge.net/dotnet/Sdk/5.0.200/dotnet-sdk-5.0.200-win-x64.exe
# https://dotnetcli.azureedge.net/dotnet/aspnetcore/Runtime/5.0.3/aspnetcore-runtime-5.0.3-win-x64.exe

# https://download.visualstudio.microsoft.com/download/pr/dff39ddb-b399-43c5-9af0-04875134ce04/1c449bb9ad4cf75ec616482854751069/dotnet-hosting-5.0.3-win.exe

function InstallSDK {
    wget "https://dotnetcli.azureedge.net/dotnet/Sdk/5.0.200/dotnet-sdk-5.0.200-win-x64.exe" -outfile "dotnet-setup.exe";
    $pathvargs = { .\dotnet-setup.exe /install /quiet /norestart };
    Invoke-Command -ScriptBlock $pathvargs
    Remove-Item ".\dotnet-setup.exe" -Force -Confirm:$false
}

function InstallRuntime {
    wget "https://download.visualstudio.microsoft.com/download/pr/dff39ddb-b399-43c5-9af0-04875134ce04/1c449bb9ad4cf75ec616482854751069/dotnet-hosting-5.0.3-win.exe" -outfile "dotnet-hosting-setup.exe";
    $pathvargs = { .\dotnet-hosting-setup.exe /install /quiet /norestart };
    Invoke-Command -ScriptBlock $pathvargs
    Remove-Item ".\dotnet-hosting-setup.exe" -Force -Confirm:$false
}

$dotnetExists = $false
try {
    if(Get-Command dotnet) { $dotnetExists = $true }
} 
catch [System.Management.Automation.CommandNotFoundException] { }

if (!$dotnetExists) {
    InstallSDK
    echo "Installation Completed. Rebooting the machine. Please restart the Job, cause it is going to fail."
    Restart-Computer
}

$runtimes = dotnet --list-runtimes
$sdks = dotnet --list-sdks

if (($runtimes -like "Microsoft.AspNetCore.App 5.0.3*").Count -eq 0 -or ($runtimes -like "Microsoft.NETCore.App 5.0.3*").Count -eq 0) {
    InstallRuntime
}

if (($sdks -like "5.0.200*").Count == 0) {
    InstallSDK
}
