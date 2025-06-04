using Moq;
using SuperStarTrek.Space;
using SuperStarTrek.Systems;
using SuperStarTrek.Commands;
using Games.Common.IO;
using Games.Common.Randomness;
using SuperStarTrek.Objects;

namespace SuperStarTrek.Tests.Systems
{

    public class ShortRangeSensorsTests
    {
        [Test]
        public void ExecuteCommandCore_ReturnsOk()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockGalaxy = new Mock<Galaxy>(mockRandom.Object);
            var mockGame = new Mock<Game>(mockIO.Object, mockRandom.Object);
            var quadrant = new Mock<IQuadrant>();

            var srs = new ShortRangeSensors(mockEnterprise.Object, mockGalaxy.Object, mockGame.Object, mockIO.Object);
            var result = srs.ExecuteCommandCore(quadrant.Object);
            Assert.That(result, Is.EqualTo(CommandResult.Ok));
        }
    }
}
