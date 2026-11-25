# Quickstart: Plant Location Dashboard

## Prerequisites

- Visual Studio 2026 with ASP.NET and web development support.
- .NET 10 SDK.
- Access to the repository and the two public JSON feeds.
- An Azure subscription and an Azure App Service resource for deployment.
- A GitHub repository with Actions enabled for CI/CD.

## Run locally

1. Open `PlantDiary26FS001.slnx` in Visual Studio 2026.
2. Select the `PlantDiary26FS001` project as the startup project.
3. Run the project with the HTTPS launch profile.
4. Open the home page and verify the loading, success, empty, and error states.
5. Confirm that matching records are ordered by common name and specimen identifier and that latitude and longitude are displayed.

The application has no application database. Local behavior depends on access to both public feed URLs.

## Validate locally

1. Build the solution without warnings introduced by the feature.
2. Run the unit and integration test projects.
3. Use deterministic HTTP test responses for normal feeds, empty feeds, malformed payloads, unavailable feeds, unmatched identifiers, duplicate plant references, and optional fields.
4. Manually check keyboard navigation and representative desktop and mobile viewport sizes.

## GitHub CI/CD

The GitHub Actions workflow should:

1. Trigger on pull requests and pushes to the protected deployment branch.
2. Restore dependencies, build the solution, and run all tests.
3. Publish the .NET 10 web application only after validation succeeds.
4. Authenticate to Azure using a protected deployment identity, preferably GitHub OIDC federation; do not store long-lived publish profiles in source control.
5. Deploy the published application to the configured Azure App Service.
6. Use a protected GitHub environment for production approval and deployment secrets.
7. Report failed build, test, or deployment steps clearly in the workflow summary.

Configure the `production` GitHub environment with these values before enabling deployment:

- **Secrets**: `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, and `AZURE_SUBSCRIPTION_ID`.
- **Variables**: `AZURE_WEBAPP_NAME`.
- **Azure identity**: Grant the federated deployment identity the minimum role needed for the
  target App Service resource and configure the GitHub repository and `master` branch as trusted
  OIDC subjects.

The workflow must remain free of client secrets, publish profiles, subscription keys, and
environment-specific resource identifiers.

## Azure runtime verification

After deployment, verify:

- The App Service starts successfully and serves the home page.
- Both external feed requests can be completed from Azure.
- A successful page shows combined records in the required order.
- Feed failure produces the required error state and no combined results.
- The deployed HTTPS endpoint is reachable and usable by keyboard and mobile visitors.
