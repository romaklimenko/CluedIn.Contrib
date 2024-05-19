using CluedIn.Contrib.Extensions;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Processing.Processors.PreProcessing;
using ExecutionContext = CluedIn.Core.ExecutionContext;

namespace CluedIn.Contrib.Processors.Pre;

public class AddMissingProviderDefinitionIdPreProcessor : IPreProcessor
{
    public bool Accepts(ExecutionContext context, IEnumerable<IEntityCode> codes)
    {
        return true;
    }

    public void Process(ExecutionContext context, IEntityMetadataPart metadata, IDataPart data)
    {
        if (data.EntityData.ProviderDefinitionId != null)
        {
            return;
        }

        data.OriginProviderDefinitionId = data.OriginEntityCode.Origin.Code.ToGuid();
    }
}
