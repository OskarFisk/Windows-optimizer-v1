# macOS

The project targets both Intel and Apple Silicon Macs where the shared runtime and required monitoring APIs are available.

macOS-specific maintenance must use macOS APIs and must not reuse Windows registry or service operations.

The application-library layer can integrate Homebrew when available, while official vendor downloads remain available for software that is not distributed through a package manager.

See [PORTABILITY.md](PORTABILITY.md).
