function InstallSDK {
    echo "Installing dotnet SDK"
    wget "https://dotnetcli.azureedge.net/dotnet/Sdk/5.0.200/dotnet-sdk-5.0.200-win-x64.exe" -outfile "dotnet-setup.exe";
    $pathvargs = { .\dotnet-setup.exe /install /quiet /norestart };
    Invoke-Command -ScriptBlock $pathvargs
    Remove-Item ".\dotnet-setup.exe" -Force -Confirm:$false
}

function InstallRuntime {
    echo "Installing dotnet Runtime"
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

[string]$pathToDotnetX64 = "C:\Program Files (x86)\dotnet\dotnet.exe";
[string]$pathToDotnetX86 = "C:\Program Files\dotnet\dotnet.exe";
[Array]$arguments = "--list-runtimes";
$runtimes = & $pathToDotnetX86 $arguments;
$sdks = & $pathToDotnetX64 $arguments;

if (($runtimes -like "Microsoft.AspNetCore.App 5.0.3*").Count -eq 0 -or ($runtimes -like "Microsoft.NETCore.App 5.0.3*").Count -eq 0) {
    InstallRuntime
}

if (($sdks -like "5.0.200*").Count -eq 0) {
    InstallSDK
}
