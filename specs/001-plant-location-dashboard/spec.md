# Feature Specification: Plant Location Dashboard

**Feature Branch**: `001-plant-location-dashboard`
**Created**: 2026-11-24
**Status**: Draft
**Input**: User description: "Build an application that combines several JSON feeds: a list of plants that require frequent watering and a list of plant specimens at the Cincinnati Zoo containing GPS locations and a foreign key to the plant list. The application should be a simple one page application where the relevant data are shown on the first page the user sees on the site."

## Clarifications

### Session 2026-11-24

- Q: If one of the two source feeds is unavailable, should the page show no combined results until both feeds are available? → A: Show an error state and no combined results until both feeds are available.
- Q: How should the specimen GPS location be presented to visitors? → A: Show latitude and longitude only.
- Q: Should the first page display every matching specimen in one list, or provide browsing controls for the results? → A: Display every matching specimen in one consistently ordered list.
- Q: How should the matching specimens be ordered in the single list? → A: Common plant name, then specimen identifier.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - View Watering-Relevant Zoo Plants (Priority: P1)

As a visitor, I want to see Cincinnati Zoo plant specimens that correspond to plants requiring frequent watering so that I can identify which specimens need attention and where they are located.

**Why this priority**: Combining the two source lists into one useful view is the core value of the application.

**Independent Test**: Load the site’s first page and verify that a specimen with a matching plant identifier displays both its plant information and location information.

**Acceptance Scenarios**:

1. **Given** both source lists are available and contain matching identifiers, **When** a visitor opens the site, **Then** the first page displays the matching specimens with their plant and location details.
2. **Given** a specimen references a plant identifier that is not in the watering list, **When** a visitor opens the site, **Then** that specimen is excluded from the watering-relevant results.

---

### User Story 2 - Understand Plant and Specimen Details (Priority: P1)

As a visitor, I want each result to clearly identify the plant and specimen so that I can understand what I am viewing without consulting the source lists separately.

**Why this priority**: Clear combined information makes the feed integration useful to visitors.

**Independent Test**: Inspect a displayed result and verify that it identifies the plant and includes the specimen’s coordinates and available descriptive details.

**Acceptance Scenarios**:

1. **Given** a matching specimen has plant and location data, **When** the result is displayed, **Then** the result shows the common name, scientific name, relevant cultivar information when available, specimen identifier, latitude, longitude, address, and notes when available.
2. **Given** a field is empty in a source record, **When** the result is displayed, **Then** the page uses a clear empty-value treatment rather than showing misleading or broken content.

---

### User Story 3 - Receive Clear Data Availability Feedback (Priority: P2)

As a visitor, I want clear feedback when the source data cannot be loaded or no records match so that I understand the state of the page.

**Why this priority**: A useful and trustworthy page must explain unavailable or empty results.

**Independent Test**: Simulate an unavailable source or a valid response with no matching records and verify that the page displays an understandable status message.

**Acceptance Scenarios**:

1. **Given** one or both source lists cannot be loaded, **When** a visitor opens the site, **Then** the page displays a user-friendly data availability message and does not present incomplete records as complete results.
2. **Given** both source lists load successfully but no specimen matches a frequently watered plant, **When** a visitor opens the site, **Then** the page displays a clear no-results message.

### Edge Cases

- A source list is empty or contains malformed records.
- Multiple specimens reference the same plant identifier; each specimen remains a separate result.
- A plant appears in the watering list but has no corresponding specimen.
- A specimen contains coordinates but no address or notes.
- A source feed responds slowly or becomes unavailable after a previous successful load.
- Source data contains duplicate identifiers; the page must avoid silently combining unrelated records.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The application MUST retrieve the plant list from `https://raw.githubusercontent.com/discospiff/data/refs/heads/main/thirstyplants.json`.
- **FR-002**: The application MUST retrieve the specimen list from `https://raw.githubusercontent.com/discospiff/data/refs/heads/main/specimens.json`.
- **FR-003**: The application MUST combine a specimen with a plant record when the specimen’s `plant_id` equals the plant record’s `id`.
- **FR-004**: The application MUST show combined results on the first page presented to a visitor.
- **FR-005**: The application MUST include the plant common name and scientific classification in each matching result when those values are available.
- **FR-006**: The application MUST identify that each displayed plant comes from the frequent-watering list.
- **FR-007**: The application MUST display each matching specimen’s specimen identifier, latitude, longitude, address, and notes when those values are available.
- **FR-008**: The application MUST display cultivar and plant growing-condition information when available and relevant to identifying the plant.
- **FR-009**: The application MUST display a clear loading state while the source data is being retrieved.
- **FR-010**: The application MUST display a clear error state and no combined results when either source list cannot be retrieved or cannot be interpreted.
- **FR-011**: The application MUST display a clear no-results state when both source lists are available but no matching specimens exist.
- **FR-012**: The application MUST keep separate specimen results separate when multiple specimens reference the same plant.
- **FR-013**: The page MUST be usable on supported desktop and mobile screen sizes and by keyboard users.
- **FR-014**: The application MUST display every matching specimen in one consistently ordered list without requiring search, filtering, or pagination controls.
- **FR-015**: The application MUST order results alphabetically by common plant name and then by specimen identifier.

### Key Entities *(include if feature involves data)*

- **Watering Plant**: A plant in the frequent-watering source list. It is identified by `id` and may include genus, species, cultivar, common name, soil preferences, rain-garden suitability, and light preferences.
- **Zoo Specimen**: A plant specimen at the Cincinnati Zoo. It is identified by `specimen_id`, references a Watering Plant through `plant_id`, and may include latitude, longitude, address, notes, and plant naming details.
- **Combined Specimen Result**: The visitor-facing representation of a Zoo Specimen joined to its matching Watering Plant.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: In a normal successful load, at least 95% of page visits display the first combined results or a clear empty state within 3 seconds of opening the site.
- **SC-002**: For a source dataset containing matching records, 100% of displayed results have a valid relationship between the specimen `plant_id` and the plant `id`.
- **SC-003**: A visitor can identify a matching specimen’s common name and geographic coordinates within 10 seconds of viewing the first page.
- **SC-004**: 100% of tested source-load failures display a clear error state without presenting incomplete combined records as valid results.
- **SC-005**: At least 90% of representative users can locate a frequently watered specimen and its location on their first attempt without instructions.
- **SC-006**: The page remains readable and all primary information remains usable at the supported desktop and mobile viewport sizes.

## Assumptions

- The two referenced JSON feeds are publicly accessible and remain the authoritative sources for this feature.
- The plant `id` and specimen `plant_id` values are numeric identifiers and use the same identifier domain.
- The first version does not require user accounts, personalization, editing, persistence, or administrative workflows.
- The first version uses a single page and does not require separate detail pages.
- GPS coordinates are shown as location information; an interactive map is not required unless a later specification adds that need.
- Source-feed changes are reflected when the application retrieves the feeds; historical snapshots are out of scope.
- Results are intended for visitors interested in Cincinnati Zoo specimens and plants requiring frequent watering.
