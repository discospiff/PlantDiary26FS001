# Weather Feed Contract

## External request

- **Method**: `GET`
- **URL**: Configured NWS endpoint, defaulting to `https://api.weather.gov/gridpoints/ILN/33,67/forecast`
- **Accept**: `application/geo+json, application/json`
- **Authentication**: None; the endpoint is public.

## Required response shape

```json
{
  "properties": {
	"periods": [
	  {
		"probabilityOfPrecipitation": {
		  "value": 20,
		  "unitCode": "wmoUnit:percent"
		}
	  }
	]
  }
}
```

Only `properties.periods[0].probabilityOfPrecipitation.value` is used. The value is interpreted as a percentage.

## Application service result

The service returns either:

- An available recommendation with the selected percentage and one exact user-facing message; or
- An unavailable result with no recommendation message.

The service does not expose raw external exceptions to the page and does not treat missing values as zero.
