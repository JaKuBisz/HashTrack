# HashTrack

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
![.NET Framework](https://img.shields.io/badge/.NET_Framework-4.8-512BD4?logo=dotnet&logoColor=white)
![VSTO](https://img.shields.io/badge/Outlook-VSTO_Add--in-0078D4?logo=microsoftoutlook&logoColor=white)
![C#](https://img.shields.io/badge/C%23-512BD4?logo=csharp&logoColor=white)

> Outlook has folders and categories. It doesn't have hashtags. HashTrack adds them,
> and makes them searchable across emails, appointments and tasks.

A Visual Studio Tools for Office (VSTO) add-in for Microsoft Outlook that lets you tag
Outlook items with hashtags and retrieve them later with a fast, indexed search. Built as
my bachelor thesis at VŠB – Technical University of Ostrava.

<!--
THESIS PDF: export from Overleaf to docs/thesis.pdf, then uncomment.
Check the embedded assignment page first for a handwritten signature / student login.
📄 **Bachelor thesis:** [Hashtag Plugin for MS Outlook](docs/thesis.pdf) (Czech, VŠB-TUO 2024)
-->

**If you're here to look at the code**, the parts worth your time are the
[convention-based DI container](#convention-based-dependency-injection), which registers
services by scanning for attributes instead of maintaining a composition file, and the
[tag-clustering algorithm](#algorithms), which handles the case plain string similarity gets
wrong.

<!--
SCREENSHOT: add one of the sidebar in Outlook to docs/screenshots/sidebar.png and uncomment:
<p align="center"><img src="docs/screenshots/sidebar.png" width="70%" alt="HashTrack sidebar in Outlook"></p>
-->

## Features

- **Hashtag search.** Find Outlook items by hashtag across emails, appointments and tasks.
- **Custom tagging.** Attach hashtags to any Outlook artefact.
- **Indexed lookups.** A dedicated indexing service keeps search fast on large mailboxes.
- **Tag clustering.** Related tags are grouped by a similarity classifier, so near-miss
  tags (`#invoice` / `#invoices`) still surface relevant results.
- **Runs inside Outlook.** No separate application to keep open.

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

HashTrack.IoC    Autofac container registration + composition root (Startup.cs)
```

**Patterns used and why**

| Pattern | Where | Why |
|---|---|---|
| Dependency injection (Autofac) | `HashTrack.IoC/Startup.cs` | Layers depend on interfaces from `Core`, wired in one place |
| Decorator | `CachedPersistenceStorage` wraps `PersistantStorage` | Caching added without touching storage logic |
| Factory | `SearchCompleteHandlerFactory` | Picks the right completion handler per search mode |
| Event aggregator | `EventAggregator` | Decouples the WPF sidebar from services, no direct references |
| Repository / storage abstraction | `IStorage`, `IPersistenceHashTagService` | The persistence engine is swappable |

## Convention-based dependency injection

Rather than maintaining a growing list of `builder.RegisterType<X>().As<IX>()` calls, the
container **discovers its own registrations by scanning assemblies for marker attributes**.
Registering a new service means adding one attribute to the class. The composition root is
never edited.

```csharp
[RegisterService(LifeCycle.Transient, typeof(IClusteringClassifier))]
public class ClusteringClassifier : IClusteringClassifier { ... }

[RegisterService(LifeCycle.Singleton, typeof(IStorage))]
public class CachedPersistenceStorage : IStorage { ... }
```

The attributes form a small hierarchy, each adding one capability:

| Attribute | Adds | Used for |
|---|---|---|
| `RegisterServiceAttribute` | lifetime, explicit service type, open-generic flag | ordinary services |
| `RegisterKeyedAttribute` | a string key | multiple implementations of one interface |
| `RegisterHandlerAttribute` | key + fixed transient lifetime | search-completion handlers |

`Startup.ConfigureContainer` then runs three scanning pipelines over the loaded assemblies
(standard, keyed and open-generic) once per `LifeCycle` value:

```csharp
foreach (LifeCycle lifeCycle in Enum.GetValues(typeof(LifeCycle)))
{
    GetStandardServicesPipeline(builder, assemblies).PropertiesAutowired().ConfigureLifecycle(lifeCycle);
    GetKeyedServicesPipeline(builder, assemblies).PropertiesAutowired().ConfigureLifecycle(lifeCycle);
    GetOpenGenericServicesPipeline(builder, assemblies)...
}
```

Two details worth calling out:

- **Interface convention.** When `ServiceType` isn't given, the type binds to whichever
  interface it implements that is named `I{TypeName}`, so `SearchService` resolves as
  `ISearchService` with no configuration at all.
- **Open generics.** A separate pipeline registers open generic types, which is what allows
  `IRepository<T>` to be resolved for any entity without one registration per entity.

**Why it was worth building:** the add-in grew across four assemblies during development.
A central registration file would have been edited in every single feature commit and would
have been a constant source of merge conflicts and *"resolved at runtime, crashed in
production"* bugs. Attribute-driven registration keeps each service's wiring next to the
service itself. The Open/Closed principle applied to composition.

## Algorithms

### Hashtag clustering: why plain string similarity fails

Users don't type tags consistently: `#invoice`, `#invoices`, `#Invoce`. Naive edit-distance
similarity groups those correctly, but it also groups `#Sprint1` with `#Sprint2`, which are
**over 90% similar as strings and completely different in meaning**. Merging them would be
worse than no clustering at all.

`ClusteringClassifier` solves this by splitting each tag into a text part and a numeric part,
and treating the two differently:

```csharp
private const double SimilarityThreshold = 0.75;
private readonly IStringMetric _metric = new Levenstein();

public bool Classify(string text1, string text2)
{
    var tag1 = SplitTag(text1);
    var tag2 = SplitTag(text2);

    if (tag1.NumberPart != tag2.NumberPart) return false;      // digits must match exactly

    return _metric.GetSimilarity(tag1.TextPart, tag2.TextPart) > SimilarityThreshold;
}
```

- **Digits are compared exactly.** `#Sprint1` and `#Sprint2` can never be merged, because
  numbers in a tag almost always carry identity (sprint, ticket, invoice, year).
- **Text is compared fuzzily.** Levenshtein similarity above `0.75` absorbs typos and
  plural/singular drift.

How that plays out:

| Tag A | Tag B | Merged? | Why |
|---|---|---|---|
| `#invoice` | `#invoices` | ✅ yes | digits equal (none), text similarity 0.89 |
| `#bug` | `#bugs` | ✅ yes | text similarity 0.80, just over the threshold |
| `#report2024` | `#reports2024` | ✅ yes | digits both `2024`, text similarity 0.88 |
| `#Sprint1` | `#Sprint2` | ❌ no | digits differ, rejected before similarity is even computed |
| `#invoice` | `#invoice2024` | ❌ no | digits differ (none vs `2024`) |

The threshold matters: `#bug`/`#bugs` scores `0.80`, so anything above `0.80` would stop
merging simple plurals. It is currently hardcoded and is the one value most worth exposing
as configuration.

The overload taking two `HashTagModel`s adds a second rule that makes clustering
**directional**:

```csharp
if (tag1.NumOfOccurrences <= tag2.NumOfOccurrences) return false;
```

A tag only absorbs another if it is used *more often*, so the most frequently used spelling
becomes the canonical one and the relation can't cycle between two variants.

### Dispatching Outlook's asynchronous search results

Outlook's `AdvancedSearch` doesn't return results. It fires one global `AdvancedSearchComplete`
event for *every* search in the application, so a naive implementation delivers every result
to every consumer.

The dispatch key is the `Tag` that Outlook carries on the search object, which is used
directly as the DI resolution key:

```csharp
var handler = _context.ResolveKeyed<ISearchCompleteHandler>(searchObject.Tag);
handler.HandleSearchComplete(searchObject);
```

If no handler is registered for that tag, the factory falls back to publishing through the
`EventAggregator`, so UI components can subscribe to searches they own without the business
layer knowing they exist. Adding a new search mode is therefore one class with a
`[RegisterHandler("myTag")]` attribute, with no changes to the dispatcher.

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
3. Set `HashTrack` as the startup project and press F5. Visual Studio launches Outlook
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

## Known limitations & what I'd do differently

Written after the fact, with the benefit of distance from the deadline:

- **`CachedPersistenceStorage` is currently cache-only.** The write-through calls to
  `PersistantStorage` are commented out, so it behaves as an in-memory
  `ConcurrentDictionary` rather than the caching decorator the structure implies. The shape
  is right; the delegation needs re-enabling and a cache-invalidation policy behind it.
- **`ServiceLocator` is exposed as a static.** Convenient inside a VSTO add-in, where the
  framework, not the container, owns object construction, but it is still service location
  rather than injection. I'd limit it strictly to the VSTO entry point today.
- **`GetAssemblies()` scans everything in the AppDomain.** Simple and it works, but it means
  registration cost scales with every assembly Outlook happens to have loaded. Explicitly
  listing the four HashTrack assemblies would be faster and more predictable. The code for
  it is still there, unreachable, below the active `return`.
- **Digit position is discarded when splitting a tag.** `SplitTag` collects every digit into
  one string regardless of where it appeared, so `#Sprint1` and `#1Sprint` both reduce to
  text `#Sprint` + digits `1` and are treated as the same tag. The same applies to `#a1b`
  and `#ab1`. In practice tags rarely lead with a digit, so this never surfaced, but the
  correct fix is to compare the digit *runs* positionally rather than as a single
  concatenated string.
- **The similarity threshold (`0.75`) and lifetimes are hardcoded.** They should be
  configuration, especially the threshold, which is the single value that determines
  clustering quality.
- **No automated tests.** The layering makes the business logic straightforwardly testable,
  `ClusteringClassifier` in particular is a pure function and would take minutes to cover.
  It was cut for the thesis deadline; it's the first thing I'd add.

## Status

Completed as a bachelor thesis project (2024) and published as a portfolio / reference
implementation. Not under active development. Issues and PRs are welcome but may not
receive a fast response.

## License

[MIT](LICENSE) © Jakub Kubisz

## Acknowledgements

Submitted as a bachelor thesis at **VŠB – Technical University of Ostrava**, Faculty of
Electrical Engineering and Computer Science, 2024.

- **Thesis title:** *Hashtag Plugin for MS Outlook* (cz: *Nástroj pro hashtag integraci v MS Outlook*)
- **Supervisor:** Ing. Svatopluk Štolfa, Ph.D.

> The aim of this bachelor thesis is to develop a tool for hashtag integration and
> utilization in Microsoft Outlook. The tool will enable the search of hashtags across all
> user artifacts, provide an overview of their usage, and offer filtering and preview
> options for individual items such as tasks and calendar entries. The thesis will focus on
> analyzing the possibilities of creating a plugin for MS Outlook, exploring existing tools
> for hashtag use, and describing the design and implementation of a custom solution.

*(abstract as submitted)*
