# Building on Windows 11

## Requirements
- Windows 11 64-bit desktop or laptop.
- .NET SDK compatible with the project's chosen target.
- Visual Studio 2022 with the required Windows desktop workload.
- Git.

## Development
1. Clone the repository.
2. Open the solution in Visual Studio.
3. Restore dependencies.
4. Build Debug.
5. Run the app without administrator rights first.
6. Test each privileged operation through its UAC path.

## Release checklist
- Build Release.
- Verify the application starts unelevated.
- Verify UAC is requested only when needed.
- Verify all optimization actions are previewed and logged.
- Verify the Programs Library uses official sources.
- Test on both a desktop and a laptop.
- Test with NVIDIA, AMD and Intel hardware where available.
