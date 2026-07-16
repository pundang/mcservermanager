[Setup]
AppName=MCServerManager
AppVersion=0.1.0.alpha.1
DefaultDirName={autopf}\MCServerManager
DefaultGroupName=MCServerManager
OutputDir=Output
OutputBaseFilename=MCServerManager-Setup
Compression=lzma
SolidCompression=yes
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

[Files]
Source: "..\publish\win-x64\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\MCServerManager"; Filename: "{app}\MCServerManager.Desktop.exe"
Name: "{autodesktop}\MCServerManager"; Filename: "{app}\MCServerManager.Desktop.exe"

[Run]
Filename: "{app}\MCServerManager.Desktop.exe"; Description: "Launch MCServerManager"; Flags: nowait postinstall skipifsilent
