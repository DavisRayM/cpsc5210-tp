using Moq;
using SuperStarTrek.Space;
using SuperStarTrek.Systems;
using SuperStarTrek.Commands;
using Games.Common.IO;
using Games.Common.Randomness;
using SuperStarTrek.Objects;

namespace SuperStarTrek.Test.Systems
{

    public class ShortRangeSensorsTests
    {
        IOSpy _ioSpy;
        [Test]
        public void ExecuteCommandCore_ReturnsOk()
        {
            _ioSpy = new();
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockGalaxy = new Mock<Galaxy>(mockRandom.Object);
            var mockGame = new Mock<Game>(mockIO.Object, mockRandom.Object);
            var quadrant = new Mock<IQuadrant>();

            var srs = new ShortRangeSensors(mockEnterprise.Object, mockGalaxy.Object, mockGame.Object,_ioSpy);
            var result = srs.ExecuteCommandCore(quadrant.Object);

            Assert.That(result, Is.EqualTo(CommandResult.Ok));
        }
        [Test]
                public void ExecuteCommandCore_Returndashstring()
                {
                    _ioSpy = new();
                    var mockIO = new Mock<IReadWrite>();
                    var mockRandom = new Mock<IRandom>();
                    var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
                    var mockGalaxy = new Mock<Galaxy>(mockRandom.Object);
                    var mockGame = new Mock<Game>(mockIO.Object, mockRandom.Object);
                    var quadrant = new Mock<IQuadrant>();

                    var srs = new ShortRangeSensors(mockEnterprise.Object, mockGalaxy.Object, mockGame.Object,_ioSpy);
                    var result = srs.ExecuteCommandCore(quadrant.Object);

                    Assert.That(_ioSpy.GetOutput(), Is.EqualTo(string.Join(Environment.NewLine,
                                [
                                    "---------------------------------",
                                    "---------------------------------",
                                    ""
                                ])));
                }
        [Test]
                public void ExecuteCommandCore_Returnsstatus()
                {
                    _ioSpy = new();
                    var mockIO = new Mock<IReadWrite>();
                    var mockRandom = new Mock<IRandom>();
                    var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(0, 0), mockIO.Object, mockRandom.Object);
                    var mockGalaxy = new Mock<Galaxy>(mockRandom.Object);
                    mockGalaxy.SetupGet(g => g.KlingonCount).Returns(10);
                    var mockGame = new Mock<Game>(mockIO.Object, mockRandom.Object);
                    var quadrant = new Mock<IQuadrant>();
                    var pt = new Mock<PhotonTubes>(15, mockEnterprise.Object, mockIO.Object);
                    mockEnterprise.SetupGet(e => e.Condition).Returns("Good");
                    quadrant.Setup(q => q.GetDisplayLines()).Returns(new List<string>
                    {
                        "SEC1", "SEC2", "SEC3", "SEC4", "SEC5", "SEC6", "SEC7", "SEC8"
                    });
                    mockGame.SetupGet(game => game.Stardate).Returns(1);
                    var c1 = new Coordinates(1, 1);
                    mockEnterprise.SetupGet(e => e.QuadrantCoordinates).Returns(c1);
                    mockEnterprise.SetupGet(e => e.PhotonTubes).Returns(pt.Object);
                    var mockShield = new Mock<ShieldControl>(mockEnterprise.Object, mockIO.Object);
                    mockEnterprise.Object.Add(mockShield.Object);
                    mockShield.SetupGet(s => s.ShieldEnergy).Returns(50);
                    pt.SetupGet(p => p.TorpedoCount).Returns(5);
                    var srs = new ShortRangeSensors(mockEnterprise.Object, mockGalaxy.Object, mockGame.Object,_ioSpy);

                    var result = srs.ExecuteCommandCore(quadrant.Object);


