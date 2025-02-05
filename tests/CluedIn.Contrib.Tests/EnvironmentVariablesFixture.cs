using CluedIn.Contrib.Processors.Post.MergeDataParts;
using CluedIn.Contrib.Processors.Pre;

namespace CluedIn.Contrib.Tests;

public class EnvironmentVariablesFixture
{
    public EnvironmentVariablesFixture()
    {
        // Set environment variables before any tests run
        Environment.SetEnvironmentVariable("CluedIn_AppSettings__FooPreProcessor_Enabled", "true");
        Environment.SetEnvironmentVariable("CluedIn_AppSettings__FooMergeDataPartsPostProcessor_Enabled", "true");

        Environment.SetEnvironmentVariable($"CluedIn_AppSettings__{nameof(AddMdmIdCodesPreProcessor)}_Enabled", "true");
        Environment.SetEnvironmentVariable(
            $"CluedIn_AppSettings__{nameof(AddMissingProviderDefinitionIdPreProcessor)}_Enabled", "true");
        Environment.SetEnvironmentVariable($"CluedIn_AppSettings__{nameof(AddSequenceNumberPreProcessor)}_Enabled",
            "true");
        Environment.SetEnvironmentVariable($"CluedIn_AppSettings__{nameof(OriginsMergeDataPartsPostProcessor)}_Enabled",
            "true");
    }
}
