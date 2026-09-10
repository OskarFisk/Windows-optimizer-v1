# ⚡ Windows Optimizer v1

A modern, modular Windows performance, maintenance, debloat and software toolkit — built to make optimization safer, clearer and easier.

## 📦 Download

**Windows setup:** GitHub Actions builds `WindowsOptimizerSetup.exe` automatically from `installer/windows/WindowsOptimizerSetup.iss`.

- **Setup.exe:** normal Windows installer for x64-compatible PCs.
- **Portable x64:** self-contained ZIP.
- **Portable ARM64:** self-contained ZIP.

Open the repository's **Actions → Windows Optimizer releases** workflow to download the latest successful build artifact. Version tags (`v*`) also create a GitHub Release with the generated packages.

## ✨ What it includes

- Modern dark performance-cockpit UI.
- CPU/process, memory, system-drive and uptime monitoring.
- Safe temporary-file cleanup.
- DNS cache maintenance with UAC elevation when required.
- Conservative application-memory cleanup.
- Windows Update shortcut.
- Programs Library with the supplied master software catalog.
- Package modes: Mini, Standard, Full, Gaming + Overclocking, Video + Photo and Manual.
- Ninite integration through the official Ninite selector.
- Launchers/integrations for MSI Afterburner, AMD Ryzen Master and BlueStacks.
- Windows x64 and ARM64 self-contained builds.
- Automated Windows `setup.exe` generation.

## 🧰 Setup installer

The installer is designed to install the application itself, create Start Menu shortcuts and optionally create a desktop shortcut.

The application is published **self-contained with .NET 8**, so users do **not** need to install the .NET runtime separately. The setup performs a safe Windows-version check and detects `winget` for optional software installation through the Programs Library.

Third-party proprietary programs are not silently bundled or redistributed. If a selected application requires its own installer or license, Windows Optimizer should use an official publisher source or Windows Package Manager where appropriate and with user confirmation.

## 📚 Programs Library

The repository contains the supplied desktop-software master list and a categorized program catalog. The selector supports searching, category filtering, package filtering, tags and selecting individual programs.

### Packages

| Package | Purpose |
|---|---|
| **Mini** | Low-memory core toolkit |
| **Standard** | Recommended everyday setup |
| **Full** | Full available catalog without fake/padding files |
| **Gaming + Overclocking** | Gaming, monitoring and tuning tools |
| **Video + Photo** | Creator, video, image and graphics tools |
| **Manual** | Choose individual programs |

## 🛡️ Safety first

Windows Optimizer must never silently:

- disable Defender, firewall or other security protections;
- disable Windows Update;
- delete critical Windows files;
- apply unsafe registry changes;
- apply dangerous voltage/clock settings;
- redistribute proprietary installers without permission.

Administrative elevation is requested only when an operation actually requires it.

## 🏗️ Build locally

Install the .NET 8 SDK on Windows:

```powershell
dotnet publish src/WindowsOptimizer/WindowsOptimizer.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

To build the Windows setup installer locally, install Inno Setup 6 and compile:

```powershell
ISCC.exe installer/windows/WindowsOptimizerSetup.iss
```

The generated installer is placed in `pkg/Windows/WindowsOptimizerSetup.exe`.

## 🤖 Automated builds

`.github/workflows/release.yml` builds on pushes to `main`, manual workflow runs and version tags. It produces:

- `WindowsOptimizerSetup.exe`
- `WindowsOptimizer-win-x64.zip`
- `WindowsOptimizer-win-arm64.zip`

A version tag such as `v1.0.0` additionally publishes the generated files to a GitHub Release.

## 📖 Documentation

- [Architecture](docs/ARCHITECTURE.md)
- [Windows build/setup](docs/BUILD-WINDOWS.md)
- [Safety](docs/SAFETY.md)
- [Programs Library](docs/PROGRAMS-LIBRARY.md)
- [Optimization profiles](docs/OPTIMIZATION.md)
- [Portability](docs/PORTABILITY.md)
- [Package output](pkg/README.md)

## 🚀 Project direction

The goal is a serious all-in-one desktop utility: optimization, cleanup, monitoring, debloat, software installation, gaming tools, creator tools and hardware utilities in one controlled interface — while keeping risky actions visible and reversible where possible.
