using Moq;
using SuperStarTrek.Space;
using SuperStarTrek.Systems;
using SuperStarTrek.Commands;
using Games.Common.IO;
using Games.Common.Randomness;
using SuperStarTrek.Objects;

namespace SuperStarTrek.Test.Systems {
    public class WarpEnginesTests {

        [Test]
        public void WarpExecuteCommandCore()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var we = new WarpEngines(mockEnterprise.Object, mockIO.Object);
            var quadrant = new Mock<IQuadrant>();
            var result = we.ExecuteCommandCore(quadrant.Object);
            Assert.That(result, Is.EqualTo(CommandResult.Ok));
        }
        /* [Test]
                                public void ExecuteCommandCoreEnergyGreaterThanDistance()
                                {
                                    IOSpy _ioSpy;
                                    _ioSpy = new();
                                    var mockIO = new Mock<IReadWrite>();
                                    var mockRandom = new Mock<IRandom>();
                                    var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
                                    var we = new WarpEngines(mockEnterprise.Object, _ioSpy);
                                    var quadrant = new Mock<IQuadrant>();
                                    mockIO.Setup(io => io.ReadNumber(It.IsAny<string>()))
                                          .Returns(5.0f);
                                    mockEnterprise.SetupGet(e => e.Energy).Returns(39);
                                    var result = we.ExecuteCommandCore(quadrant.Object);
                                    TestContext.WriteLine(_ioSpy.GetOutput());
                                    Assert.That(_ioSpy.GetOutput(), Is.EqualTo(string.Join(Environment.NewLine,
                                                [
                                                    "Engineering reports, 'Insufficient energy available",
                                                    "                      for maneuvering at warp 5 !'",
                                                    ""
                                                ])));

                                }
        */



    }
}