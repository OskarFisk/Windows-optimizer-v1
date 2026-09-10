# Windows Optimizer v1 — downloadable packages

This folder is the destination for generated installer packages.

## Windows

The GitHub Actions release workflow builds:

- `WindowsOptimizerSetup.exe` — normal Windows setup installer for x64-compatible systems.
- `WindowsOptimizer-win-x64.zip` — portable/self-contained x64 build.
- `WindowsOptimizer-win-arm64.zip` — portable/self-contained ARM64 build.

`WindowsOptimizerSetup.exe` is generated during CI from `installer/windows/WindowsOptimizerSetup.iss`; the binary is intentionally not committed to Git because generated binaries belong in GitHub Releases/artifacts.

## Dependencies

The Windows application is published self-contained with .NET 8, so the user does **not** need to install the .NET runtime manually. The setup checks the Windows version and whether `winget` is available for optional Programs Library installations.

Third-party proprietary applications are not silently bundled or redistributed. When a program requires its own installer or license, Windows Optimizer should download/install it through an official source or Windows Package Manager where legally and technically appropriate, with the user's confirmation.
