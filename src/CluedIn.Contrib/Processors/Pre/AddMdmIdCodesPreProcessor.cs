using CluedIn.Contrib.Extensions;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Processing.Processors.PreProcessing;
using ExecutionContext = CluedIn.Core.ExecutionContext;

namespace CluedIn.Contrib.Processors.Pre;

public class AddMdmIdCodesPreProcessor : IPreProcessor
{
    public bool Accepts(ExecutionContext context, IEnumerable<IEntityCode> codes)
    {
        return this.IsEnabled();
    }

    public void Process(ExecutionContext context, IEntityMetadataPart metadata, IDataPart? data)
    {
        if (data?.EntityData == null)
        {
            return;
        }

        // Add Entity Id
        var entityId = EntityIdGenerator.GenerateId(
            context.Organization,
            data.EntityData.EntityType,
            data.EntityData.OriginEntityCode);
        data.EntityData.Codes.Add(
            new EntityCode(data.EntityData.EntityType, CodeOrigin.CluedIn.CreateSpecific("entityId"), entityId));

        // Add Global Id
        var globalId = data.EntityData.OriginEntityCode.ToString()!.ToGuid();
        data.EntityData.Codes.Add(
            new EntityCode(data.EntityData.EntityType, CodeOrigin.CluedIn.CreateSpecific("globalId"), globalId));
    }
}
