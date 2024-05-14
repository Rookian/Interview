using Xunit;

namespace Interview.SetupData;

public class SampleTest : TestBase
{
    [Fact]
    public async Task Should_()
    {
        // Arrange
        Item item1 = null!;
        Item item2 = null!;
        Item item3 = null!;
        Item item4 = null!;
        Publisher publisher = null!;

        await SetupDataAsync(x =>
        {
            item1 = x.AddItem("Item1");
            item2 = x.AddItem("Item2");
            item3 = x.AddItem("Item3");
            item4 = x.AddItem("Item4");
            publisher = x.AddPublisher("Publisher1");
        });

        // Act

        // Assert
    }
}