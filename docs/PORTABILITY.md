# Cross-platform support

Windows Optimizer v1 is being designed as a **desktop application**, with a shared cross-platform UI/core and OS-specific adapters.

## Target operating systems

### Windows
- Windows 11 x64
- Windows 11 ARM64 where supported by the runtime and individual features
- Windows 10 x64 as a compatibility target where APIs permit
- Windows Server is not a primary target

Windows receives the deepest optimization support because the project is primarily a Windows optimizer.

### Linux
Support is designed around distributions rather than pretending every distribution has identical APIs:

- Ubuntu / Ubuntu-based distributions
- Debian
- Linux Mint
- Fedora
- RHEL-compatible distributions
- Arch Linux / Arch-based distributions
- openSUSE
- SteamOS where desktop-mode permissions and package availability allow it

The Linux adapter detects the distribution and only enables operations known to be compatible with that system.

### Other desktop OS
- macOS Intel
- macOS Apple Silicon
- FreeBSD where the required runtime and APIs are available

Other operating systems receive monitoring, application launching, diagnostics and safe maintenance features first. OS-specific debloating is enabled only when implemented and tested for that OS.

## Architecture

```text
Windows Optimizer
├── Shared UI
├── Shared core
│   ├── Hardware monitoring interfaces
│   ├── Process/service interfaces
│   ├── Program catalog
│   ├── Profiles
│   └── Audit/logging
└── OS adapters
    ├── Windows
    ├── Linux
    ├── macOS
    └── Other Unix-like systems
```

The shared layer must never assume Windows paths, registry APIs, PowerShell, systemd, launchd, or a particular Linux package manager. Those belong in platform adapters.

## Package management

The Programs Library uses a catalog of official vendor links and native package-manager integrations where practical. It does **not** redistribute third-party installers without permission.

Examples:
- Windows: winget when available
- Debian/Ubuntu: apt
- Fedora: dnf
- Arch: pacman
- openSUSE: zypper
- macOS: Homebrew when installed/appropriate

A package-manager operation must be previewable before execution.

## Privileges

The application should request elevated privileges only for an operation that requires them. Never run the entire application permanently as root or Administrator just because one feature needs elevation.

## Feature matrix

| Feature | Windows | Linux | macOS | Other |
|---|---|---|---|---|
| Hardware monitor | Deep | Deep/adapter | Adapter | Best effort |
| RAM cleanup | Yes | Yes | Yes | Best effort |
| Startup manager | Yes | Adapter | Adapter | Adapter |
| Debloat | Deep | Distro-specific | macOS-specific maintenance | Limited |
| Program library | Yes | Yes | Yes | Best effort |
| Gaming profile | Yes | Yes | Limited | Best effort |
| GPU tools | NVIDIA/AMD/Intel integrations | Vendor-specific | Vendor-specific | Vendor-specific |
| Overclock launchers | Yes | Vendor-specific | Limited | Limited |
| BlueStacks | Windows where supported | Not assumed | Not assumed | No |
| MSI Afterburner | Windows | No native assumption | No native assumption | No |
| Ryzen Master | Windows | No native assumption | No | No |

## Build philosophy

Every release should clearly state which OSes and architectures were actually tested. "Cross-platform" means the application has an adapter for a platform; it does not mean every optimization is identical on every OS.
