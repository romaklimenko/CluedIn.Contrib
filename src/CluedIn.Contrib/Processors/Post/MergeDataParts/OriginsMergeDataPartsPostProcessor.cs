using CluedIn.Core.Data.Parts;
using CluedIn.Core.Processing;
using CluedIn.Processing;
using ExecutionContext = CluedIn.Core.ExecutionContext;

namespace CluedIn.Contrib.Processors.Post.MergeDataParts;

public class OriginsMergeDataPartsPostProcessor : IMergeDataPartsPostProcessor
{
    public bool IsEnabled(ExecutionContext context)
    {
        return true;
    }

    public void Process(ProcessingContext context, IProcessedEntityMetadataPart entityProcessedData,
        IEnumerable<IDataPart> orderedDataParts)
    {
        entityProcessedData.Properties["system.origins"] = string.Join(
            ",",
            entityProcessedData.Codes
                .Where(x => !x.Origin.Code.Equals("CluedIn", StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Origin)
                .Distinct()
                .OrderBy(x => x));
    }

    public int Priority => 1;
}
