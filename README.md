# LanOra – LAN Screen Monitoring Software

A professional, fully-offline LAN screen monitoring tool built in **C# / .NET Framework 4.5 / WinForms**.  
Works on **Windows 7 and above**. No internet. No external libraries. Pure TCP over a local network.

---

## Architecture

```
┌─────────────────────────┐          TCP Port 5000         ┌─────────────────────────┐
│   LANMonitor.Server     │  ──────────────────────────►   │   LANMonitor.Viewer     │
│  (Target / Host PC)     │        JPEG frame stream        │  (Monitoring PC)        │
│                         │                                  │                         │
│  • Detects own LAN IP   │                                  │  • Enter server IP      │
│  • Accepts 1 client     │                                  │  • Connect / Disconnect │
│  • Captures screen      │                                  │  • Live PictureBox feed │
│  • Resizes to 1280×720  │                                  │                         │
│  • JPEG 50% quality     │                                  │                         │
│  • Sends frame length   │                                  │                         │
│    + JPEG bytes         │                                  │                         │
└─────────────────────────┘                                  └─────────────────────────┘
```

### Frame Wire Format

```
[4 bytes – Int32 frame length][N bytes – JPEG image data] … repeated
```

### Authentication

On connect the viewer sends a **password string** first.  
The server replies with a **bool** (`true` = authenticated, `false` = rejected).  
Default password: `lanora123` (defined in both `ScreenServer.cs` and `ScreenClient.cs`).

---

## Folder Structure

```
LANMonitor.sln
├── LANMonitor.Server/
│   ├── Forms/
│   │   ├── MainForm.cs           – UI code-behind
│   │   ├── MainForm.Designer.cs  – Designer-generated layout
│   │   └── MainForm.resx
│   ├── Networking/
│   │   └── ScreenServer.cs       – TcpListener, auth, frame send loop
│   ├── Utilities/
│   │   └── ScreenCapture.cs      – Graphics.CopyFromScreen + JPEG compression
│   ├── Properties/
│   │   └── AssemblyInfo.cs
│   ├── Program.cs
│   └── LANMonitor.Server.csproj
└── LANMonitor.Viewer/
    ├── Forms/
    │   ├── MainForm.cs           – UI code-behind
    │   ├── MainForm.Designer.cs  – Designer-generated layout
    │   └── MainForm.resx
    ├── Networking/
    │   └── ScreenClient.cs       – TcpClient, auth, frame receive loop
    ├── Properties/
    │   └── AssemblyInfo.cs
    ├── Program.cs
    └── LANMonitor.Viewer.csproj
```

---

## Build Instructions

### Requirements

| Tool | Version |
|------|---------|
| Visual Studio | 2013 / 2015 / 2017 / 2019 / 2022 |
| .NET Framework | **4.5** (included in Windows 8+; downloadable for Win 7) |
| MSBuild | Comes with Visual Studio |

### Build in Visual Studio

1. Open `LANMonitor.sln`.
2. Select **Build → Build Solution** (or press `Ctrl+Shift+B`).
3. Executables are placed in:
   - `LANMonitor.Server\bin\Debug\LANMonitor.Server.exe`
   - `LANMonitor.Viewer\bin\Debug\LANMonitor.Viewer.exe`

### Build via MSBuild (command line)

```bat
msbuild LANMonitor.sln /p:Configuration=Release
```

Output ends up in each project's `bin\Release\` folder.

---

## Deployment on Windows 7

1. **Install .NET Framework 4.5** on the target machine if not already present:  
   Download from Microsoft: https://www.microsoft.com/en-us/download/details.aspx?id=30653

2. Copy `LANMonitor.Server.exe` to the **host PC** (the machine whose screen will be shared).

3. Copy `LANMonitor.Viewer.exe` to the **monitoring PC**.

4. Ensure **TCP port 5000** is allowed through the Windows Firewall on the server machine:
   ```
   netsh advfirewall firewall add rule name="LANMonitor Server" dir=in action=allow protocol=TCP localport=5000
   ```

### Usage

**On the Server PC:**
1. Run `LANMonitor.Server.exe`.
2. Note the displayed IP address (e.g. `192.168.1.10`).
3. Click **Start Server**.

**On the Viewer PC:**
1. Run `LANMonitor.Viewer.exe`.
2. Enter the server IP address in the text box.
3. Click **Connect**.

---

## Single EXE Installer

For the simplest deployment, use **ILMerge** or Visual Studio's publish feature, or simply
copy the single `.exe` file (it has no external DLL dependencies).

### Using ILMerge (optional)

```bat
ILMerge.exe /out:LANMonitor.Server.merged.exe LANMonitor.Server.exe
ILMerge.exe /out:LANMonitor.Viewer.merged.exe LANMonitor.Viewer.exe
```

---

## Changing the Password

Both the server and viewer share the same hardcoded password (`lanora123`).  
To change it, update **both** constants before building:

| File | Constant |
|------|----------|
| `LANMonitor.Server/Networking/ScreenServer.cs` | `public const string Password = "lanora123";` |
| `LANMonitor.Viewer/Networking/ScreenClient.cs` | `public const string Password = "lanora123";` |

---

## Performance Tuning

| Setting | Location | Default |
|---------|----------|---------|
| Capture interval (ms) | `ScreenServer.CaptureIntervalMs` | 100 ms (10 fps) |
| JPEG quality (0-100) | `ScreenCapture.JpegQuality` | 50 |
| Target resolution | `ScreenCapture.TargetWidth/Height` | 1280 × 720 |
| Connect timeout (ms) | `ScreenClient.ConnectTimeoutMs` | 5000 |

Lowering `CaptureIntervalMs` increases frame rate but uses more CPU and bandwidth.

---

## Security Notes

- Password is transmitted in plain text over TCP. This is intentional for a LAN-only tool.
- For production use, consider wrapping the stream with `SslStream`.
- The server accepts **one client at a time**; additional connections are queued by the OS and served after the current client disconnects.

---

## Roadmap (future enhancements)

- [ ] Multi-client support
- [ ] Screen resolution selector
- [ ] System tray minimize / auto-start with Windows
- [ ] Auto LAN device discovery (UDP broadcast)
- [ ] Remote keyboard/mouse control (RemoteDesktop mode)