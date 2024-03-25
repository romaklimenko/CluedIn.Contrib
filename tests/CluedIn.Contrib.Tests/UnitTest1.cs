namespace CluedIn.Contrib.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var class1 = new Class1();
        // Act
        var result = class1.SayHello("World");
        // Assert
        Assert.Equal("Hello, World!", result);
    }
}