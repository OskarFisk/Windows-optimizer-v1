# Architecture

## Recommended stack
- C# / .NET desktop application.
- WinUI 3 or WPF for the Windows interface.
- UAC elevation only for privileged actions.
- PowerShell scripts used only for reviewed, signed/controlled maintenance operations.
- Hardware monitoring through well-defined adapters so a failed sensor does not crash the app.

## Main modules
1. Dashboard — live CPU/GPU/RAM/disk/network and temperature overview.
2. Optimizer — preview, apply, verify and rollback-safe changes.
3. Debloater — categorized Windows components with risk labels and exclusions.
4. RAM Cleaner — conservative cleanup and memory-pressure information.
5. Startup Manager — startup programs and services with explanations.
6. Programs Library — searchable categorized catalog using official sources.
7. Hardware Tools — launch/integration pages for vendor utilities.
8. Monitor — component graphs, sensors and process/resource usage.
9. Settings — restore-point policy, language, theme, update checks and logging.
10. Logs — every privileged change is recorded with timestamp, action and result.

## Privilege model
Start unelevated. When an action requires administrator rights, request UAC elevation for that operation rather than running the entire UI as administrator. Never bypass UAC.
