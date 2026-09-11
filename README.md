# ⚡ Windows Optimizer v1

A modern, modular Windows performance, maintenance, debloat and software toolkit — built to make optimization safer, clearer and easier.

## 📦 Download

### 🟢 Windows Installer — Recommended

[**⬇️ Download Windows Optimizer Setup.exe**](https://github.com/OskarFisk/Windows-optimizer-v1/releases/download/latest/WindowsOptimizerSetup.exe)

The button above always points to the **latest successful Windows x64 installer**. The installer is self-contained, requests Windows administrator permission through UAC, and creates the normal Start Menu/optional desktop shortcuts.

**Other packages:**

- [**Windows x64 Portable ZIP**](https://github.com/OskarFisk/Windows-optimizer-v1/releases/download/latest/WindowsOptimizer-win-x64.zip) — no installation required.
- **GitHub Actions artifact** — available from the latest successful `Windows Optimizer releases` workflow.
- **Versioned releases** — version tags (`v*`) publish their generated packages to GitHub Releases.

## ✨ What it includes

- Modern dark performance-cockpit UI.
- CPU/process, physical memory, system-drive and uptime monitoring.
- Live CPU and GPU temperature monitoring when supported sensors are available.
- Live fan RPM monitoring when supported hardware/driver sensors are available.
- Safe temporary-file cleanup.
- DNS cache maintenance with administrator elevation when required.
- Microsoft Sysinternals RAMMap standby-list cleanup.
- Windows Update shortcut.
- Programs Library with the supplied master software catalog.
- Package modes: Mini, Standard, Full, Gaming + Overclocking, Video + Photo and Manual.
- Ninite integration through the official Ninite selector.
- Launchers/integrations for MSI Afterburner, AMD Ryzen Master and BlueStacks.
- Windows x64 self-contained build.
- Automated Windows `setup.exe` generation.

## 🧰 Setup installer

The installer installs the application itself, creates Start Menu shortcuts and can create a desktop shortcut. Windows Optimizer requests administrator privileges at launch because several system-level features require elevated access.

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

Administrative elevation is used for the application because system-level features require it, while risky operations should still remain visible and user-controlled.

## 🏗️ Build locally

Install the .NET 8 SDK on Windows:

```powershell
dotnet publish src/WindowsOptimizer/WindowsOptimizer.csproj -c Release -r win-x64 --self-contained true
```

To build the Windows setup installer locally, install Inno Setup 6 and compile:

```powershell
ISCC.exe installer/windows/WindowsOptimizerSetup.iss
```

The generated installer is placed in `pkg/Windows/WindowsOptimizerSetup.exe`.

## 🤖 Automated builds

`.github/workflows/release.yml` builds on pushes to `main`, manual workflow runs and version tags. Every successful `main` build updates the **Windows Optimizer - Latest** GitHub Release, which powers the direct download buttons above.

Generated packages include:

- `WindowsOptimizerSetup.exe`
- `WindowsOptimizer-win-x64.zip`

A version tag such as `v1.0.0` additionally publishes the generated files to a versioned GitHub Release.

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
