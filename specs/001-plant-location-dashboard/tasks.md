# Tasks: Plant Location Dashboard

**Input**: Design documents from `/specs/001-plant-location-dashboard/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/feeds.md`, and `quickstart.md`

**Testing approach**: Write deterministic xUnit tests before the corresponding implementation task. External feed tests MUST use controlled HTTP responses and MUST NOT depend on live GitHub data.

## Phase 1: Setup

**Purpose**: Establish the test project and delivery workflow without changing application behavior.

- [ ] T001 Create the `PlantDiary26FS001.Tests` xUnit project and add it to `PlantDiary26FS001.slnx`.
- [ ] T002 [P] Configure the test project for .NET 10, nullable reference types, implicit usings, and a project reference to `PlantDiary26FS001/PlantDiary26FS001.csproj` in `PlantDiary26FS001.Tests/PlantDiary26FS001.Tests.csproj`.
- [ ] T003 [P] Create the GitHub Actions validation and deployment workflow in `.github/workflows/ci-cd.yml` for restore, build, test, publish, and Azure App Service deployment.
- [ ] T004 [P] Document required Azure and GitHub environment values, authentication expectations, and deployment prerequisites in `specs/001-plant-location-dashboard/quickstart.md`.

## Phase 2: Foundational

**Purpose**: Establish the domain model, external-feed boundary, validation rules, and dependency injection needed by all user stories.

- [ ] T005 Create `WateringPlant` with the fields and optionality defined in `data-model.md` under `WateringPlant`, including the required `WateringPlant.id` join key.
- [ ] T006 Create `ZooSpecimen` with the fields and optionality defined in `data-model.md` under `ZooSpecimen`, including required `ZooSpecimen.specimen_id`, `ZooSpecimen.plant_id`, `ZooSpecimen.lat`, and `ZooSpecimen.lng`.
- [ ] T007 Create `CombinedSpecimenResult` representing the relationship and display ordering defined in `data-model.md` under `CombinedSpecimenResult`.
- [ ] T008 Create `IPlantFeedService` in `PlantDiary26FS001/Services/IPlantFeedService.cs` with an asynchronous operation that returns combined results or a typed feed failure state.
- [ ] T009 Create feed URL options in `PlantDiary26FS001/appsettings.json` and register the typed HTTP client and feed service in `PlantDiary26FS001/Program.cs`.
- [ ] T010 Implement JSON property mapping and required-field validation for both external feed contracts in `PlantDiary26FS001/Services/PlantFeedService.cs`, resolving every constrained field from `data-model.md` (`WateringPlant.id`, `ZooSpecimen.specimen_id`, `ZooSpecimen.plant_id`, `ZooSpecimen.lat`, `ZooSpecimen.lng`).
- [ ] T011 Implement the server-side join, unmatched-record exclusion, one-result-per-specimen rule, and common-name/specimen-identifier ordering in `PlantDiary26FS001/Services/PlantFeedService.cs`.
- [ ] T012 Add bounded asynchronous HTTP behavior and short-lived response reuse in `PlantDiary26FS001/Services/PlantFeedService.cs` so normal requests avoid unnecessary repeated feed retrieval while preserving the required failure semantics.

**Checkpoint**: Feed models and composition service are available through dependency injection and can be tested without live network access.

## Phase 3: User Story 1 - View Watering-Relevant Zoo Plants (Priority: P1)

**Goal**: Show every valid specimen that joins to a frequently watered plant on the initial page.

**Independent Test**: Provide deterministic plant and specimen feed responses containing matching and unmatched identifiers, request the home page, and confirm that only matching specimens appear in the required order.

### Tests for User Story 1

- [ ] T013 [P] [US1] Add service tests for matching `WateringPlant.id` to `ZooSpecimen.plant_id` in `PlantDiary26FS001.Tests/Services/PlantFeedServiceTests.cs`.
- [ ] T014 [P] [US1] Add service tests for unmatched specimens, multiple specimens sharing one plant, and alphabetical common-name/specimen-identifier ordering in `PlantDiary26FS001.Tests/Services/PlantFeedServiceTests.cs`.
- [ ] T015 [US1] Add home-page model tests for exposing all combined results without search, filtering, or pagination in `PlantDiary26FS001.Tests/Pages/IndexPageModelTests.cs`.

### Implementation for User Story 1

