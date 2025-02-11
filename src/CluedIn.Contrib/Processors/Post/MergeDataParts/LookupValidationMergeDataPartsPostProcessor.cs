using CluedIn.Contrib.Extensions;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Data.Vocabularies;
using CluedIn.Core.Processing;
using CluedIn.Processing;
using ExecutionContext = CluedIn.Core.ExecutionContext;

namespace CluedIn.Contrib.Processors.Post.MergeDataParts;

public class LookupValidationMergeDataPartsPostProcessor : IMergeDataPartsPostProcessor
{
    private static readonly string s_entityCodePostfix = $"-{Vocabularies.CluedInLookupDataVocabulary.EntityCode.Name}";

    public bool IsEnabled(ExecutionContext context)
    {
        return this.IsEnabled();
    }

    public void Process(ProcessingContext context, IProcessedEntityMetadataPart entityProcessedData,
        IEnumerable<IDataPart> orderedDataParts)
    {
        var entityCodeKeys = entityProcessedData.Properties.Where(x => x.Key.EndsWith(s_entityCodePostfix))
            .Select(x => x.Key);

        foreach (var entityCodeKey in entityCodeKeys)
        {
            var lookupKey = entityCodeKey[..^s_entityCodePostfix.Length];
            if (!entityProcessedData.Properties.ContainsKey(lookupKey) ||
                !entityProcessedData.Properties.TryGetValue(entityCodeKey, out var entityIdKeyValue))
            {
                continue;
            }

            var tag = new Tag($"Invalid Lookup Value - {lookupKey}");

            // this code is obviously not the best way to check if the entity id is invalid
            // if the LookupProcessor implementation is changed, this code will need to be updated
            // however, this is a simple way to check if the lookup value is invalid
            if (entityIdKeyValue.StartsWith("[Invalid ("))
            {
                entityProcessedData.Tags.Add(tag);
            }
            else
            {
                entityProcessedData.Tags.Remove(tag);
            }
        }
    }

    public int Priority => 1;
}
