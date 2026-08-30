# HashTrack

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
![.NET Framework](https://img.shields.io/badge/.NET_Framework-4.8-512BD4?logo=dotnet&logoColor=white)
![VSTO](https://img.shields.io/badge/Outlook-VSTO_Add--in-0078D4?logo=microsoftoutlook&logoColor=white)
![C#](https://img.shields.io/badge/C%23-512BD4?logo=csharp&logoColor=white)

> Outlook has folders and categories. It doesn't have hashtags. HashTrack adds them —
> and makes them searchable across emails, appointments and tasks.

A Visual Studio Tools for Office (VSTO) add-in for Microsoft Outlook that lets you tag
Outlook items with hashtags and retrieve them later with a fast, indexed search. Built as
my bachelor thesis at VŠB – Technical University of Ostrava.

<!--
SCREENSHOT — add one of the sidebar in Outlook to docs/screenshots/sidebar.png and uncomment:
<p align="center"><img src="docs/screenshots/sidebar.png" width="70%" alt="HashTrack sidebar in Outlook"></p>
-->

## Features

- **Hashtag search** — find Outlook items by hashtag across emails, appointments and tasks.
- **Custom tagging** — attach hashtags to any Outlook artefact.
- **Indexed lookups** — a dedicated indexing service keeps search fast on large mailboxes.
- **Tag clustering** — related tags are grouped by a similarity classifier, so near-miss
  tags (`#invoice` / `#invoices`) still surface relevant results.
- **Seamless integration** — lives in the Outlook sidebar, no separate application.

## Architecture

The solution is deliberately split into layers with dependencies pointing inward, so the
domain has no knowledge of Outlook, WPF or the storage engine.

```
┌────────────────────────────────────────────────────────────┐
│ UI  (WPF sidebar + ViewModels, ThisAddIn VSTO entry point) │
└───────────────────────────┬────────────────────────────────┘
                            │  depends on abstractions only
┌───────────────────────────▼────────────────────────────────┐
│ HashTrack.BusinessLogic                                    │
│   SearchService · IndexingService · CategoryManagerService │
│   EventAggregator · ClusteringClassifier                   │
│   SearchCompleteHandlerFactory → *CompleteHandler          │
└───────────────────────────┬────────────────────────────────┘
┌───────────────────────────▼────────────────────────────────┐
│ HashTrack.Persistence                                      │
│   PersistantStorage · CachedPersistenceStorage (decorator) │
│   Cache · PersistenceHashTagService                        │
└───────────────────────────┬────────────────────────────────┘
┌───────────────────────────▼────────────────────────────────┐
│ HashTrack.Core   ← no outward dependencies                 │
│   Interfaces (Search, Indexing, Persistence, Clustering)   │
│   Models · Enums · Exceptions · Extensions · Helpers       │
└────────────────────────────────────────────────────────────┘

HashTrack.IoC — Autofac container registration + composition root (Startup.cs)
```

**Patterns used and why**

| Pattern | Where | Why |
|---|---|---|
| Dependency injection (Autofac) | `HashTrack.IoC/Startup.cs` | Layers depend on interfaces from `Core`, wired in one place |
| Decorator | `CachedPersistenceStorage` wraps `PersistantStorage` | Caching added without touching storage logic |
| Factory | `SearchCompleteHandlerFactory` | Picks the right completion handler per search mode |
| Event aggregator | `EventAggregator` | Decouples the WPF sidebar from services — no direct references |
| Repository / storage abstraction | `IStorage`, `IPersistenceHashTagService` | The persistence engine is swappable |

## Tech stack

- C# / .NET Framework 4.8, VSTO for Outlook
- WPF (MVVM) for the sidebar UI
- Autofac for dependency injection
- SQLite for local persistence
- SimMetrics.Net for tag similarity / clustering

## Getting started

### Prerequisites

- Visual Studio 2022 with the **Office/SharePoint development** workload
- .NET Framework 4.8 developer pack
- Microsoft Outlook (desktop)

### Build & run

```bash
git clone https://github.com/JaKuBisz/HashTrack.git
```

1. Open `HashTrack.sln` in Visual Studio.
2. Restore NuGet packages.
3. Set `HashTrack` as the startup project and press F5 — Visual Studio launches Outlook
   with the add-in registered for debugging.

> The add-in creates its SQLite database locally on first run; no database file is
> committed to the repository.

## Project layout

```
HashTrack/
├── HashTrack.Core/          domain models, interfaces, enums, extensions
├── HashTrack.BusinessLogic/ search, indexing, clustering, event aggregation
├── HashTrack.Persistence/   SQLite storage, caching decorator
├── HashTrack.IoC/           Autofac registration + composition root
├── UI/                      WPF sidebar, ViewModels, services
├── Startup/                 add-in bootstrap
└── ThisAddIn.cs             VSTO entry point
```

## Status

Completed as a bachelor thesis project (2024) and published as a portfolio / reference
implementation. Not under active development — issues and PRs are welcome but may not
receive a fast response.

## License

[MIT](LICENSE) © Jakub Kubisz

## Acknowledgements

Bachelor thesis at VŠB – Technical University of Ostrava. Thanks to my supervisor and
peers for their guidance.
