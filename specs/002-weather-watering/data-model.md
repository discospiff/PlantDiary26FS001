# Data Model: Weather Watering Guidance

## WeatherFeedResponse

Represents the relevant portion of the National Weather Service forecast response.

- `properties`: Required response properties object.
- `properties.periods`: Ordered collection of forecast periods; the first item is selected.

## ForecastPeriod

Represents one NWS forecast period.

- `probabilityOfPrecipitation`: Required object for usable recommendation data.
- `probabilityOfPrecipitation.value`: Nullable numeric percentage from 0 through 100.
- Other NWS period fields may be ignored because they are not needed for the recommendation.

## WeatherRecommendation

Represents the result exposed to the Razor Page.

- `IsAvailable`: Whether the first period contained a usable probability.
- `Message`: Exactly `No need to water plants` when the value is greater than 20; exactly `Please to water plants` when the value is less than or equal to 20.
- `ProbabilityOfPrecipitation`: The selected percentage, when available.

## WeatherFeedOptions

Configuration values supplied by application settings.

- `ForecastUrl`: `https://api.weather.gov/gridpoints/ILN/33,67/forecast` by default/configuration.
- `CacheDuration`: Short-lived successful-result cache duration.
- `RequestTimeout`: Maximum duration for the weather request.

## State rules

1. No forecast periods, null probability, malformed JSON, non-success response, timeout, or request failure produces an unavailable recommendation.
2. An unavailable recommendation causes the recommendation area to be omitted from the page.
3. A value greater than 20 produces the no-watering message.
4. A value from 0 through 20 inclusive produces the watering message.
5. Weather state does not change plant-feed state.
