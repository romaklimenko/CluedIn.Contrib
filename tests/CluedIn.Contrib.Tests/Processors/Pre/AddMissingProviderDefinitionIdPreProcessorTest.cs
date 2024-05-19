using CluedIn.Contrib.Extensions;
using CluedIn.Contrib.Processors.Pre;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using NSubstitute;
using ExecutionContext = CluedIn.Core.ExecutionContext;

namespace CluedIn.Contrib.Tests.Processors.Pre;

public class AddMissingProviderDefinitionIdPreProcessorTest
{
    private readonly ExecutionContext _executionContext = Substitute.For<ExecutionContext>();

    [Fact]
    public void Accepts_True()
    {
        // Arrange
        var processor = new AddMissingProviderDefinitionIdPreProcessor();
        // Act
        // Assert
        Assert.True(processor.Accepts(_executionContext, Enumerable.Empty<IEntityCode>()));
    }

    [Fact]
    public void Process_WithProviderDefinitionId_NoChanges()
    {
        // Arrange
        var processor = new AddMissingProviderDefinitionIdPreProcessor();
        var originProviderDefinitionId = Guid.NewGuid();
        var dataPart = new DataPart
        {
            EntityData = { OriginEntityCode = new EntityCode("/Person", "Dynamics365(Contact)", 1) },
            OriginProviderDefinitionId = originProviderDefinitionId
        };
        // Act
        processor.Process(_executionContext, null!, dataPart);
        // Assert
        Assert.Equal(originProviderDefinitionId, dataPart.OriginProviderDefinitionId);
        Assert.Equal(originProviderDefinitionId, dataPart.EntityData.ProviderDefinitionId);
    }

    [Fact]
    public void Process_WithoutProviderDefinitionId_SetsId()
    {
        // Arrange
        var processor = new AddMissingProviderDefinitionIdPreProcessor();
        var expectedOriginProviderDefinitionId = "Dynamics365".ToGuid();
        var dataPart = new DataPart { OriginEntityCode = new EntityCode("/Person", "Dynamics365(Contact)", 1) };
        // Act
        processor.Process(_executionContext, null!, dataPart);
        // Assert
        Assert.Equal(expectedOriginProviderDefinitionId, dataPart.OriginProviderDefinitionId);
        Assert.Equal(expectedOriginProviderDefinitionId, dataPart.EntityData.ProviderDefinitionId);
    }
}
