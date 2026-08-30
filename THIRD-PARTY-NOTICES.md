# Third-party notices

HashTrack uses the following third-party packages. All are consumed unmodified via NuGet and
none are redistributed in source form in this repository.

## Direct dependencies

| Package | Version | License |
|---|---|---|
| [Autofac](https://github.com/autofac/Autofac) | 8.0.0 | MIT |
| [SimMetrics.Net](https://github.com/StefH/SimMetrics.Net) | 1.0.5 | MIT |
| [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) | 8.2.0 | MIT |
| [EntityFramework](https://github.com/dotnet/ef6) | 6.3.0 | MIT |
| [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) | 13.x | MIT |
| [System.Data.SQLite](https://system.data.sqlite.org/) | 1.0.112 | **Public domain** |
| System.Data.SQLite.Core | 1.0.112 | **Public domain** |
| System.Data.SQLite.EF6 | 1.0.112 | **Public domain** |
| System.Data.SQLite.EF6.Migrations | 1.0.112 | **Public domain** |
| System.Data.SQLite.Linq | 1.0.112 | **Public domain** |
| Stub.System.Data.SQLite.Core.NetFramework | 1.0.113 | **Public domain** |

## Transitive framework packages

The following are Microsoft-published support libraries pulled in by the packages above, all
licensed **MIT**:

`Microsoft.Bcl.AsyncInterfaces` · `Microsoft.Extensions.Caching.Abstractions` ·
`Microsoft.Extensions.Caching.Memory` ·
`Microsoft.Extensions.DependencyInjection.Abstractions` ·
`Microsoft.Extensions.Logging.Abstractions` · `Microsoft.Extensions.Options` ·
`Microsoft.Extensions.Primitives` · `System.Buffers` ·
`System.ComponentModel.Annotations` · `System.Diagnostics.DiagnosticSource` ·
`System.Memory` · `System.Numerics.Vectors` · `System.Runtime.CompilerServices.Unsafe` ·
`System.Threading.Tasks.Extensions` · `System.ValueTuple`

## Notes

SQLite itself is released into the [public domain](https://www.sqlite.org/copyright.html),
which imposes no conditions on use or redistribution.

Every dependency is permissive (MIT or public domain). None are copyleft, so they place no
restrictions on the MIT license of this project.

Full license text for each package is available in its linked repository, and locally under
`packages/` after a NuGet restore.
