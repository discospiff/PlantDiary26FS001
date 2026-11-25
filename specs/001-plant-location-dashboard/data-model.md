# Data Model: Plant Location Dashboard

## WateringPlant

Represents a plant from the frequent-watering feed.

| Field | Type | Required | Description |
|---|---|---:|---|
| `id` | integer | yes | Unique plant identifier used by the specimen foreign key. |
| `genus` | string | no | Botanical genus. |
| `species` | string | no | Botanical species or related classification text. |
| `cultivar` | string | no | Cultivar name when supplied. |
| `common` | string | no | Common plant name. |
| `wetSoil` | boolean | no | Whether the plant tolerates or prefers wet soil. |
| `drySoil` | boolean | no | Whether the plant tolerates dry soil. |
| `rainGarden` | boolean | no | Whether the plant is identified as suitable for a rain garden. |
| `fullSun` | integer | no | Full-sun preference rating when supplied. |
| `partShade` | integer | no | Part-shade preference rating when supplied. |
| `deepShade` | integer | no | Deep-shade preference rating when supplied. |

## ZooSpecimen

Represents a specimen from the Cincinnati Zoo specimen feed.

| Field | Type | Required | Description |
|---|---|---:|---|
| `specimen_id` | integer | yes | Unique specimen identifier. |
| `plant_id` | integer | yes | Foreign key to `WateringPlant.id`. |
| `lat` | decimal | yes | Latitude of the specimen. |
| `lng` | decimal | yes | Longitude of the specimen. |
| `common` | string | no | Common name from the specimen record. |
| `genus` | string | no | Genus from the specimen record. |
| `species` | string | no | Species from the specimen record. |
| `address` | string | no | Site address when supplied. |
| `notes` | string | no | Descriptive notes when supplied. |

## CombinedSpecimenResult

The page-facing combination of one `WateringPlant` and one `ZooSpecimen`.

- The relationship is valid only when `ZooSpecimen.plant_id == WateringPlant.id`.
- Each specimen produces its own result, even when several specimens share one plant.
- A specimen without a matching watering plant is excluded.
- The result retains the specimen’s coordinates and optional descriptive fields.
- Display order is ascending by common plant name, then ascending by specimen identifier.
- Empty optional values are represented by a clear page-level empty-value treatment rather than fabricated values.

## Data validation and failure rules

- A feed response must contain a JSON array of records; an invalid payload is a feed failure.
- Required identifiers must be valid integers before a record participates in the join.
- Invalid or incomplete records must not create fabricated combined results.
- Either feed failure produces a page error state with no combined results.
- Two successful feeds with zero matches produce a no-results state.
