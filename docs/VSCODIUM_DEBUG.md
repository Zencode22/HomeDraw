# Debugging with VS Codium

This repository is configured so the projects can be built and debugged directly
from **VS Codium** (the open-source VS Code distribution) using the Run & Debug
panel — no proprietary Microsoft binaries required.

## Prerequisites

1. **.NET 8 SDK** available on your `PATH`
   Verify with: `dotnet --version`
2. **A C# extension from Open VSX** that provides a .NET debug adapter, e.g.
   `cs-dev` (recommended in [.vscode/extensions.json](.vscode/extensions.json)).
   VS Codium installs extensions from Open VSX; the standard
   `ms-dotnettools.csharp` extension does **not** work there because it ships
   proprietary components.
3. The debug configurations use adapter type `"netcoredbg"`. If your installed
   extension registers a different type (e.g. `"coreclr"`), change `"type"` in
   [.vscode/launch.json](.vscode/launch.json) accordingly.

## What's in `.vscode/`

| File | Purpose |
|---|---|
| [launch.json](.vscode/launch.json) | Debug configurations (see below) |
| [tasks.json](.vscode/tasks.json) | Build tasks run automatically before each debug launch (`preLaunchTask`) |
| [extensions.json](.vscode/extensions.json) | Recommends the Open VSX C# extension, blocks the MS-only one |

## Debug configurations (pick from the Run & Debug dropdown, then press F5)

- **Debug HomeDraw (MonoGame)** – builds `HomeDraw/HomeDraw.csproj` (this also
  runs the MonoGame content builder via `dotnet-tools.json`) and launches
  `bin/Debug/net8.0/HomeDraw.dll` under the debugger with an integrated terminal.
- **Debug TextureGenerator (console)** – builds and debugs the
  `TextureGenerator` console tool the same way.
- **Attach to HomeDraw process** – attach to an already-running game process
  (`${command:pickProcess}` shows a process picker).

Breakpoints, stepping, watch/variables and call stacks work as usual once the
matching C# extension is installed.

## Building without debugging

`Terminal → Run Build Task…` (Ctrl+Shift+B) runs **build solution**, which
builds both projects from `HomeDraw.slnx`. Individual project build tasks
(`build HomeDraw`, `build TextureGenerator`) are also available.
