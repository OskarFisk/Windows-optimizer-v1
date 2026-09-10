[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$os = Get-CimInstance Win32_OperatingSystem
$build = [int]$os.BuildNumber
if ($build -lt 19041) {
    Write-Error "Windows 10 version 2004 (build 19041) or newer is required. Detected build $build."
    exit 1
}

$arch = $env:PROCESSOR_ARCHITECTURE
if ($arch -notin @('AMD64','ARM64')) {
    Write-Error "Unsupported architecture: $arch. Windows x64 or ARM64 is required."
    exit 1
}

# Windows Optimizer is published self-contained, so the .NET runtime is not
# required on the target machine. This check intentionally does not download
# an unnecessary runtime.

$wingetAvailable = $false
try {
    $winget = Get-Command winget.exe -ErrorAction Stop
    $wingetAvailable = $true
    Write-Host "winget detected: $($winget.Source)"
} catch {
    Write-Host "winget is not available. Windows Optimizer can still run; optional package installation will use official web installers when available."
}

Write-Host "Windows Optimizer prerequisites: OK" -ForegroundColor Green
Write-Host "OS: $($os.Caption)"
Write-Host "Build: $build"
Write-Host "Architecture: $arch"
Write-Host "winget: $wingetAvailable"
exit 0
