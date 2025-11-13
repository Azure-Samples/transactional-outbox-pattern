# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade Contacts.Domain\Contacts.Domain.csproj
4. Upgrade Contacts.Infrastructure\Contacts.Infrastructure.csproj
5. Upgrade Contacts.Application\Contacts.Application.csproj
6. Upgrade Contacts.EventsProcessor\Contacts.EventsProcessor.csproj
7. Upgrade Contacts.API\Contacts.API.csproj
8. Upgrade Contacts.Tests\Contacts.Tests.csproj
9. Run unit tests to validate upgrade in the projects listed below:
   - Contacts.Tests\Contacts.Tests.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

| Project name | Description |
|:-----------------------------------------------|:---------------------------:|

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                    | Current Version | New Version | Description                                   |
|:-----------------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Microsoft.Extensions.Configuration             |   9.0.10        |  10.0.0     | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Configuration.Abstractions|   9.0.10        |  10.0.0     | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Configuration.EnvironmentVariables | 9.0.10 | 10.0.0 | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Configuration.Json        |   9.0.10        |  10.0.0     | Recommended for .NET 10.0                     |
| Microsoft.Extensions.DependencyInjection       |   9.0.10        |  10.0.0     | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Hosting                   |   9.0.10        |  10.0.0     | Recommended for .NET 10.0                     |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 9.0.0 | 10.0.0-rc.1.25458.5 | Recommended for .NET 10.0                     |

### Project upgrade details

#### Contacts.Domain\Contacts.Domain.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### Contacts.Infrastructure\Contacts.Infrastructure.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - Microsoft.Extensions.Configuration.Abstractions should be updated from `9.0.10` to `10.0.0` (recommended for .NET 10.0)

#### Contacts.Application\Contacts.Application.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### Contacts.EventsProcessor\Contacts.EventsProcessor.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - Microsoft.Extensions.Hosting should be updated from `9.0.10` to `10.0.0` (recommended for .NET 10.0)

#### Contacts.API\Contacts.API.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - Microsoft.VisualStudio.Web.CodeGeneration.Design should be updated from `9.0.0` to `10.0.0-rc.1.25458.5` (recommended for .NET 10.0)

#### Contacts.Tests\Contacts.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - Microsoft.Extensions.Configuration should be updated from `9.0.10` to `10.0.0` (recommended for .NET 10.0)
  - Microsoft.Extensions.Configuration.EnvironmentVariables should be updated from `9.0.10` to `10.0.0` (recommended for .NET 10.0)
  - Microsoft.Extensions.Configuration.Json should be updated from `9.0.10` to `10.0.0` (recommended for .NET 10.0)
  - Microsoft.Extensions.DependencyInjection should be updated from `9.0.10` to `10.0.0` (recommended for .NET 10.0)
