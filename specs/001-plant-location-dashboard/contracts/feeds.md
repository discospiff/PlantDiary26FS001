# External Feed Contracts

## Thirsty plants feed

**Source**: `https://raw.githubusercontent.com/discospiff/data/refs/heads/main/thirstyplants.json`

- Response: JSON array of `WateringPlant` records.
- Required join field: numeric `id`.
- Relevant display fields: `genus`, `species`, `cultivar`, `common`, `wetSoil`, `drySoil`, `rainGarden`, `fullSun`, `partShade`, and `deepShade`.

## Specimens feed

**Source**: `https://raw.githubusercontent.com/discospiff/data/refs/heads/main/specimens.json`

- Response: JSON array of `ZooSpecimen` records.
- Required join fields: numeric `plant_id` and `specimen_id`.
- Required location fields: numeric `lat` and `lng`.
- Optional display fields: `common`, `genus`, `species`, `address`, and `notes`.

## Composition contract

The application MUST:

1. Retrieve both feeds for the same page request or valid short-lived cache window.
2. Reject the combined result when either feed fails retrieval or JSON interpretation.
3. Match `specimens.plant_id` to `thirstyplants.id`.
4. Emit one page result per valid specimen match.
5. Sort results by common plant name, then specimen identifier.
6. Exclude unmatched specimens and invalid records from successful results.

This is an external read contract; the application does not modify either source feed.
