using CluedIn.Contrib.Processors.Post.MergeDataParts;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Processing;
using NSubstitute;
using ExecutionContext = CluedIn.Core.ExecutionContext;

namespace CluedIn.Contrib.Tests.Processors.Post.MergeDataParts;

public class OriginsMergeDataPartsPostProcessorTest
{
    private readonly ExecutionContext _executionContext = Substitute.For<ExecutionContext>();
    private readonly ProcessingContext _processingContext = Substitute.For<ProcessingContext>();

    [Fact]
    public void IsEnabled_True()
    {
        // Arrange
        var processor = new OriginsMergeDataPartsPostProcessor();
        // Act
        // Assert
        Assert.True(processor.IsEnabled(_executionContext));
    }

    [Fact]
    public void Priority_One()
    {
        // Arrange
        var processor = new OriginsMergeDataPartsPostProcessor();
        // Act
        // Assert
        Assert.Equal(1, processor.Priority);
    }

    [Fact]
    public void NoCodes()
    {
        // Arrange
        var processor = new OriginsMergeDataPartsPostProcessor();
        IProcessedEntityMetadataPart entityProcessedData = new ProcessedEntityMetadataPart();
        // Act
        processor.Process(_processingContext, entityProcessedData, Enumerable.Empty<IDataPart>());
        // Assert
        Assert.False(entityProcessedData.Properties.ContainsKey("system.origins"));
    }

    [Fact]
    public void OneCodeOneOrigin()
    {
        // Arrange
        var processor = new OriginsMergeDataPartsPostProcessor();
        IProcessedEntityMetadataPart entityProcessedData = new ProcessedEntityMetadataPart();
        entityProcessedData.Codes.Add(new EntityCode("/Person", "Salesforce", 1));
        // Act
        processor.Process(_processingContext, entityProcessedData, Enumerable.Empty<IDataPart>());
        // Assert
        Assert.Equal("Salesforce", entityProcessedData.Properties["system.origins"]);
    }

    [Fact]
    public void TwoCodesOneOrigin()
    {
        // Arrange
        var processor = new OriginsMergeDataPartsPostProcessor();
        IProcessedEntityMetadataPart entityProcessedData = new ProcessedEntityMetadataPart();
        entityProcessedData.Codes.Add(new EntityCode("/Person", "Salesforce", 1));
        entityProcessedData.Codes.Add(new EntityCode("/Person", "Salesforce", 2));
        // Act
        processor.Process(_processingContext, entityProcessedData, Enumerable.Empty<IDataPart>());
        // Assert
        Assert.Equal("Salesforce", entityProcessedData.Properties["system.origins"]);
    }

    [Fact]
    public void TwoCodesTwoOrigins()
    {
        // Arrange
        var processor = new OriginsMergeDataPartsPostProcessor();
        IProcessedEntityMetadataPart entityProcessedData = new ProcessedEntityMetadataPart();
        entityProcessedData.Codes.Add(new EntityCode("/Person", "Salesforce", 1));
        entityProcessedData.Codes.Add(new EntityCode("/Person", "Dynamics365", 2));
        // Act
        processor.Process(_processingContext, entityProcessedData, Enumerable.Empty<IDataPart>());
        // Assert
        Assert.Equal("Dynamics365,Salesforce", entityProcessedData.Properties["system.origins"]);
    }

    [Fact]
    public void ThreeCodesTwoOriginsOneCluedIn()
    {
        // Arrange
        var processor = new OriginsMergeDataPartsPostProcessor();
        IProcessedEntityMetadataPart entityProcessedData = new ProcessedEntityMetadataPart();
        entityProcessedData.Codes.Add(new EntityCode("/Person", "Salesforce", 1));
        entityProcessedData.Codes.Add(new EntityCode("/Person", "Dynamics365", 2));
        entityProcessedData.Codes.Add(new EntityCode("/Person", "CluedIn(name)", "Bob"));
        // Act
        processor.Process(_processingContext, entityProcessedData, Enumerable.Empty<IDataPart>());
        // Assert
        Assert.Equal("Dynamics365,Salesforce", entityProcessedData.Properties["system.origins"]);
    }
}