                    Assert.That(_ioSpy.GetOutput(), Is.EqualTo(string.Join(Environment.NewLine,
                                [
                                    "---------------------------------",
                                    " SEC1         Stardate           1",
                                    " SEC2         Condition          Good",
                                    " SEC3         Quadrant           2 , 2",
                                    " SEC4         Sector             1 , 1",
                                    " SEC5         Photon torpedoes   5",
                                    " SEC6         Total energy       1",
                                    " SEC7         Shields            50",
                                    " SEC8         Klingons remaining 10",
                                    "---------------------------------",
                                    ""
                                ])));
                }
        [Test]
                        public void ExecuteCommandCore_ReturnsstatusIsDocked()
                        {
                            _ioSpy = new();
                            var mockIO = new Mock<IReadWrite>();
                            var mockRandom = new Mock<IRandom>();
                            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(0, 0), mockIO.Object, mockRandom.Object);
                            var mockGalaxy = new Mock<Galaxy>(mockRandom.Object);
                            var mockGame = new Mock<Game>(mockIO.Object, mockRandom.Object);
                            var quadrant = new Mock<IQuadrant>();
                            var pt = new Mock<PhotonTubes>(15, mockEnterprise.Object, mockIO.Object);
                            mockEnterprise.SetupGet(e => e.Condition).Returns("Good");
                            quadrant.Setup(q => q.GetDisplayLines()).Returns(new List<string>
                            {
                                "SEC1", "SEC2", "SEC3", "SEC4", "SEC5", "SEC6"
                            });
                            mockGame.SetupGet(game => game.Stardate).Returns(1);
                            var c1 = new Coordinates(1, 1);
                            mockEnterprise.SetupGet(e => e.QuadrantCoordinates).Returns(c1);
                            mockEnterprise.SetupGet(e => e.PhotonTubes).Returns(pt.Object);
                            pt.SetupGet(p => p.TorpedoCount).Returns(5);
                            var srs = new ShortRangeSensors(mockEnterprise.Object, mockGalaxy.Object, mockGame.Object,_ioSpy);

                            mockEnterprise.SetupGet(e => e.IsDocked).Returns(true);
                            var result = srs.ExecuteCommandCore(quadrant.Object);

                            Assert.That(_ioSpy.GetOutput(), Is.EqualTo(string.Join(Environment.NewLine,
                                        [
                                            "Shields dropped for docking purposes",
                                            "---------------------------------",
                                            " SEC1         Stardate           1",
                                            " SEC2         Condition          Good",
                                            " SEC3         Quadrant           2 , 2",
                                            " SEC4         Sector             1 , 1",
                                            " SEC5         Photon torpedoes   5",
                                            " SEC6         Total energy       1",
                                            "---------------------------------",
                                            ""
                                        ])));
                        }
        [Test]
                        public void ExecuteCommandCore_ReturnsstatusConditionLessThanZero()
                        {
                            _ioSpy = new();
                            var mockIO = new Mock<IReadWrite>();
                            var mockRandom = new Mock<IRandom>();
                            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(0, 0), mockIO.Object, mockRandom.Object);
                            var mockGalaxy = new Mock<Galaxy>(mockRandom.Object);
                            var mockGame = new Mock<Game>(mockIO.Object, mockRandom.Object);
                            var quadrant = new Mock<IQuadrant>();
                            var pt = new Mock<PhotonTubes>(15, mockEnterprise.Object, mockIO.Object);
                            mockEnterprise.SetupGet(e => e.Condition).Returns("Good");
                            quadrant.Setup(q => q.GetDisplayLines()).Returns(new List<string>
                            {
                                "SEC1", "SEC2", "SEC3", "SEC4", "SEC5", "SEC6"
                            });
                            mockGame.SetupGet(game => game.Stardate).Returns(1);
                            var c1 = new Coordinates(1, 1);
                            mockEnterprise.SetupGet(e => e.QuadrantCoordinates).Returns(c1);
                            mockEnterprise.SetupGet(e => e.PhotonTubes).Returns(pt.Object);
                            pt.SetupGet(p => p.TorpedoCount).Returns(5);
                            var srs = new ShortRangeSensors(mockEnterprise.Object, mockGalaxy.Object, mockGame.Object,_ioSpy);
                            srs.TakeDamage(1);

                            var result = srs.ExecuteCommandCore(quadrant.Object);

                            Assert.That(_ioSpy.GetOutput(), Is.EqualTo(string.Join(Environment.NewLine,
                                        [
                                            "*** Short Range Sensors are out ***",
                                            "",
                                            "---------------------------------",
                                            " SEC1         Stardate           1",
                                            " SEC2         Condition          Good",
                                            " SEC3         Quadrant           2 , 2",
                                            " SEC4         Sector             1 , 1",
                                            " SEC5         Photon torpedoes   5",
                                            " SEC6         Total energy       1",
                                            "---------------------------------",
                                            ""
                                        ])));
                        }

    }
}