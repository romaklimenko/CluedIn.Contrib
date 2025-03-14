using CluedIn.Contrib.Rules.Actions;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Processing;
using NSubstitute;

namespace CluedIn.Contrib.Tests.Rules.Actions;

public class SumRuleActionTest
{
    // private readonly string _apiKey = Guid.NewGuid().ToString();
    private readonly ProcessingContext _context = Substitute.For<ProcessingContext>();

    private static IEntityMetadataPart GetEntityMetadataPart()
    {
        return new EntityMetadataPart
        {
            Properties =
            {
                ["test.amount.int"] = "1",
                ["test.amount.double"] = "3.14",
                ["test.amount.wat"] = "wat",
                ["test.amount.nest.int"] = "100"
            }
        };
    }

    [Fact]
    public void Run_HappyPath()
    {
        // Arrange
        var sum = new SumRuleAction
        {
            SourceVocabularyPrefixFieldValue = "test.amount", TargetVocabularyKeyFieldValue = "test.amount.dq.sum"
        };
        var entityMetadataPart = GetEntityMetadataPart();
        // Act
        var ruleActionResult = sum.Run(
            _context,
            entityMetadataPart,
            false);
        // Assert
        Assert.True(ruleActionResult.IsSuccess);
        Assert.Equal("4.14", entityMetadataPart.Properties["test.amount.dq.sum"]);
    }

    [Fact]
    public void Run_IsPreview()
    {
        // Arrange
        var sum = new SumRuleAction
        {
            SourceVocabularyPrefixFieldValue = "test.amount", TargetVocabularyKeyFieldValue = "test.amount.dq.sum"
        };
        var entityMetadataPart = new EntityMetadataPart();
        // Act
        var ruleActionResult = sum.Run(
            _context,
            entityMetadataPart,
            true);
        // Assert
        Assert.True(ruleActionResult.IsSuccess);
        Assert.True(entityMetadataPart.Properties.ContainsKey("test.amount.dq.sum"));
    }
}
