# Powershell History

PowerToys plugin which helps you in Your daily development routine.

## Usage

[tbd]

## Installation

1. Download the latest release of the Powershell History from the [Releases](https://github.com/hciupinski/powershell-history/releases).
2. Extract the zip file's contents to your PowerToys modules directory (usually `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`).
3. Restart PowerToys.

## Development

### Prepare

1. Copy `.dll` from the PowerToys Run folder to `.\Lib` (using symlink or running `copyLib.ps1` is recommended).
    - `PowerToys.Common.UI.dll`
    - `PowerToys.ManagedCommon.dll`
    - `PowerToys.Settings.UI.Lib.dll`
    - `Wox.Infrastructure.dll`
    - `Wox.Plugin.dll`

### How to debug

1. Build the project.
2. Run `debug.ps1`.
3. Attach to the process `PowerToys.PowerLauncher`.
