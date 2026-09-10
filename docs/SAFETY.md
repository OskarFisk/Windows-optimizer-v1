# Safety Policy

Windows optimization can make a machine less reliable if changes are too aggressive. This project follows a conservative policy.

## Required behavior
- Show exactly what a change will do before applying it.
- Group changes by risk: Safe, Caution, Advanced.
- Offer a restore point before system-level changes when Windows allows it.
- Keep an application log of privileged actions.
- Provide rollback instructions whenever a change is reversible.
- Preserve user exclusions.
- Never require disabling Defender, firewall, Windows Update or UAC.
- Never delete unknown system files.
- Never run arbitrary commands from a remote source.

## Overclocking
MSI Afterburner, AMD Ryzen Master, Intel tuning utilities and other hardware tools may be listed in the Programs Library and launched from the app. The optimizer must not automatically set unsafe voltage, power-limit or frequency values.

## Debloating
Debloat candidates must be explicitly categorized. Critical Windows components and dependencies must be excluded by default. Removing a package must never be presented as guaranteed to improve FPS.

## RAM cleaning
RAM cleaning should focus on reclaimable caches and safe OS-supported mechanisms. It must not kill random system processes or claim that unused RAM is inherently a problem.
