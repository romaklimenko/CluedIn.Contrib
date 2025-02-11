using CluedIn.Contrib.Processors.Post.MergeDataParts;
using NSubstitute;
using ExecutionContext = CluedIn.Core.ExecutionContext;

namespace CluedIn.Contrib.Tests.Processors.Post.MergeDataParts;

public class LookupValidationMergeDataPartsPostProcessorTest : IClassFixture<EnvironmentVariablesFixture>
{
    private readonly ExecutionContext _executionContext = Substitute.For<ExecutionContext>();

    [Fact]
    public void Accepts_True()
    {
        // Arrange
        var processor = new LookupValidationMergeDataPartsPostProcessor();
        // Act
        // Assert
        Assert.True(processor.IsEnabled(_executionContext));
    }
}
