using CluedIn.Contrib.Extensions;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Processing.Processors.PreProcessing;
using Serilog;
using ExecutionContext = CluedIn.Core.ExecutionContext;

namespace CluedIn.Contrib.Processors.Pre;

public class AddMissingProviderDefinitionIdPreProcessor : IPreProcessor
{
    public bool Accepts(ExecutionContext context, IEnumerable<IEntityCode> codes)
    {
        return true;
    }

    public void Process(ExecutionContext context, IEntityMetadataPart metadata, IDataPart? data)
    {
        if (data?.EntityData == null)
        {
            return;
        }

        if (data.EntityData.ProviderDefinitionId != null)
        {
            return;
        }

        var originProviderDefinitionId = data
            .EntityData?
            .OriginEntityCode?
            .Origin?.Code?.ToGuid();

        if (originProviderDefinitionId == null)
        {
            Log.Warning(
                "Processor: {Processor}. originProviderDefinitionId is null.",
                nameof(AddMissingProviderDefinitionIdPreProcessor));
            return;
        }

        data.OriginProviderDefinitionId = originProviderDefinitionId;
    }
}
