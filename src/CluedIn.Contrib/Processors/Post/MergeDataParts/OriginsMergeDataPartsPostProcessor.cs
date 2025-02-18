using CluedIn.Contrib.Extensions;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Processing;
using CluedIn.Processing;
using ExecutionContext = CluedIn.Core.ExecutionContext;

namespace CluedIn.Contrib.Processors.Post.MergeDataParts;

public class OriginsMergeDataPartsPostProcessor : IMergeDataPartsPostProcessor
{
    public bool IsEnabled(ExecutionContext context)
    {
        return this.IsEnabled();
    }

    public void Process(ProcessingContext context, IProcessedEntityMetadataPart entityProcessedData,
        IEnumerable<IDataPart> orderedDataParts)
    {
        entityProcessedData.Properties["system.origins"] = string.Join(
            ",",
            entityProcessedData.Codes
                .Where(x => !new[]
                {
                    // smells bad but: https://github.com/CluedIn-io/CluedIn/tree/develop/Code/Providers/System
                    "CluedIn", "File Data Source", "Endpoint Data Source", "Database Data Source",
                    "Excel Plugin Source", "Landing Zone Data Source", "Manual Hierarchy Builder Source"
                }.Any(s => x.Origin.Code.Contains(s, StringComparison.InvariantCultureIgnoreCase)))
                .Select(x => x.Origin)
                .Distinct()
                .OrderBy(x => x));
    }

    public int Priority => 1;
}
