using System.Reflection;
using CluedIn.Contrib.Processors.Pre;
using CluedIn.Core;
using CluedIn.Core.Accounts;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using NSubstitute;
using ExecutionContext = CluedIn.Core.ExecutionContext;

namespace CluedIn.Contrib.Tests.Processors.Pre;

public class AddMdmIdCodesPreProcessorTest : IClassFixture<EnvironmentVariablesFixture>
{
    private readonly ExecutionContext _executionContext = Substitute.For<ExecutionContext>();

    public AddMdmIdCodesPreProcessorTest()
    {
        var organization = Substitute.For<IOrganization, IIdentifiable<Guid>>();

        organization.Id.Returns(new Guid("7CCF4580-738F-4ABD-BFA6-42AD665E0A24"));

        var fieldInfo = typeof(ExecutionContext)
            .GetField("organization", BindingFlags.NonPublic | BindingFlags.Instance);
        fieldInfo!.SetValue(_executionContext, organization);
    }

    [Fact]
    public void Accepts_True()
    {
        // Arrange
        var processor = new AddMdmIdCodesPreProcessor();
        // Act
        // Assert
        Assert.True(processor.Accepts(_executionContext, Enumerable.Empty<IEntityCode>()));
    }

    [Fact]
    public void Process_WithEntityData_AddsTwoCodes()
    {
        // Arrange
        var processor = new AddMdmIdCodesPreProcessor();
        var dataPart = new DataPart
        {
            EntityData =
            {
                EntityType = "/Person", OriginEntityCode = new EntityCode("/Person", "Dynamics365(Contact)", 1)
            }
        };
        var expectedEntityId = EntityIdGenerator.GenerateId(
                _executionContext.Organization,
                dataPart.EntityData.EntityType,
                dataPart.EntityData.OriginEntityCode)
            .ToString();
        // Act
        processor.Process(_executionContext, dataPart.EntityData, dataPart);
        // Assert
        Assert.Equal(
            expectedEntityId,
            dataPart.EntityData.Codes.Single(x => x.Origin.Id == "entityId").Value);
        Assert.Equal("28416f74-0117-5a6b-a8bc-cee5db00a168",
            dataPart.EntityData.Codes.Single(x => x.Origin.Id == "globalId").Value);
    }

    [Fact]
    public void Process_WithoutEntityData_DoNothing()
    {
        // Arrange
        var processor = new AddMdmIdCodesPreProcessor();
        // Act
        processor.Process(_executionContext, null!, null);
        // Assert
    }
}
