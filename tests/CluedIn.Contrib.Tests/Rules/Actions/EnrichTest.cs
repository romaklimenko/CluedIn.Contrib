using System.Net;
using System.Text;
using System.Web;
using CluedIn.Contrib.Rules.Actions;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Processing;
using CluedIn.Core.Rules.Models;
using NSubstitute;

namespace CluedIn.Contrib.Tests.Rules.Actions;

public class EnrichTest
{
    private readonly string _apiKey = Guid.NewGuid().ToString();
    private readonly ProcessingContext _context = Substitute.For<ProcessingContext>();

    private static IEntityMetadataPart GetEntityMetadataPart()
    {
        return new EntityMetadataPart { Properties = { ["test.a"] = "a", ["test.b"] = "b", ["test.c"] = "c" } };
    }

    [Fact]
    public void Constructor_WithoutArguments_Succeeds()
    {
        // Arrange
        // Act
        var enrich = new Enrich();
        // Assert
        Assert.Equal("Enrich", enrich.Name);
        Assert.True(enrich.SupportsPreview);
        Assert.Equal(
            new[] { ProcessingRuleScope.DataPart, ProcessingRuleScope.Entity },
            enrich.SupportsProcessingRuleScopes);
    }

    [Fact]
    public void Properties_GetAndSet()
    {
        // Arrange
        var enrich = new Enrich
        {
            // Act
            UrlFieldValue = "A", PayloadFieldValue = "B", VocabularyPrefixFieldValue = "C"
        };
        // Assert
        Assert.Equal("A", enrich.UrlFieldValue);
        Assert.Equal("B", enrich.PayloadFieldValue);
        Assert.Equal("C", enrich.VocabularyPrefixFieldValue);
    }

    [Fact]
    public void Run_NoUrlFieldValue_ReturnsFailure()
    {
        // Arrange
        var enrich = new Enrich();
        // Act
        var ruleActionResult = enrich.Run(
            _context,
            GetEntityMetadataPart(),
            false);
        // Assert
        Assert.False(ruleActionResult.IsSuccess);
        Assert.Equal(
            $"The field {nameof(Enrich.UrlFieldValue)} cannot be empty.",
            ruleActionResult.Messages.Single());
    }

    [Fact]
    public void Run_NoPayloadFieldValue_ReturnsFailure()
    {
        // Arrange
        var enrich = new Enrich { UrlFieldValue = "A" };
        // Act
        var ruleActionResult = enrich.Run(
            _context,
            GetEntityMetadataPart(),
            false);
        // Assert
        Assert.False(ruleActionResult.IsSuccess);
        Assert.Equal(
            $"The field {nameof(Enrich.PayloadFieldValue)} cannot be empty.",
            ruleActionResult.Messages.Single());
    }

    [Fact]
    public void Run_NoVocabularyPrefixFieldValue_ReturnsFailure()
    {
        // Arrange
        var enrich = new Enrich { UrlFieldValue = "A", PayloadFieldValue = "B" };
        // Act
        var ruleActionResult = enrich.Run(
            _context,
            GetEntityMetadataPart(),
            false);
        // Assert
        Assert.False(ruleActionResult.IsSuccess);
        Assert.Equal(
            $"The field {nameof(Enrich.VocabularyPrefixFieldValue)} cannot be empty.",
            ruleActionResult.Messages.Single());
    }

    [Fact]
    public void Run_ReturnsFailure()
    {
        // Arrange
        const string exceptionMessage = "Something went wrong.";
        var httpClient = new HttpClient(
            new MockHttpMessageHandler(
                (_, _) =>
                    throw new NotImplementedException(exceptionMessage)));
        var enrich = new Enrich(httpClient)
        {
            UrlFieldValue = "http://localhost:8888",
            PayloadFieldValue = "test.a,test.c",
            VocabularyPrefixFieldValue = "enrich"
        };
        // Act
        var ruleActiomResult = enrich.Run(
            _context,
            GetEntityMetadataPart(),
            false);
        // Assert
        Assert.False(ruleActiomResult.IsSuccess);
        Assert.Equal(
            exceptionMessage,
            ruleActiomResult.Messages.Single());
    }

