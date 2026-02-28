; =============================================================================
; LanOra Inno Setup Script
; =============================================================================
; Application  : LanOra – LAN Screen Sharing
; Publisher    : Jayesh Karemore
; Target OS    : Windows 7 SP1 and later
; .NET Req.    : .NET Framework 4.6.2
; Compression  : LZMA2 solid
; =============================================================================
;
; HOW TO COMPILE
; --------------
; 1. Install Inno Setup 6 from https://jrsoftware.org/isinfo.php
; 2. Open this file in the Inno Setup IDE, or run from the command line:
;      iscc /DMyAppVersion=1.0.0 LanOra.iss
; 3. The compiled installer is placed in the Output\ sub-directory.
;
; HOW TO EMBED VERSION AUTOMATICALLY (CI / build pipeline)
; ---------------------------------------------------------
; Pass the version on the command line with the /D switch:
;   iscc /DMyAppVersion=%BUILD_VERSION% LanOra.iss
;
; PLACEHOLDER PATHS – replace before compiling
; ---------------------------------------------
;   Source\LanOra.exe        – compiled main application
;   Source\Updater.exe       – compiled auto-updater
;   Source\*                 – any additional runtime files
;   Assets\LanOra.ico        – application icon (optional, uncomment below)
;   license.txt              – present in this Installer\ directory
; =============================================================================

; ---------------------------------------------------------------------------
; Version define (override on command line: iscc /DMyAppVersion=1.2.0 ...)
; ---------------------------------------------------------------------------
#ifndef MyAppVersion
  #define MyAppVersion "1.0.0"
#endif

; ---------------------------------------------------------------------------
; Application metadata defines
; ---------------------------------------------------------------------------
#define MyAppName        "LanOra"
#define MyAppPublisher   "Jayesh Karemore"
#define MyAppURL         "https://github.com/Jayesh435/LanOra"
#define MyAppExeName     "LanOra.exe"
#define MyAppUpdaterName "Updater.exe"
#define MyAppGUID        "{{C3D4E5F6-A7B8-9012-CDEF-012345678902}"

