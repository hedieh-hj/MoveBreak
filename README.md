# MoveBreak

MoveBreak is a calm, offline break-reminder application for Windows. It helps developers, office workers, students, and anyone who spends long hours at a computer remember to stand up, stretch, move, and rest their eyes.

The app runs quietly in the Windows System Tray, pauses its sitting timer when the user is away, and avoids showing disruptive reminders over full-screen applications.

## Features

- Configurable reminder interval and break duration
- Large countdown to the next break
- Start, pause, resume, restart, skip, and five-minute snooze controls
- 18 illustrated exercises for the neck, shoulders, back, wrists, legs, and eyes
- 20-20-20 eye-rest reminders
- A different exercise is shown immediately after skipping
- Automatic pause when Windows is locked or the user is idle
- Notification deferral while a full-screen application is active
- Windows System Tray integration
- Completed, skipped, and snoozed break history
- Daily sitting-time estimate and seven-day activity overview
- Optional notification sound
- Optional launch at Windows startup
- Light and dark themes
- Complete English, Persian, and Spanish interfaces with automatic LTR/RTL layout
- Local SQLite storage with no account or cloud service required

## Screens and Languages

English is the default language. The entire interface can be switched to Persian or Spanish from **Settings > Language**. Navigation, buttons, status messages, notifications, exercise names, and exercise instructions are updated immediately, and the selected language is saved for future launches.

## Technology

- [.NET 8](https://dotnet.microsoft.com/)
- WPF
- MVVM with `CommunityToolkit.Mvvm`
- Entity Framework Core
- SQLite
- Microsoft Dependency Injection
- Windows activity detection and System Tray integration

## Project Structure

```text
MoveBreak/
├── Assets/Exercises/    Offline exercise illustrations
├── Data/                EF Core database context and schema updates
├── Models/              Domain models and localized display models
├── Resources/           English and Persian resource dictionaries
├── Services/            Timer, activity, notification, settings, and localization services
├── ViewModels/          Presentation state and commands
└── Views/               WPF windows and controls
```

Timer logic, Windows activity monitoring, notifications, persistence, localization, and presentation code are kept separate so the application can evolve without tightly coupling platform and UI concerns.

## Requirements

- Windows 10 version 1809 or later
- .NET 8 SDK for building from source

## Run from Source

Clone the repository and run:

```powershell
git clone https://github.com/hedieh-hj/MoveBreak.git
cd MoveBreak
dotnet restore
dotnet run
```

Closing the main window keeps MoveBreak running in the System Tray. Use the tray menu to reopen, pause, resume, or exit the application.

## Build a Standalone Windows Version

To publish a self-contained 64-bit Windows build:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true
```

The published files will be available under:

```text
bin/Release/net8.0-windows10.0.17763.0/win-x64/publish/
```

## Local Data and Privacy

MoveBreak works completely offline. User preferences and break records are stored in a local SQLite database at:

```text
%LocalAppData%\MoveBreak\movebreak.db
```

No account is required, and the application does not upload activity information to a remote service.

## How Reminders Work

1. The active-work timer counts down while the user is working.
2. The timer pauses when Windows is locked or the configured idle threshold is reached.
3. When a break becomes due, MoveBreak checks whether a full-screen window is active.
4. At an appropriate moment, the app displays an illustrated exercise and a Windows notification.
5. The completed, skipped, or snoozed result is stored locally and reflected in the statistics.

## Roadmap

- Native interactive Windows Toast actions
- More exercise collections and filtering options
- Improved session and historical statistics
- Better detection of presentations and online meetings
- Accessibility improvements and keyboard navigation
- MSIX packaging, signing, and automated GitHub releases
- Automated tests for timer and reminder policies

## Contributing

Issues and pull requests are welcome. If you find a bug or have an idea for a useful desk-break feature, open an issue and describe the expected behavior and your Windows version.

## Wellness Notice

MoveBreak is a general wellness and productivity tool, not medical advice. Exercise gently, stop if a movement causes pain, and consult a qualified healthcare professional when appropriate.
