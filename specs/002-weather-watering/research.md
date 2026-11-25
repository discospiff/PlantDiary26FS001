# Research: Weather Watering Guidance

## Decision: Extend the existing ASP.NET Core Razor Pages application

- **Decision**: Add the weather integration to the existing `PlantDiary26FS001` web project and render the recommendation on the existing home Razor Page.
- **Rationale**: The repository already targets .NET 10, uses Razor Pages, and has an established home-page model and test project. This avoids a separate frontend or API surface.
- **Alternatives considered**: A separate client-side application or API would add deployment and maintenance complexity without value for this server-rendered, read-only feature.

## Decision: Retrieve the NWS forecast server-side with HttpClientFactory

- **Decision**: Use a dedicated weather service backed by an injected `HttpClient`, `System.Text.Json`, a configured endpoint, and an application timeout.
- **Rationale**: Server-side retrieval keeps the external endpoint and failure handling out of browser code, supports deterministic tests with a fake HTTP handler, and matches the existing plant-feed pattern.
- **Alternatives considered**: Browser-side fetch was rejected because it exposes integration behavior to the client and complicates graceful failure when the recommendation area must be hidden.

## Decision: Evaluate the first returned forecast period

- **Decision**: Read the first item in `properties.periods` and its `probabilityOfPrecipitation.value`.
- **Rationale**: This is the clarified deterministic selection rule and prevents different callers from applying different forecast windows.
- **Alternatives considered**: Next daytime period and any-period aggregation were rejected by clarification.

## Decision: Keep weather failure independent from plant-feed rendering

- **Decision**: Load plant and weather data as independent concerns. Weather failure returns an unavailable result, and the Razor Page omits only the weather recommendation area while preserving plant results.
- **Rationale**: This directly satisfies FR-010 and prevents an optional external dependency from taking down the existing dashboard.
- **Alternatives considered**: Making weather a required input to the existing combined service was rejected because it would regress the existing plant experience during NWS outages.

## Decision: Use a short-lived memory cache for the forecast

- **Decision**: Cache successful weather results using the existing in-process memory-cache approach and configured duration, while never converting missing probabilities into zero.
- **Rationale**: The home page should avoid repeated external calls while preserving a bounded freshness window. A missing or invalid first-period value remains unavailable.
- **Alternatives considered**: Persistent storage is unnecessary for transient read-only forecast data; no cache would create avoidable repeated network calls.

## Decision: Use Azure App Service deployment without new secrets

- **Decision**: Preserve the existing GitHub Actions/Azure App Service deployment model and configure the public NWS endpoint through application configuration.
- **Rationale**: The endpoint is public and requires no credential. Existing CI/CD already targets Azure App Service.
- **Alternatives considered**: Container deployment would add infrastructure work not required by this feature.

## Decision: Add deterministic unit, HTTP integration, and page tests

- **Decision**: Extend the existing xUnit test project with threshold, boundary, missing-data, malformed-response, HTTP failure, and independent page rendering tests.
- **Rationale**: The constitution requires tests for business logic, external HTTP behavior, errors, and boundaries without relying on the live NWS service.
- **Alternatives considered**: Live endpoint tests were rejected as nondeterministic and unsuitable for routine CI.