- [ ] T016 [US1] Update `PlantDiary26FS001/Pages/Index.cshtml.cs` to request combined results asynchronously from `IPlantFeedService` for the initial page.
- [ ] T017 [US1] Update `PlantDiary26FS001/Pages/Index.cshtml` to present every combined specimen result in one consistently ordered list with the frequent-watering designation.
- [ ] T018 [US1] Update `PlantDiary26FS001/Pages/Shared/_Layout.cshtml` and `PlantDiary26FS001/wwwroot/css/site.css` to provide a clear page heading, readable result structure, and responsive presentation for the one-page experience.

**Checkpoint**: A visitor can open the home page and independently identify all valid watering-relevant zoo specimens and their stable order.

## Phase 4: User Story 2 - Understand Plant and Specimen Details (Priority: P1)

**Goal**: Make each result self-contained and understandable using available plant and specimen fields.

**Independent Test**: Provide records with populated and empty optional values and confirm that the page identifies the plant and specimen while using clear empty-value treatments.

### Tests for User Story 2

- [ ] T019 [P] [US2] Add page-model tests for common name, genus, species, cultivar, growing-condition, and frequent-watering data in `PlantDiary26FS001.Tests/Pages/IndexPageModelTests.cs`.
- [ ] T020 [P] [US2] Add page-model tests for specimen identifier, latitude, longitude, address, notes, and missing optional values in `PlantDiary26FS001.Tests/Pages/IndexPageModelTests.cs`.

### Implementation for User Story 2

- [ ] T021 [US2] Update `PlantDiary26FS001/Pages/Index.cshtml` to show common and scientific plant names, cultivar, relevant growing-condition values, specimen identifier, latitude, longitude, address, and notes when available.
- [ ] T022 [US2] Add explicit accessible empty-value wording and semantic labels for optional fields in `PlantDiary26FS001/Pages/Index.cshtml`.
- [ ] T023 [US2] Refine `PlantDiary26FS001/wwwroot/css/site.css` for readable plant/specimen groupings at supported desktop and mobile widths without introducing an interactive map, search, or pagination.

**Checkpoint**: Each displayed result explains the plant, specimen, and available location information without requiring source-feed knowledge.

## Phase 5: User Story 3 - Receive Clear Data Availability Feedback (Priority: P2)

**Goal**: Provide understandable loading, error, and no-results states without presenting incomplete combined data.

**Independent Test**: Supply successful matching feeds, successful feeds with zero matches, malformed feed payloads, and unavailable feed responses; confirm each state is represented distinctly.

### Tests for User Story 3

- [ ] T024 [P] [US3] Add feed-service tests for malformed JSON, invalid required identifiers, and invalid required coordinates in `PlantDiary26FS001.Tests/Services/PlantFeedServiceTests.cs`.
- [ ] T025 [P] [US3] Add feed-service tests proving either source failure produces no combined results and a typed failure state in `PlantDiary26FS001.Tests/Services/PlantFeedServiceTests.cs`.
- [ ] T026 [P] [US3] Add page-model tests for loading, source-error, and successful-zero-match states in `PlantDiary26FS001.Tests/Pages/IndexPageModelTests.cs`.

### Implementation for User Story 3

- [ ] T027 [US3] Add explicit feed failure, empty successful-feed, and no-match result states to `PlantDiary26FS001/Services/IPlantFeedService.cs` and `PlantDiary26FS001/Services/PlantFeedService.cs`.
- [ ] T028 [US3] Update `PlantDiary26FS001/Pages/Index.cshtml.cs` to distinguish loading, complete success, no-results, and required-feed failure states without exposing incomplete combined results.
- [ ] T029 [US3] Update `PlantDiary26FS001/Pages/Index.cshtml` with accessible loading, error, and no-results status regions and actionable explanatory text.
- [ ] T030 [US3] Update `PlantDiary26FS001/wwwroot/css/site.css` and `PlantDiary26FS001/wwwroot/js/site.js` so status regions remain readable, keyboard-accessible, and responsive.

**Checkpoint**: Visitors can distinguish loading, complete results, no matches, and required-feed failure states.

## Phase 6: Polish and Cross-Cutting Concerns

**Purpose**: Apply quality gates, security safeguards, performance validation, documentation, and deployment readiness.