    [Fact]
    public void Run_CantParseJson_ReturnsFailure()
    {
        // Arrange
        var httpClient = new HttpClient(
            new MockHttpMessageHandler(
                (_, _) =>
                    Task.FromResult(
                        new HttpResponseMessage { Content = new StringContent("not a JSON") })));
        var enrich = new Enrich(httpClient)
        {
            UrlFieldValue = "http://localhost:8888",
            PayloadFieldValue = "test.a,test.c",
            VocabularyPrefixFieldValue = "enrich"
        };
        // Act
        var ruleActionResult = enrich.Run(
            _context,
            GetEntityMetadataPart(),
            false);
        // Assert
        Assert.False(ruleActionResult.IsSuccess);
        Assert.Equal(
            "'not a JSON' is an invalid JSON literal. Expected the literal 'null'. LineNumber: 0 | BytePositionInLine: 1.",
            ruleActionResult.Messages.Single());
    }

    [Fact]
    public void Run_HappyPath_Enriches()
    {
        // Arrange
        Environment.SetEnvironmentVariable(
            "CLUEDIN_RULE_ACTION_API_KEY",
            _apiKey);
        var httpClient = new HttpClient(
            new MockHttpMessageHandler(SendAsync));
        var enrich = new Enrich(httpClient)
        {
            UrlFieldValue = "http://localhost:8888",
            PayloadFieldValue = "test.a,test.c",
            VocabularyPrefixFieldValue = "enrich"
        };
        var entityMetadataPart = GetEntityMetadataPart();
        // Act
        var ruleActionResult = enrich.Run(
            _context,
            entityMetadataPart,
            false);
        // Assert
        Assert.True(ruleActionResult.IsSuccess);
        Assert.Equal("enriched_a", entityMetadataPart.Properties["enrich.a"]);
        Assert.Equal("enriched_b", entityMetadataPart.Properties["enrich.b"]);
    }

    [Fact]
    public void Run_HappyPathPreview_Enriches()
    {
        // Arrange
        Environment.SetEnvironmentVariable(
            "CLUEDIN_RULE_ACTION_API_KEY",
            _apiKey);
        var httpClient = new HttpClient(
            new MockHttpMessageHandler(SendAsync));
        var enrich = new Enrich(httpClient)
        {
            UrlFieldValue = "http://localhost:8888",
            PayloadFieldValue = "test.a,test.c",
            VocabularyPrefixFieldValue = "enrich"
        };
        var entityMetadataPart = GetEntityMetadataPart();
        // Act
        var ruleActionResult = enrich.Run(
            _context,
            entityMetadataPart,
            true);
        // Assert
        Assert.True(ruleActionResult.IsSuccess);
        Assert.Equal("preview_enriched_a", entityMetadataPart.Properties["enrich.a"]);
        Assert.Equal("preview_enriched_b", entityMetadataPart.Properties["enrich.b"]);
    }

    [Fact]
    public void Run_NoApiKey_Forbidden()
    {
        // Arrange
        Environment.SetEnvironmentVariable(
            "CLUEDIN_RULE_ACTION_API_KEY",
            null);
        var httpClient = new HttpClient(
            new MockHttpMessageHandler(SendAsync));
        var enrich = new Enrich(httpClient)
        {
            UrlFieldValue = "http://localhost:8888",
            PayloadFieldValue = "test.a,test.c",
            VocabularyPrefixFieldValue = "enrich"
        };
        var entityMetadataPart = GetEntityMetadataPart();
        // Act
        var ruleActionResult = enrich.Run(
            _context,
            entityMetadataPart,
            false);
        // Assert
        Assert.True(ruleActionResult.IsSuccess);
        Assert.Equal(
            "Forbidden",
            entityMetadataPart.Properties["enrich.status"]);
    }

    private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken token)
    {
        if (!message.Headers.TryGetValues("X-ApiKey", out var apiKeys)
            || !string.Equals(apiKeys.First(), _apiKey, StringComparison.Ordinal))
        {
            return new HttpResponseMessage
            {
                Content = new StringContent("{ \"status\": \"Forbidden\" }"), StatusCode = HttpStatusCode.Forbidden
            };
        }

        if (message.Content is not FormUrlEncodedContent)
        {
            return new HttpResponseMessage { Content = new StringContent("nope") };
        }

        var formData = HttpUtility.ParseQueryString(
            await message.Content.ReadAsStringAsync(CancellationToken.None)
                .ConfigureAwait(false));

        return new HttpResponseMessage
        {
            Content = new StringContent(
                formData["is_preview"] == "true"
                    ? "{ \"a\": \"preview_enriched_a\", \"b\": \"preview_enriched_b\" }"
                    : "{ \"a\": \"enriched_a\", \"b\": \"enriched_b\" }",
                Encoding.UTF8,
                "application/json")
        };
    }
}
