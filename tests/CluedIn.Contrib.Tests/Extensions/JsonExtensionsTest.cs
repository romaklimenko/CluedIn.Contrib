using System.Text.Json;
using CluedIn.Contrib.Extensions;

namespace CluedIn.Contrib.Tests.Extensions;

public class JsonExtensionsTest
{
    [Fact]
    public void FlattenTest()
    {
        // Arrange
        var json = "{ \"a\": { \"b\": 1 }, \"c\": \"d\", \"e\": [ { \"f\": \"f0\" }, { \"f\": \"f1\" } ] }";
        var jsonDocument = JsonDocument.Parse(json);

        // Act
        var result = jsonDocument.Flatten();

        // Assert
        Assert.Equal(4, result.Keys.Count);
        Assert.Equal("1", result["a.b"]);
        Assert.Equal("d", result["c"]);
        Assert.Equal("f0", result["e.0.f"]);
        Assert.Equal("f1", result["e.1.f"]);
    }
}
