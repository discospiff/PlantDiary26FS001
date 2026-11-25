# Implementation Plan: Weather Watering Guidance

**Branch**: `WeatherData` | **Date**: 2026-11-24 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/002-weather-watering/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Add a server-side National Weather Service forecast integration to the existing .NET 10 ASP.NET Core Razor Pages application. The home page will use the first returned forecast period, show the exact watering message for probabilities above or at/below 20%, and omit only the weather recommendation area when weather data is unavailable. Plant-feed rendering remains independent. The design uses a focused weather service, typed JSON models, configured `HttpClientFactory`, short-lived memory caching, and deterministic xUnit tests.

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: C# on .NET 10

**Primary Dependencies**: ASP.NET Core Razor Pages, built-in `HttpClientFactory`, `System.Text.Json`, `IMemoryCache`, xUnit

**Storage**: N/A; transient external forecast data with in-process memory caching

**Testing**: Existing `PlantDiary26FS001.Tests` xUnit project with fake HTTP handlers and page-model tests

**Target Platform**: Visual Studio Enterprise 2026 local development and Azure App Service

**Project Type**: ASP.NET Core server-rendered Razor Pages web application

**Performance Goals**: One bounded weather request per cache interval; asynchronous I/O; no material increase to home-page response work when cached

**Constraints**: Preserve existing plant-page behavior during weather failure; use configured timeout; do not add unnecessary dependencies; hide recommendation area when unavailable

**Scale/Scope**: One existing home page, one public NWS endpoint, one forecast period, no persistence, no authentication changes

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Code quality and maintainability**: PASS — use existing C# conventions, nullable types, focused service responsibility, built-in libraries, and explicit error handling.
- **Testing**: PASS — add deterministic unit and HTTP integration coverage for threshold, boundary, malformed, missing, timeout, and failure cases.
- **User experience**: PASS — preserve the existing dashboard, provide an accessible weather region when available, and omit it when unavailable.
- **Performance**: PASS — use asynchronous HTTP, timeout control, and bounded memory caching; validate no repeated calls within the cache interval.
- **Definition of done**: PASS — require aligned artifacts, successful build, passing tests, and Azure App Service readiness documentation.
- **Project constraints**: PASS — remain in the existing .NET 10 Razor Pages project and preserve public behavior.

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/           # Phase 1 output (/speckit-plan command)
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
PlantDiary26FS001/
├── Models/
│   └── Weather*.cs                  # Typed NWS response/result models
├── Services/
│   ├── IWeatherService.cs           # Weather integration abstraction
│   ├── WeatherService.cs            # Async retrieval, parsing, caching, mapping
│   └── WeatherFeedOptions.cs        # Endpoint and timeout configuration
├── Pages/
│   ├── Index.cshtml.cs              # Compose plant and independent weather states
│   └── Index.cshtml                 # Render accessible recommendation conditionally
├── Program.cs                       # Register options, HTTP client, cache, service
└── appsettings.json                 # Public endpoint and operational defaults

PlantDiary26FS001.Tests/
├── Services/
│   └── WeatherServiceTests.cs       # Threshold, parsing, cache, and failure tests
└── Pages/
    └── IndexPageModelTests.cs       # Independent weather/page-state tests
```

**Structure Decision**: Extend the existing single web project and existing xUnit test project. Keep the NWS integration in a dedicated service rather than coupling it to `PlantFeedService`, so weather failure cannot suppress plant results. Render the recommendation from the existing home Razor Page and preserve the current Azure App Service workflow.

## Constitution Check — Post-Design

- **Code quality and maintainability**: PASS — the design uses a focused weather service, typed models, existing framework libraries, and configuration rather than hard-coded operational behavior.
- **Testing**: PASS — service and page tests cover exact messages, the 20% boundary, unavailable data, HTTP failures, and weather/plant independence.
- **Consistent user experience**: PASS — the recommendation is rendered in the existing home-page flow with accessible labeling and is omitted cleanly when unavailable.
- **Performance and resource efficiency**: PASS — asynchronous requests, cancellation/timeout, and bounded caching are included; no persistent store or unbounded collection is introduced.
- **Definition of done**: PASS — implementation must update documentation as needed, build successfully, pass focused and full tests, and remain deployable through the existing Azure App Service workflow.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | N/A | The feature fits the existing web and test projects and requires no new infrastructure project. |
