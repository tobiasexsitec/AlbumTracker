# AlbumTracker

A Blazor WebAssembly progressive web app for tracking your music listening habits. Search for albums, organize them into custom lists, rate them, and keep a detailed listen history -- all running entirely in your browser.

![.NET 10](https://img.shields.io/badge/.NET-10-512bd4)
![Blazor WASM](https://img.shields.io/badge/Blazor-WebAssembly-7b2fa0)
![PWA](https://img.shields.io/badge/PWA-enabled-5a0fc8)

## Features

- **Album Search** -- Search for albums by name or artist, powered by the [MusicBrainz](https://musicbrainz.org/) API with cover art from the [Cover Art Archive](https://coverartarchive.org/).
- **Custom Lists** -- Create, rename, and delete personal album lists. A built-in "Latest Played" list is automatically maintained.
- **Star Ratings** -- Rate albums from 1-5 stars with a single click.
- **Listen History** -- Log when you listened to an album (today, yesterday, or a custom date) and browse a paginated play history.
- **Album Detail Panel** -- View tracklists, ratings, listen stats, and manage list membership from a slide-out detail panel.
- **Offline-Ready PWA** -- Installable as a progressive web app with service worker support.
- **Client-Side Storage** -- All user data (lists, ratings, history) is persisted in the browser's local storage -- no backend required.

## Project Structure

The solution contains three projects:

- **AlbumTracker** (`AlbumTracker.csproj`) -- The main Blazor WebAssembly app.
- **AlbumTracker.MusicBrainz** -- Client library for the MusicBrainz API.
- **AlbumTracker.Spotify** -- Client library for the Spotify API (included for future extensibility).

Key folders inside the main project:

- **Components/** -- Reusable Blazor components (`AlbumCard`, `AlbumDetailPanel`, `AlbumCoverImage`, `StarRating`, `ListenTracker`, `TrackList`).
- **Pages/** -- Routable pages (`Home`, `Search`, `MyLists`, `ListDetail`).
- **Layout/** -- App layout and navigation.
- **Models/** -- Core domain models (`Album`, `AlbumList`, `AlbumRating`, `AlbumListenHistory`).
- **Services/** -- Service interfaces and browser local storage implementations.
- **wwwroot/** -- Static assets, CSS, and PWA manifest.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

### Run the App

```bash
# Clone the repository
git clone https://github.com/tobiasexsitec/AlbumTracker.git
cd AlbumTracker

# Restore dependencies and run
dotnet run
```

The app will start and be available at the URL shown in the console (typically `https://localhost:5001` or `http://localhost:5000`).

### Build for Production

```bash
dotnet publish -c Release
```

The published output will be in `bin/Release/net10.0/publish/wwwroot` and can be deployed to any static file host (GitHub Pages, Azure Static Web Apps, Netlify, etc.).

## Configuration

### MusicBrainz

The MusicBrainz client is configured in `Program.cs`. The MusicBrainz API requires a descriptive `User-Agent` header, which is set via options:

```csharp
builder.Services.AddMusicBrainz(options =>
{
    options.AppName = "AlbumTracker";
    options.AppVersion = "1.0";
    options.AppContact = "your@email.com";
});
```

Update `AppContact` with your own email to comply with the [MusicBrainz API usage guidelines](https://musicbrainz.org/doc/MusicBrainz_API#User-Agent).

### Spotify (Optional)

A Spotify integration library is included for future extensibility. To use it, configure your Spotify API credentials and swap the search service implementation.

### Authentication (Optional)

OIDC authentication scaffolding is included but not yet configured. See the comments in `Program.cs` to set up a provider (e.g., Google).

## Tech Stack

| Layer | Technology |
|---|---|
| **Frontend** | [Blazor WebAssembly](https://learn.microsoft.com/aspnet/core/blazor/) (.NET 10) |
| **Music Data** | [MusicBrainz API](https://musicbrainz.org/doc/MusicBrainz_API) + [Cover Art Archive](https://coverartarchive.org/) |
| **Persistence** | Browser Local Storage |
| **PWA** | Service Worker with offline caching |
| **Auth** | OIDC (scaffolded, not configured) |

## License

This project is open source. See the repository for license details.
