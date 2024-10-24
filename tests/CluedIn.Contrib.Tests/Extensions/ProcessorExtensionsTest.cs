using CluedIn.Contrib.Extensions;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Processing;
using CluedIn.Processing;
using CluedIn.Processing.Processors.PreProcessing;
using ExecutionContext = CluedIn.Core.ExecutionContext;

namespace CluedIn.Contrib.Tests.Extensions;

internal class FooPreProcessor : IPreProcessor
{
    public bool Accepts(ExecutionContext context, IEnumerable<IEntityCode> codes)
    {
        throw new NotImplementedException();
    }

    public void Process(ExecutionContext context, IEntityMetadataPart metadata, IDataPart data)
    {
        throw new NotImplementedException();
    }
}

internal class BarPreProcessor : IPreProcessor
{
    public bool Accepts(ExecutionContext context, IEnumerable<IEntityCode> codes)
    {
        throw new NotImplementedException();
    }

    public void Process(ExecutionContext context, IEntityMetadataPart metadata, IDataPart data)
    {
        throw new NotImplementedException();
    }
}

internal class FooMergeDataPartsPostProcessor : IMergeDataPartsPostProcessor
{
    public bool IsEnabled(ExecutionContext context)
    {
        throw new NotImplementedException();
    }

    public void Process(ProcessingContext context, IProcessedEntityMetadataPart entityProcessedData,
        IEnumerable<IDataPart> orderedDataParts)
    {
        throw new NotImplementedException();
    }

    public int Priority => 1;
}

internal class BarMergeDataPartsPostProcessor : IMergeDataPartsPostProcessor
{
    public bool IsEnabled(ExecutionContext context)
    {
        throw new NotImplementedException();
    }

    public void Process(ProcessingContext context, IProcessedEntityMetadataPart entityProcessedData,
        IEnumerable<IDataPart> orderedDataParts)
    {
        throw new NotImplementedException();
    }

    public int Priority => 1;
}

public class ProcessorExtensionsTest : IClassFixture<EnvironmentVariablesFixture>
{
    [Fact]
    public void IsEnabled_True()
    {
        // Arrange
        var preProcessor = new FooPreProcessor();
        var mergeDataPartsPostProcessor = new FooMergeDataPartsPostProcessor();
        // Act
        // Assert
        Assert.True(preProcessor.IsEnabled());
        Assert.True(mergeDataPartsPostProcessor.IsEnabled());
    }

    [Fact]
    public void IsEnabled_False()
    {
        // Arrange
        var preProcessor = new BarPreProcessor();
        var mergeDataPartsPostProcessor = new BarMergeDataPartsPostProcessor();
        // Act
        // Assert
        Assert.False(preProcessor.IsEnabled());
        Assert.False(mergeDataPartsPostProcessor.IsEnabled());
    }
}
