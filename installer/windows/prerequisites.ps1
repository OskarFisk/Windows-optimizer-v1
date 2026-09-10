$ErrorActionPreference = 'SilentlyContinue'

# The application is published self-contained, so .NET is NOT required.
# This script only installs a small, optional Windows component when it is
# missing. It never disables security features or changes system policy.

$osBuild = [System.Environment]::OSVersion.Version.Build
if ($osBuild -lt 19041) {
    Write-Host 'Windows Optimizer v1 requires Windows 10 20H1 (build 19041) or newer.'
    exit 1
}

# Ensure the Windows App Installer / winget stack is available for optional
# package installation from the Programs Library. We do not install arbitrary
# third-party software silently here.
try {
    $winget = Get-Command winget.exe -ErrorAction Stop
    Write-Host "winget detected: $($winget.Source)"
} catch {
    Write-Host 'winget is not available. The optimizer itself can still run.'
}

exit 0