- [ ] T031 [P] Add accessibility-focused requirements tests or review fixtures for semantic headings, status announcements, keyboard access, and responsive content in `PlantDiary26FS001.Tests/Pages/IndexAccessibilityTests.cs`.
- [ ] T032 [P] Add performance-oriented tests for bounded feed retrieval and the specified three-second user outcome in `PlantDiary26FS001.Tests/Services/PlantFeedServicePerformanceTests.cs`.
- [ ] T033 [P] Add workflow checks preventing credentials and environment-specific secrets from entering `.github/workflows/ci-cd.yml` or application configuration.
- [ ] T034 Validate the complete local workflow described in `specs/001-plant-location-dashboard/quickstart.md` using Visual Studio 2026.
- [ ] T035 Validate GitHub Actions Azure deployment configuration, protected environment settings, and post-deployment checks described in `specs/001-plant-location-dashboard/quickstart.md`.
- [ ] T036 Update `specs/001-plant-location-dashboard/quickstart.md` with any validated local or Azure operational details discovered during implementation.
- [ ] T037 Build the solution and run the complete automated test suite before marking the feature complete.

## Dependencies and Execution Order

### Phase Dependencies

- **Phase 1: Setup** has no dependencies and establishes the test project and delivery workflow.
- **Phase 2: Foundational** depends on Phase 1 and blocks all user-story work.
- **Phase 3: User Story 1** depends on Phase 2 and delivers the MVP combined list.
- **Phase 4: User Story 2** depends on Phase 3 page structure but its model and rendering tests may be prepared in parallel after Phase 2.
- **Phase 5: User Story 3** depends on Phase 2 service contracts and integrates with the page from Phase 3; its failure-state tests may be prepared in parallel after Phase 2.
- **Phase 6: Polish** depends on the desired user stories being complete.

### User Story Completion Order

1. User Story 1 (P1) — core combined result list and MVP.
2. User Story 2 (P1) — complete plant, specimen, and location detail presentation.
3. User Story 3 (P2) — complete data availability and failure feedback.

### Parallel Opportunities

- T002, T003, and T004 can run in parallel after T001.
- T005, T006, and T007 can run in parallel.
- T013 and T014 can run in parallel; T015 can run after the page model test fixture exists.
- T019 and T020 can run in parallel.
- T024, T025, and T026 can run in parallel.
- T031, T032, and T033 can run in parallel after the relevant user stories are implemented.

## Parallel Example: User Story 1

```text
Task T013: Service join test in PlantDiary26FS001.Tests/Services/PlantFeedServiceTests.cs
Task T014: Ordering and unmatched-record tests in PlantDiary26FS001.Tests/Services/PlantFeedServiceTests.cs
Task T015: Home-page result-list test in PlantDiary26FS001.Tests/Pages/IndexPageModelTests.cs
```

## Parallel Example: User Story 2

```text
Task T019: Plant-detail presentation tests in PlantDiary26FS001.Tests/Pages/IndexPageModelTests.cs
Task T020: Specimen-detail and empty-value tests in PlantDiary26FS001.Tests/Pages/IndexPageModelTests.cs
```

## Parallel Example: User Story 3

```text
Task T024: Malformed and invalid-record tests in PlantDiary26FS001.Tests/Services/PlantFeedServiceTests.cs
Task T025: Required-feed failure tests in PlantDiary26FS001.Tests/Services/PlantFeedServiceTests.cs
Task T026: Page-state tests in PlantDiary26FS001.Tests/Pages/IndexPageModelTests.cs
```

## Implementation Strategy

### MVP First

1. Complete Phase 1 setup.
2. Complete Phase 2 foundational models and feed composition.
3. Complete Phase 3 User Story 1.
4. Stop and validate the combined list independently.
5. Continue to detail and failure-state stories only after the MVP is accepted.

### Incremental Delivery

1. Setup and foundational service.
2. User Story 1: combined watering-relevant specimen list.
3. User Story 2: complete plant and specimen details.
4. User Story 3: loading, empty, and error states.
5. Polish, local validation, and Azure CI/CD validation.

### Notes

- `[P]` tasks affect separate files or independent concerns and can be performed in parallel.
- `[US1]`, `[US2]`, and `[US3]` map tasks to the corresponding specification user story.
- Tests must be written and fail before the related implementation task is completed.
- Live feed access is reserved for manual or deployment smoke checks; automated tests remain deterministic.
- `/speckit-implement` is the next command that should execute these tasks.
