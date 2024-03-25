# CluedIn.Contrib

CluedIn Contrib is a collection of features that are not part of the core CluedIn platform.

## Prerequisites

- .NET Core 6.0 SDK
- `dotnet tool install --global coverlet.console`
- `dotnet tool install --global dotnet-reportgenerator-globaltool`


Run tests with coverage:

`coverlet ./tests/CluedIn.Contrib.Tests/bin/Debug/net6.0/CluedIn.Contrib.Tests.dll --target "dotnet" --targetargs "test ./tests/CluedIn.Contrib.Tests/CluedIn.Contrib.Tests.csproj --no-build" --format lcov`

Generate coverage report:

`reportgenerator -reports:coverage.info -targetdir:coveragereport -reporttypes:Html`