; ===========================================================================
[Setup]
; ---------------------------------------------------------------------------
; Identity & versioning
; ---------------------------------------------------------------------------
AppId={#MyAppGUID}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription={#MyAppName} Setup
VersionInfoProductName={#MyAppName}

; ---------------------------------------------------------------------------
; Installation paths
; ---------------------------------------------------------------------------
; Installs to C:\Program Files\LanOra (or Program Files (x86) on 32-bit)
DefaultDirName={commonpf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=no

; ---------------------------------------------------------------------------
; License agreement – shown before the installation begins
; ---------------------------------------------------------------------------
LicenseFile=license.txt

; ---------------------------------------------------------------------------
; Output
; ---------------------------------------------------------------------------
OutputDir=Output
OutputBaseFilename=LanOra_Setup_{#MyAppVersion}

; ---------------------------------------------------------------------------
; Compression – LZMA2 solid block gives the best compression ratio
; ---------------------------------------------------------------------------
Compression=lzma2/ultra64
SolidCompression=yes
LZMAUseSeparateProcess=yes

; ---------------------------------------------------------------------------
; Visual / icon  (uncomment when icon file is available)
; ---------------------------------------------------------------------------
; SetupIconFile=Assets\LanOra.ico
; WizardImageFile=Assets\WizardBanner.bmp
; WizardSmallImageFile=Assets\WizardSmall.bmp

; ---------------------------------------------------------------------------
; Privileges – admin required for Program Files and firewall rules
; ---------------------------------------------------------------------------
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=commandline

; ---------------------------------------------------------------------------
; Minimum OS: Windows 7 SP1 (6.1.7601)
; ---------------------------------------------------------------------------
MinVersion=6.1.7601

; ---------------------------------------------------------------------------
; Uninstaller appearance
; ---------------------------------------------------------------------------
UninstallDisplayName={#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}
CreateUninstallRegKey=yes

; ---------------------------------------------------------------------------
; Misc
; ---------------------------------------------------------------------------
ShowLanguageDialog=no
; Install native binaries in the correct Program Files folder on x64 Windows
ArchitecturesInstallIn64BitMode=x64

; ===========================================================================
[Languages]
; English messages (compiler built-in)
Name: "english"; MessagesFile: "compiler:Default.isl"

; ===========================================================================
[Tasks]
; ---------------------------------------------------------------------------
; Optional tasks shown to the user in the "Select Additional Tasks" wizard
; page.  Each task maps to a name used in [Icons] / [Run] via Tasks: key.
; ---------------------------------------------------------------------------

; Create a desktop shortcut (unchecked by default)
Name: "desktopicon"; \
  Description: "{cm:CreateDesktopIcon}"; \
  GroupDescription: "{cm:AdditionalIcons}"; \
  Flags: unchecked

; Add LanOra to the Windows Startup folder so it launches at logon
Name: "autostart"; \
  Description: "Launch {#MyAppName} automatically when Windows starts"; \
  GroupDescription: "Startup options:"; \
  Flags: unchecked

; Offer to launch the application immediately after install (checked by default)
Name: "launchapp"; \
  Description: "Launch {#MyAppName} after installation"; \
  GroupDescription: "After installation:"; \
  Flags: checkedonce

; ===========================================================================
[Files]
; ---------------------------------------------------------------------------
; Application files copied to {app} (= Program Files\LanOra by default).
; Place all compiled output in a Source\ sub-directory next to this script.
; ---------------------------------------------------------------------------

; Main executable
Source: "Source\{#MyAppExeName}";     DestDir: "{app}"; Flags: ignoreversion

; Auto-updater executable
Source: "Source\{#MyAppUpdaterName}"; DestDir: "{app}"; Flags: ignoreversion

; All remaining files in Source\ (DLLs, config files, resources, etc.)
Source: "Source\*"; \
  DestDir: "{app}"; \
  Excludes: "{#MyAppExeName},{#MyAppUpdaterName}"; \
  Flags: ignoreversion recursesubdirs createallsubdirs

; ===========================================================================
[Icons]
; ---------------------------------------------------------------------------
; Start Menu group + optional Desktop shortcut + optional Startup shortcut
; ---------------------------------------------------------------------------

; Start Menu: launch application
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

; Start Menu: uninstall
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"

; Desktop shortcut (only when user selects the "desktopicon" task above)
Name: "{commondesktop}\{#MyAppName}"; \
  Filename: "{app}\{#MyAppExeName}"; \
  Tasks: desktopicon

; Windows Startup folder (only when user selects the "autostart" task above)
Name: "{userstartup}\{#MyAppName}"; \
  Filename: "{app}\{#MyAppExeName}"; \
  Tasks: autostart

; ===========================================================================
[Registry]
; ---------------------------------------------------------------------------
; Store install metadata so the updater and other tools can locate the
; installation at runtime without relying on the exe path.
; ---------------------------------------------------------------------------
Root: HKLM; \
  Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; \
  ValueType: string; ValueName: "InstallDir"; ValueData: "{app}"; \
  Flags: uninsdeletekey

Root: HKLM; \
  Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; \
  ValueType: string; ValueName: "Version"; ValueData: "{#MyAppVersion}"

; ===========================================================================
[Run]
; ---------------------------------------------------------------------------
; Steps executed at the end of a successful installation (before the finish
; wizard page).  All netsh commands run hidden (no console window shown).
; ---------------------------------------------------------------------------

; Firewall rule: allow inbound TCP on port 5000 (screen-streaming)
Filename: "{sys}\netsh.exe"; \
  Parameters: "advfirewall firewall add rule name=""LanOra TCP 5000"" dir=in action=allow protocol=TCP localport=5000 enable=yes profile=any"; \
  Flags: runhidden; \
  StatusMsg: "Configuring firewall (TCP 5000)..."

; Firewall rule: allow inbound UDP on port 5001 (host discovery broadcast)
Filename: "{sys}\netsh.exe"; \
  Parameters: "advfirewall firewall add rule name=""LanOra UDP 5001"" dir=in action=allow protocol=UDP localport=5001 enable=yes profile=any"; \
  Flags: runhidden; \
  StatusMsg: "Configuring firewall (UDP 5001)..."

; Launch application post-install (only when the "launchapp" task is checked)
Filename: "{app}\{#MyAppExeName}"; \
  Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; \
  Flags: nowait postinstall skipifsilent; \
  Tasks: launchapp

; ===========================================================================
[UninstallRun]
; ---------------------------------------------------------------------------
; Remove the firewall rules added during installation
; ---------------------------------------------------------------------------
Filename: "{sys}\netsh.exe"; \
  Parameters: "advfirewall firewall delete rule name=""LanOra TCP 5000"""; \
  Flags: runhidden

Filename: "{sys}\netsh.exe"; \
  Parameters: "advfirewall firewall delete rule name=""LanOra UDP 5001"""; \
  Flags: runhidden

; ===========================================================================
[UninstallDelete]
; ---------------------------------------------------------------------------
; Remove runtime directories that the uninstaller would otherwise leave behind
; ---------------------------------------------------------------------------
Type: filesandordirs; Name: "{commonappdata}\LanOra\Logs"
Type: filesandordirs; Name: "{app}\UpdateTemp"
Type: filesandordirs; Name: "{app}\Backup"

; ===========================================================================
[Code]
// ---------------------------------------------------------------------------
// Pascal Script: pre-installation .NET Framework check
// ---------------------------------------------------------------------------
// Reads the "Release" DWORD under HKLM\SOFTWARE\Microsoft\NET Framework Setup
// \NDP\v4\Full.  A value >= 394802 means .NET 4.6.2 is installed on Windows 7
// SP1 / Windows Server 2008 R2.
// Reference:
//   https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/
//     how-to-determine-which-versions-are-installed
// ---------------------------------------------------------------------------

function IsDotNet462Installed(): Boolean;
var
  release: Cardinal;
begin
  Result :=
    RegQueryDWordValue(
      HKLM,
      'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full',
      'Release',
      release)
    and (release >= 394802);
end;

// Called by Inno Setup before the installation wizard begins.
// Returning False aborts the setup process.
function InitializeSetup(): Boolean;
begin
  if not IsDotNet462Installed() then
  begin
    MsgBox(
      '{#MyAppName} requires Microsoft .NET Framework 4.6.2 or later.' + #13#10 +
      #13#10 +
      'Your system does not have a compatible version of .NET Framework.' + #13#10 +
      'Please download and install .NET Framework 4.6.2 from:' + #13#10 +
      #13#10 +
      '  https://dotnet.microsoft.com/download/dotnet-framework/net462' + #13#10 +
      #13#10 +
      'The installation will now abort.',
      mbCriticalError,
      MB_OK);
    Result := False;
  end
  else
    Result := True;
end;
