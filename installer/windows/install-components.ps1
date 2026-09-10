[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$InstallDirectory
)

$ErrorActionPreference = 'Stop'

# Windows Optimizer v1 is self-contained. There is no mandatory .NET runtime
# download. This script prepares the local package-installation capability and
# never disables Defender, Firewall, Windows Update, or other security controls.

$stateDir = Join-Path $InstallDirectory 'state'
New-Item -ItemType Directory -Path $stateDir -Force | Out-Null

$state = [ordered]@{
    installedAt = (Get-Date).ToUniversalTime().ToString('o')
    architecture = $env:PROCESSOR_ARCHITECTURE
    windowsBuild = [Environment]::OSVersion.Version.Build
    dotnetRuntimeRequired = $false
    wingetAvailable = $false
}

try {
    Get-Command winget.exe -ErrorAction Stop | Out-Null
    $state.wingetAvailable = $true
} catch {
    # winget is optional. The main application remains functional without it.
}

$state | ConvertTo-Json | Set-Content -Path (Join-Path $stateDir 'installation.json') -Encoding UTF8
Write-Host 'Required Windows Optimizer components prepared.'
if ($state.wingetAvailable) {
    Write-Host 'Windows Package Manager detected; optional packages can be installed from the Programs Library.'
} else {
    Write-Host 'Windows Package Manager was not detected; optional packages can still use their official installers.'
}
exit 0
