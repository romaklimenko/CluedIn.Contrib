using System.Diagnostics.CodeAnalysis;
using CluedIn.Contrib.Extensions;
using CluedIn.Contrib.Processors.Post.MergeDataParts;
using CluedIn.Contrib.Processors.Pre;

namespace CluedIn.Contrib.Tests.Extensions;

public class ProcessorExtensionsTest
{
    public ProcessorExtensionsTest()
    {
        Environment.SetEnvironmentVariable(PreProcessorEnvironmentVariableName, "true");
        Environment.SetEnvironmentVariable(MergeDataPartsPostProcessorEnvironmentVariableName, "true");
    }

    private static string PreProcessorEnvironmentVariableName =>
        $"CluedIn_AppSettings__{nameof(AddMissingProviderDefinitionIdPreProcessor)}_Enabled";

    private static string MergeDataPartsPostProcessorEnvironmentVariableName =>
        $"CluedIn_AppSettings__{nameof(OriginsMergeDataPartsPostProcessor)}_Enabled";

    [Fact(Skip =
        "The problem is that we can't clear cache from ConfigurationManagerEx, " +
        "so no matter what we set, it will always return the same value.")]
    [SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped")]
    public void IsEnabled_False()
    {
        // Arrange
        var obj = new AddMissingProviderDefinitionIdPreProcessor();
        // Act
        var actual = obj.IsEnabled();
        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void IPreProcessorIsEnabled_True()
    {
        // Arrange
        var processor = new AddMissingProviderDefinitionIdPreProcessor();
        // Act
        var actual = processor.IsEnabled();
        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void IMergeDataPartsPostProcessorIsEnabled_True()
    {
        // Arrange
        var processor = new OriginsMergeDataPartsPostProcessor();
        // Act
        var actual = processor.IsEnabled();
        // Assert
        Assert.True(actual);
    }
}
