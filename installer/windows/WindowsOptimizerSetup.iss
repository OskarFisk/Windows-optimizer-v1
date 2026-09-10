#define MyAppName "Windows Optimizer v1"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "OskarFisk"
#define MyAppExeName "WindowsOptimizer.exe"

[Setup]
AppId={{B2C1F0A4-6B8D-4A47-AF2B-0D7B0A9A0F01}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\Windows Optimizer v1
DefaultGroupName=Windows Optimizer v1
OutputDir=..\..\pkg\Windows
OutputBaseFilename=WindowsOptimizerSetup
Compression=lzma2/max
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
WizardStyle=modern
UninstallDisplayIcon={app}\{#MyAppExeName}
SetupLogging=yes
DisableProgramGroupPage=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "swedish"; MessagesFile: "compiler:Languages\Swedish.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Shortcuts:"
Name: "startmenu"; Description: "Create a Start Menu shortcut"; GroupDescription: "Shortcuts:"; Flags: checkedonce
Name: "autorun"; Description: "Launch Windows Optimizer after installation"; GroupDescription: "Finish:"

[Files]
Source: "..\..\publish\win-x64\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion
Source: "prerequisites.ps1"; DestDir: "{app}\installer"; Flags: ignoreversion
Source: "install-components.ps1"; DestDir: "{app}\installer"; Flags: ignoreversion
Source: "..\..\packages\package-selection.json"; DestDir: "{app}\packages"; Flags: ignoreversion skipifsourcedoesntexist
Source: "..\..\packages\program-catalog.json"; DestDir: "{app}\packages"; Flags: ignoreversion skipifsourcedoesntexist
Source: "..\..\packages\software-master-list.txt"; DestDir: "{app}\packages"; Flags: ignoreversion skipifsourcedoesntexist

[Icons]
Name: "{group}\Windows Optimizer v1"; Filename: "{app}\{#MyAppExeName}"; Tasks: startmenu
Name: "{autodesktop}\Windows Optimizer v1"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "powershell.exe"; Parameters: "-NoProfile -ExecutionPolicy Bypass -File ""{app}\installer\prerequisites.ps1"""; StatusMsg: "Checking Windows prerequisites..."; Flags: runhidden waituntilterminated
Filename: "powershell.exe"; Parameters: "-NoProfile -ExecutionPolicy Bypass -File ""{app}\installer\install-components.ps1"" -InstallDirectory ""{app}"""; StatusMsg: "Preparing required Windows components..."; Flags: runhidden waituntilterminated
Filename: "{app}\{#MyAppExeName}"; Description: "Launch Windows Optimizer v1"; Flags: nowait postinstall skipifsilent; Tasks: autorun

[UninstallDelete]
Type: filesandordirs; Name: "{app}\installer"
Type: filesandordirs; Name: "{app}\packages"

[Code]
function InitializeSetup(): Boolean;
begin
  Result := True;
end;
