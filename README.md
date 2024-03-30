# CluedIn.Contrib

CluedIn Contrib is a collection of features that are not part of the core CluedIn platform.

## Build status:

develop:

[![Build Status](https://dev.azure.com/CluedIn-io/CluedIn/_apis/build/status%2FCluedIn-io.CluedIn.Contrib?branchName=develop)](https://dev.azure.com/CluedIn-io/CluedIn/_build/latest?definitionId=388&branchName=develop)

master:

[![Build Status](https://dev.azure.com/CluedIn-io/CluedIn/_apis/build/status%2FCluedIn-io.CluedIn.Contrib?branchName=master)](https://dev.azure.com/CluedIn-io/CluedIn/_build/latest?definitionId=388&branchName=master)


## Prerequisites

- .NET Core 6.0 SDK
- `dotnet tool install --global coverlet.console`
- `dotnet tool install --global dotnet-reportgenerator-globaltool`

## Build solution:

```shell
dotnet build
```

## Run tests

```shell
dotnet test
```

## Run tests with coverage:

```shell
coverlet ./tests/CluedIn.Contrib.Tests/bin/Debug/net6.0/CluedIn.Contrib.Tests.dll --target "dotnet" --targetargs "test ./tests/CluedIn.Contrib.Tests/CluedIn.Contrib.Tests.csproj --no-build" --format lcov
```

Generate coverage report:

```shell
reportgenerator -reports:coverage.info -targetdir:coverage -reporttypes:Html
```
