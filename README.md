# Windows Optimizer v1

A modern Windows desktop/laptop optimization and maintenance toolkit.

## v1 desktop build
The first functional desktop foundation is a .NET 8 WPF application with a redesigned performance-cockpit UI, live process/uptime cards, safe maintenance actions, UAC-on-demand operations and an integrated Programs Library launcher.

### Included now
- Modern dark performance-cockpit interface.
- CPU/process-load, managed-memory, system-drive and uptime cards.
- Temporary-file cleanup with locked-file skipping.
- DNS cache flush with administrator elevation when required.
- Conservative application-memory cleanup.
- Windows Update shortcut.
- Ninite integration through the official Ninite selector.
- Launchers for MSI Afterburner, AMD Ryzen Master and BlueStacks using their official vendor pages.
- Categorized `ProgramLibrary.json` foundation.
- Windows x64 and ARM64 self-contained release workflow.

## Ninite
Ninite is integrated as a first-class Programs Library entry. The app opens the official Ninite selector instead of redistributing Ninite's installer. This keeps the project from bundling third-party proprietary installers while still giving the user one-click access to Ninite.

## Release packages
`.github/workflows/release.yml` builds self-contained `win-x64` and `win-arm64` ZIP packages. Pushing a version tag such as `v1.0.0` creates a GitHub Release and attaches both packages. The workflow can also be started manually from GitHub Actions to produce downloadable build artifacts.

## Safety
The app must not silently disable security software, Windows Update, firewall protections, Defender, or other critical protections. It must not automatically apply unsafe voltage/clock changes. Hardware-tuning utilities are integrations/launchers; users remain in control of overclocking.

Third-party installers are not redistributed by this project. The Programs Library points to official vendor pages and Ninite rather than bundling their binaries.

## Build locally
Install the .NET 8 SDK on Windows and run `BUILD_WINDOWS.ps1` or:

`dotnet publish src/WindowsOptimizer/WindowsOptimizer.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true`

## Documentation
- [Architecture](docs/ARCHITECTURE.md)
- [Windows 11 build/setup](docs/BUILD-WINDOWS.md)
- [Safety](docs/SAFETY.md)
- [Programs Library](docs/PROGRAMS-LIBRARY.md)
- [Optimization profiles](docs/OPTIMIZATION.md)
- [Portability](docs/PORTABILITY.md)
