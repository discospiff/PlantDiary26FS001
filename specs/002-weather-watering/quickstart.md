# Quickstart: Weather Watering Guidance

## Local development

1. Open `PlantDiary26FS001.slnx` in Visual Studio Enterprise 2026.
2. Confirm the web project targets `net10.0` and the test project is available.
3. Run the existing xUnit tests before implementation changes.
4. Configure the NWS forecast URL, cache duration, and request timeout through the `WeatherFeed` configuration section. Defaults are the Cincinnati Zoo endpoint, five-minute successful-result caching, and a ten-second request timeout. Do not add credentials; the endpoint is public.
5. Start the Razor Pages application with Visual Studio or `dotnet run`.
6. Open the home page and verify that a successful first forecast period displays exactly one recommendation.

## Deterministic validation

Automated tests must use a controllable `HttpMessageHandler` rather than the live weather endpoint. Cover:

- Probability values above 20.
- Probability values of 20, 0, and below/at the threshold.
- Missing periods and null probability.
- Malformed JSON, non-success status, timeout, and cancellation behavior.
- Weather failure while plant data remains visible.
- Accessible omission of the recommendation area when weather is unavailable.
- Values outside the 0–100 percentage range are treated as unavailable weather data.

## Azure deployment

The existing GitHub Actions workflow deploys the ASP.NET Core application to Azure App Service. The weather endpoint is public and should be supplied as normal application configuration if an environment-specific override is required. Do not place credentials in source control.

Before release, verify:

- CI restores, builds, tests, and publishes the .NET 10 solution.
- Azure App Service starts successfully.
- The home page still renders plant results when the NWS endpoint is unavailable.
- The recommendation is visible and associated with the Cincinnati Zoo forecast when the endpoint succeeds.
- The recommendation area is hidden for missing or unusable weather data.
- HTTPS, keyboard access, and screen-reader status behavior remain functional.
- Weather-feed failure does not suppress independently available plant results.
