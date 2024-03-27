using System.Text.Json;
using CluedIn.Contrib.Extensions;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Processing;
using CluedIn.Core.Rules;
using CluedIn.Core.Rules.Models;
using CluedIn.Rules.Actions;
using Serilog;

namespace CluedIn.Contrib.Rules.Actions;

public class Enrich : BaseRuleAction, IScopedRuleAction
{
    private readonly HttpClient _httpClient;

    public Enrich() : this(new HttpClient())
    {
    }

    public Enrich(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [RuleProperty(
        FriendlyName = "URL",
        Kind = ActionControlKind.Text,
        IsRequired = true)]
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public string? UrlFieldValue { get; set; }

    [RuleProperty(
        FriendlyName = "Payload (comma-separated list of vocabulary keys)",
        Kind = ActionControlKind.Text,
        IsRequired = true)]
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public string? PayloadFieldValue { get; set; }

    [RuleProperty(
        FriendlyName = "Vocabulary Prefix",
        Kind = ActionControlKind.Text,
        IsRequired = true)]
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public string? VocabularyPrefixFieldValue { get; set; }

    [RuleProperty(
        FriendlyName = "Cache Expiration Time (in minutes)",
        Kind = ActionControlKind.Text,
        IsRequired = true)]
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public string? CacheExpirationTimeInMinutesFieldValue { get; set; }

    public override string Name => "Enrich";
    public override bool SupportsPreview => true;

    public IEnumerable<ProcessingRuleScope> SupportsProcessingRuleScopes =>
        new[] { ProcessingRuleScope.DataPart, ProcessingRuleScope.Entity };

    public override RuleActionResult Run(ProcessingContext context, IEntityMetadataPart entityMetadataPart,
        bool isPreview)
    {
        try
        {
            return RunAsync(context, entityMetadataPart, isPreview)
                .GetAwaiter()
                .GetResult();
        }
        catch (Exception e)
        {
            Log.Logger.Error(
                e,
                "Error processing \"{OriginEntityCode}\". Message: {Message}",
                entityMetadataPart.OriginEntityCode,
                e.Message);
            return RuleActionResult.Failure(e.Message);
        }
    }

    private async Task<RuleActionResult> RunAsync(ProcessingContext context, IEntityMetadataPart entityMetadataPart,
        bool isPreview)
    {
        if (string.IsNullOrWhiteSpace(UrlFieldValue))
        {
            return RuleActionResult.Failure(
                $"The field {nameof(UrlFieldValue)} cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(PayloadFieldValue))
        {
            return RuleActionResult.Failure(
                $"The field {nameof(PayloadFieldValue)} cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(VocabularyPrefixFieldValue))
        {
            return RuleActionResult.Failure(
                $"The field {nameof(VocabularyPrefixFieldValue)} cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(
                CacheExpirationTimeInMinutesFieldValue))
        {
            return RuleActionResult.Failure(
                $"The field {nameof(CacheExpirationTimeInMinutesFieldValue)} cannot be empty.");
        }

        // TODO: Handle cache
        if (!int.TryParse(CacheExpirationTimeInMinutesFieldValue,
                out var cacheExpirationTimeInMinutes))
        {
            return RuleActionResult.Failure(
                $"Can't parse {CacheExpirationTimeInMinutesFieldValue}.");
        }

        var formUrlEncodedContent = PayloadFieldValue
            .Split(',')
            .Select(x => x.Trim())
            .Where(vocabKey => entityMetadataPart.Properties
                .ContainsKey(vocabKey))
            .ToDictionary(
                vocabKey => vocabKey,
                vocabKey => entityMetadataPart.Properties[vocabKey]);

        if (isPreview)
        {
            formUrlEncodedContent["is_preview"] = "true";
        }

        using var httpResponseMessage = await _httpClient
            .PostAsync(
                UrlFieldValue,
                new FormUrlEncodedContent(formUrlEncodedContent))
            .ConfigureAwait(false);

        var stream = await httpResponseMessage.Content.ReadAsStreamAsync()
            .ConfigureAwait(false);
        await using var _ = stream.ConfigureAwait(false);
        using var jsonDocument = await JsonDocument.ParseAsync(stream)
            .ConfigureAwait(false);
        var vocabularyPrefix = VocabularyPrefixFieldValue.TrimEnd('.');
        foreach (var (k, v) in jsonDocument.Flatten())
        {
            entityMetadataPart.Properties[$"{vocabularyPrefix}.{k}"] = v;
        }

        return RuleActionResult.Success("OK");
    }
}
