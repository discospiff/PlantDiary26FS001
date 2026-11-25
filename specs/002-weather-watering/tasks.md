# Tasks: Weather Watering Guidance

**Input**: Design documents from `/specs/002-weather-watering/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/weather-feed.md, quickstart.md

**Tests**: Tests are required by the specification and constitution. All external-feed tests must use deterministic fake HTTP handlers rather than the live NWS endpoint.

## Phase 1: Setup

**Purpose**: Prepare the existing .NET 10 web and test projects for the weather integration.

- [X] T001 Confirm the implementation targets `PlantDiary26FS001/PlantDiary26FS001.csproj` and `PlantDiary26FS001.Tests/PlantDiary26FS001.Tests.csproj` without adding a new project.
- [X] T002 [P] Verify `.gitignore` covers build output, IDE state, test results, and local configuration artifacts.
- [X] T003 [P] Add the weather feature configuration section and NWS endpoint defaults to `PlantDiary26FS001/appsettings.json` and the appropriate development configuration file.

## Phase 2: Foundational

**Purpose**: Establish typed weather data, options, service contracts, and dependency injection before page work.

- [X] T004 [P] Create `WeatherFeedOptions` with the configured forecast URL, cache duration, and request timeout in `PlantDiary26FS001/Services/WeatherFeedOptions.cs` (data-model.md: `WeatherFeedOptions.ForecastUrl`, `WeatherFeedOptions.CacheDuration`, `WeatherFeedOptions.RequestTimeout`).
- [X] T005 [P] Create typed NWS response models for `properties.periods[0].probabilityOfPrecipitation.value` in `PlantDiary26FS001/Models/WeatherForecast.cs` and `PlantDiary26FS001/Models/WeatherForecastPeriod.cs` (data-model.md: `WeatherFeedResponse.properties.periods`, `ForecastPeriod.probabilityOfPrecipitation.value`).
- [X] T006 [P] Create the weather recommendation result model with availability, selected probability, and exact message values in `PlantDiary26FS001/Models/WeatherRecommendation.cs` (data-model.md: `WeatherRecommendation.IsAvailable`, `WeatherRecommendation.Message`, `WeatherRecommendation.ProbabilityOfPrecipitation`).
- [X] T007 Create `IWeatherService` in `PlantDiary26FS001/Services/IWeatherService.cs` with an asynchronous operation returning the weather recommendation result.
- [X] T008 Register weather options, memory caching, and the typed weather HTTP client/service in `PlantDiary26FS001/Program.cs` while preserving existing plant-feed registrations.

**Checkpoint**: The application has a compile-ready weather integration boundary, configuration, and typed data model without changing page behavior.

## Phase 3: User Story 1 — View Watering Recommendation (Priority: P1)

**Goal**: Show the exact watering recommendation based on the first forecast period.

**Independent Test**: Supply deterministic forecast JSON with probabilities above 20, exactly 20, and at or below 20; confirm the service and home page expose the required exact message.

### Tests for User Story 1

- [X] T009 [P] [US1] Add service tests for probabilities above 20 and at or below 20, including exactly 20 and zero, in `PlantDiary26FS001.Tests/Services/WeatherServiceTests.cs`.
- [X] T010 [P] [US1] Add a page-model test proving an available recommendation is exposed alongside plant results in `PlantDiary26FS001.Tests/Pages/IndexPageModelTests.cs`.

### Implementation for User Story 1

- [X] T011 [US1] Implement asynchronous NWS retrieval, response validation, JSON deserialization, first-period selection, and percentage mapping in `PlantDiary26FS001/Services/WeatherService.cs` according to `specs/002-weather-watering/contracts/weather-feed.md`.
- [X] T012 [US1] Implement the exact threshold mapping in `PlantDiary26FS001/Services/WeatherService.cs`: greater than 20 returns `No need to water plants`; values from 0 through 20 inclusive return `Please to water plants` (data-model.md: `WeatherRecommendation.Message`).
- [X] T013 [US1] Compose plant and weather service calls independently in `PlantDiary26FS001/Pages/Index.cshtml.cs` while preserving existing plant statuses and results.
- [X] T014 [US1] Render the labeled Cincinnati Zoo weather recommendation conditionally on the home page in `PlantDiary26FS001/Pages/Index.cshtml`, using the existing page layout and accessible semantic markup.

**Checkpoint**: A successful forecast displays one exact recommendation on the existing home page without requiring changes to plant-feed behavior.

## Phase 4: User Story 2 — Understand Recommendation Context (Priority: P1)

**Goal**: Make the recommendation clearly identifiable as Cincinnati Zoo weather guidance.

**Independent Test**: Inspect the rendered home page with an available recommendation and confirm that its label, context, and accessible semantics identify the forecast source and purpose.

### Tests for User Story 2

- [X] T015 [P] [US2] Add page accessibility/rendering assertions for the weather recommendation label, semantic region, exact message, and Cincinnati Zoo context in `PlantDiary26FS001.Tests/Pages/IndexAccessibilityTests.cs`.
- [X] T016 [P] [US2] Add a page-model test proving weather availability does not alter successful plant result state in `PlantDiary26FS001.Tests/Pages/IndexPageModelTests.cs`.

### Implementation for User Story 2

- [X] T017 [US2] Add the weather recommendation view state and Cincinnati Zoo context properties to `PlantDiary26FS001/Pages/Index.cshtml.cs` without exposing raw feed details.
- [X] T018 [US2] Refine the weather recommendation markup and styles in `PlantDiary26FS001/Pages/Index.cshtml` and `PlantDiary26FS001/wwwroot/css/site.css` for consistent hierarchy, responsive layout, contrast, keyboard-neutral read-only presentation, and screen-reader clarity.

**Checkpoint**: The recommendation is understandable and accessible as Cincinnati Zoo weather guidance while the existing plant dashboard remains unchanged.

## Phase 5: User Story 3 — Handle Unavailable Weather Data (Priority: P2)

**Goal**: Hide only the weather recommendation area when the external feed is unusable.

**Independent Test**: Supply empty periods, null probability, malformed JSON, non-success HTTP status, timeout, and request failure; confirm the weather area is omitted while plant data remains available.

### Tests for User Story 3

- [X] T019 [P] [US3] Add weather service tests for empty periods, null probability, malformed JSON, non-success HTTP status, timeout, and cancellation in `PlantDiary26FS001.Tests/Services/WeatherServiceTests.cs`.
- [X] T020 [P] [US3] Add a page-model test proving unavailable weather produces no recommendation while successful plant results remain available in `PlantDiary26FS001.Tests/Pages/IndexPageModelTests.cs`.
- [X] T021 [P] [US3] Add a rendering/accessibility test proving the weather recommendation region is omitted when weather data is unavailable in `PlantDiary26FS001.Tests/Pages/IndexAccessibilityTests.cs`.

### Implementation for User Story 3

- [X] T022 [US3] Implement bounded timeout, cancellation propagation, non-success handling, malformed-response handling, missing-period handling, and structured logging in `PlantDiary26FS001/Services/WeatherService.cs`.
- [X] T023 [US3] Ensure unavailable weather results do not replace or suppress plant-feed status and results in `PlantDiary26FS001/Pages/Index.cshtml.cs`.
- [X] T024 [US3] Omit the weather recommendation container when the weather result is unavailable in `PlantDiary26FS001/Pages/Index.cshtml` rather than rendering stale, zero-valued, or misleading guidance.

**Checkpoint**: Upstream weather failures degrade only the optional recommendation area and never prevent independently available plant results from rendering.

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Validate the complete feature, operational behavior, and documentation.

- [X] T025 [P] Add successful-result memory caching and cache-hit tests in `PlantDiary26FS001/Services/WeatherService.cs` and `PlantDiary26FS001.Tests/Services/WeatherServiceTests.cs` (data-model.md: `WeatherFeedOptions.CacheDuration`).
- [X] T026 [P] Verify configured request timeout and cancellation behavior against the performance and reliability requirements in `PlantDiary26FS001.Tests/Services/WeatherServiceTests.cs` (data-model.md: `WeatherFeedOptions.RequestTimeout`).
- [X] T027 [P] Update `specs/002-weather-watering/quickstart.md` with any validated local or Azure App Service configuration and smoke-check details.
- [X] T028 Run the focused weather and page tests through Visual Studio Test Explorer or `dotnet test PlantDiary26FS001.Tests/PlantDiary26FS001.Tests.csproj`.
- [X] T029 Run the full solution build and test validation; resolve compiler warnings and test failures.
- [X] T030 Verify the existing GitHub Actions Azure App Service workflow remains compatible with the new configuration and document any required environment overrides in `specs/002-weather-watering/quickstart.md`.
- [X] T031 Perform an accessibility review of the available, loading, unavailable, and plant-only page states, then record results in the implementation review.

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: No dependencies.
- **Phase 2 (Foundational)**: Depends on Phase 1 and blocks all user stories.
- **Phase 3 (US1)**: Depends on Phase 2; establishes the core recommendation.
- **Phase 4 (US2)**: Depends on Phase 3 page integration, although accessibility tests can be prepared in parallel with US1 implementation.
- **Phase 5 (US3)**: Depends on the service and page integration from Phase 3; adds failure isolation and omission behavior.
- **Phase 6 (Polish)**: Depends on all required user stories.

### User Story Dependencies

- **US1 (P1)**: Can start after Phase 2; no dependency on US2 or US3.
- **US2 (P1)**: Depends on the US1 page integration; refines context and accessibility.
- **US3 (P2)**: Depends on the US1 service/page integration; validates independent failure behavior.

### Within Each User Story

- Write tests before implementation and verify they fail for the new behavior.
- Create or update models before services, services before page models, and page models before Razor markup.
- Complete the story checkpoint before moving to the next priority.

## Parallel Execution Examples

### User Story 1

```text
T009 service threshold tests and T010 page-model tests can run in parallel.
After foundational contracts exist, T011/T012 service work precedes T013/T014 page integration.
```

### User Story 2

```text
T015 accessibility tests and T016 page-state tests can run in parallel.
T017 page-model context work and T018 Razor/CSS work can proceed after US1 page integration.
```

### User Story 3

```text
T019 service failure tests, T020 page-state tests, and T021 rendering tests can run in parallel.
T022 service failure handling precedes final T023/T024 page behavior validation.
```

## Implementation Strategy

### MVP First

1. Complete Setup and Foundational phases.
2. Complete US1 for core threshold behavior.
3. Run focused tests and validate the first-page recommendation.
4. Add US2 context/accessibility and US3 failure isolation.
5. Complete polish, build, test, and Azure readiness checks.

### Incremental Delivery

Each completed story remains independently valuable: US1 provides the recommendation, US2 makes it understandable and accessible, and US3 preserves the existing dashboard during weather failures.
