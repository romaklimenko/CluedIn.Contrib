build:
	@echo "Building CluedIn.Contrib"
	@dotnet build

test:
	@echo "Running tests"
	@dotnet test

coverage: build
	@echo "Running test coverage"
	@coverlet ./tests/CluedIn.Contrib.Tests/bin/Debug/net6.0/CluedIn.Contrib.Tests.dll --target "dotnet" --targetargs "test ./tests/CluedIn.Contrib.Tests/CluedIn.Contrib.Tests.csproj --no-build" --format lcov

coverage-report: coverage
	@echo "Generating test coverage report"
	@reportgenerator -reports:coverage.info -targetdir:coverage -reporttypes:Html
