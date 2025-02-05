using CluedIn.Contrib.Processors.Pre;
using CluedIn.Core.Data;
using NSubstitute;
using ExecutionContext = CluedIn.Core.ExecutionContext;

namespace CluedIn.Contrib.Tests.Processors.Pre;

public class AddSequenceNumberPreProcessorTest : IClassFixture<EnvironmentVariablesFixture>
{
    private readonly ExecutionContext _executionContext = Substitute.For<ExecutionContext>();

    [Fact]
    public void Accepts_True()
    {
        // Arrange
        var processor = new AddSequenceNumberPreProcessor();
        // Act
        // Assert
        Assert.True(processor.Accepts(_executionContext, Enumerable.Empty<IEntityCode>()));
    }
}
