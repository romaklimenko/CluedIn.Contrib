using System.Globalization;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Processing;
using CluedIn.Core.Rules;
using CluedIn.Core.Rules.Models;
using CluedIn.Rules.Actions;

namespace CluedIn.Contrib.Rules.Actions;

public class SumRuleAction : BaseRuleAction, IScopedRuleAction
{
    private string SourceVocabularyPrefixWithDot =>
        string.IsNullOrWhiteSpace(SourceVocabularyPrefixFieldValue)
            ? string.Empty
            : SourceVocabularyPrefixFieldValue + ".";

    [RuleProperty(
        FriendlyName = "Source Vocabulary Prefix",
        Kind = ActionControlKind.Text,
        IsRequired = true)]
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public string? SourceVocabularyPrefixFieldValue { get; set; }

    [RuleProperty(
        FriendlyName = "Target Vocabulary Key",
        Kind = ActionControlKind.Text,
        IsRequired = true)]
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public string? TargetVocabularyKeyFieldValue { get; set; }

    public override string Name => "Sum";
    public override bool SupportsPreview => true;

    public IEnumerable<ProcessingRuleScope> SupportsProcessingRuleScopes =>
        new[] { ProcessingRuleScope.DataPart, ProcessingRuleScope.Entity };

    public override RuleActionResult Run(ProcessingContext context, IEntityMetadataPart entityMetadataPart,
        bool isPreview)
    {
        if (string.IsNullOrWhiteSpace(SourceVocabularyPrefixFieldValue) ||
            string.IsNullOrWhiteSpace(TargetVocabularyKeyFieldValue))
        {
            return RuleActionResult.Failure("Source Vocabulary Prefix is required");
        }

        var sum = 0.0;

        if (isPreview)
        {
            entityMetadataPart.Properties[TargetVocabularyKeyFieldValue] =
                (new Random().NextDouble() * 1000).ToString("F2", CultureInfo.InvariantCulture);
            return RuleActionResult.Success("OK");
        }

        foreach (var property in entityMetadataPart.Properties)
        {
            if (!property.Key.StartsWith(SourceVocabularyPrefixWithDot))
            {
                continue;
            }

            var keyWithoutPrefix = property.Key[SourceVocabularyPrefixWithDot.Length..];
            if (keyWithoutPrefix.Contains('.') || keyWithoutPrefix.Contains('+'))
            {
                continue;
            }

            if (double.TryParse(property.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                sum += value;
            }
        }

        entityMetadataPart.Properties[TargetVocabularyKeyFieldValue] = sum.ToString("F2", CultureInfo.InvariantCulture);

        return RuleActionResult.Success("OK");
    }
}
