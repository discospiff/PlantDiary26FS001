# Research: Plant Location Dashboard

## Decision: Use a single ASP.NET Core Razor Pages web project

- **Decision**: Extend the existing `PlantDiary26FS001` web project with the home Razor Page, typed feed models, and a small feed-composition service.
- **Rationale**: The repository already uses the Web SDK and targets .NET 10. Razor Pages directly supports the required single-page experience without adding a separate frontend or API project.
- **Alternatives considered**: A separate client-side application would add build and deployment complexity without user value for this server-rendered, read-only page.

## Decision: Retrieve and combine feeds server-side

- **Decision**: Retrieve both public JSON feeds through a shared HTTP client, deserialize them into typed records, join `specimens.plant_id` to `thirstyplants.id`, and pass only combined results to the home page.
- **Rationale**: Server-side composition keeps source URLs out of browser orchestration, centralizes failure handling, supports deterministic ordering, and makes the join logic unit-testable.
- **Alternatives considered**: Browser-side fetch and join would expose feed orchestration to the client and make a single-feed failure harder to represent consistently.

## Decision: Treat both feeds as required for a successful result

- **Decision**: If either feed cannot be retrieved or interpreted, return an error state and no combined results. An empty successful feed is distinct from a failed feed and may produce a no-results state.
- **Rationale**: This follows the clarification and prevents incomplete data from appearing authoritative.
- **Alternatives considered**: Partial rendering was rejected because it can hide missing source data.

## Decision: Use built-in JSON and HTTP functionality

- **Decision**: Use `HttpClientFactory` and `System.Text.Json` with explicit JSON property mappings and tolerant handling of optional strings.
- **Rationale**: These are platform capabilities already suited to the small, read-only integration and avoid unnecessary dependencies.
- **Alternatives considered**: A third-party JSON library is unnecessary for the documented feed shapes.

## Decision: Use Azure App Service with GitHub Actions

- **Decision**: Deploy the web project to Azure App Service using a GitHub Actions workflow. Use a protected deployment identity and repository/environment secrets or federated credentials rather than committing credentials.
- **Rationale**: App Service is a direct hosting fit for an ASP.NET Core Razor Pages application and provides a simple path from Visual Studio local execution to GitHub-based continuous delivery.
- **Alternatives considered**: Azure Container Apps or a container registry would add containerization and operational work not required by this feature.

## Decision: Add automated unit and integration coverage

- **Decision**: Add a test project covering feed composition, identifier joins, ordering, malformed data, required-feed failures, and page states. Use a controllable HTTP message handler for feed integration tests.
- **Rationale**: The constitution requires deterministic automated coverage for business logic and external HTTP behavior.
- **Alternatives considered**: Live-feed-only tests were rejected because they are nondeterministic and can fail due to external availability.

## Open operational assumptions

- The deployment environment will provide the two public GitHub raw feed URLs at runtime.
- Azure App Service is the selected Azure hosting target for the first deployment.
- The application does not persist data; each request obtains the current feed content subject to the selected short-lived cache policy.
