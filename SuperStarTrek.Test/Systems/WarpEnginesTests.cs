using Moq;
using SuperStarTrek.Space;
using SuperStarTrek.Systems;
using SuperStarTrek.Commands;
using Games.Common.IO;
using Games.Common.Randomness;
using SuperStarTrek.Objects;
using SuperStarTrek.Resources;

namespace SuperStarTrek.Test.Systems {
    public class WarpEnginesTests {

        Mock<IReadWrite> _mockIO;
        Mock<Enterprise> _mockEnterprise;
        WarpEngines _we;
        
        [SetUp]
        public void SetUp()
        {
            _mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            _mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), _mockIO.Object, mockRandom.Object);
            _we = new WarpEngines(_mockEnterprise.Object, _mockIO.Object);
        }

        #region Constructor

        [Test]
        public void WarpEngines_Constructor_Should_Correctly_Set_Name()
        {
            Assert.That(_we.Name, Is.EqualTo("Warp Engines"));
        }

        #endregion Constructor

        #region ExecuteCommandCore

        [Test]
        public void WarpEngines_ExecuteCommandCore_Returns_Ok()
        {
            var quadrant = new Mock<IQuadrant>();
            var result = _we.ExecuteCommandCore(quadrant.Object);
            Assert.That(result, Is.EqualTo(CommandResult.Ok));
        }

        [Test]
        public void WarpEngines_ExecuteCommandCore_GameOver_Returns_GameOver()
        {
            _mockIO.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(4);
            
            var quadrant = new Mock<IQuadrant>();
            CommandResult expectedResult = new CommandResult(true);
            quadrant.Setup(quadrant => quadrant.KlingonsMoveAndFire()).Returns(expectedResult);

            _mockEnterprise.Setup(enterprise => enterprise.Energy).Returns(100);

            var result = _we.ExecuteCommandCore(quadrant.Object);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void WarpEngines_ExecuteCommandCore_ShouldRepairSystems()
        {
            _mockIO.SetupSequence(io => io.ReadNumber(It.IsAny<string>()))
                .Returns(5)
                .Returns(1.0f);

            var quadrantMock = new Mock<IQuadrant>();
            quadrantMock.Setup(q => q.KlingonsMoveAndFire()).Returns(new CommandResult(false));

            _mockEnterprise.Setup(e => e.Energy).Returns(100);
            _mockEnterprise.Setup(e => e.IsDocked).Returns(true);
            _mockEnterprise.Setup(e => e.Move(It.IsAny<Course>(), It.IsAny<float>(), It.IsAny<int>()))
                .Returns(1f);

            var shieldControlMock = new Mock<ShieldControl>(_mockEnterprise.Object, _mockIO.Object);
            _mockEnterprise.Object.Add(shieldControlMock.Object);

            var photonTubesMock = new Mock<PhotonTubes>(5, _mockEnterprise.Object, _mockIO.Object);
            _mockEnterprise.Setup(e => e.PhotonTubes).Returns(photonTubesMock.Object);

            _mockEnterprise.Setup(e => e.Quadrant).Returns(quadrantMock.Object);

            _we.ExecuteCommandCore(quadrantMock.Object);

            _mockEnterprise.Verify(e => e.RepairSystems(It.IsAny<float>()), Times.Once);
        }

        [Test]
        public void WarpEngines_ExecuteCommandCore_Should_Call_VaryConditionOfRandomSystem()
        {
            _mockIO.SetupSequence(io => io.ReadNumber(It.IsAny<string>()))
                .Returns(5)
                .Returns(1.0f);

            var quadrantMock = new Mock<IQuadrant>();
            quadrantMock.Setup(q => q.KlingonsMoveAndFire()).Returns(new CommandResult(false));

            _mockEnterprise.Setup(e => e.Energy).Returns(100);
            _mockEnterprise.Setup(e => e.IsDocked).Returns(true);
            _mockEnterprise.Setup(e => e.Move(It.IsAny<Course>(), It.IsAny<float>(), It.IsAny<int>()))
                .Returns(1f);

            var shieldControlMock = new Mock<ShieldControl>(_mockEnterprise.Object, _mockIO.Object);
            _mockEnterprise.Object.Add(shieldControlMock.Object);

            var photonTubesMock = new Mock<PhotonTubes>(5, _mockEnterprise.Object, _mockIO.Object);
            _mockEnterprise.Setup(e => e.PhotonTubes).Returns(photonTubesMock.Object);

            _mockEnterprise.Setup(e => e.Quadrant).Returns(quadrantMock.Object);

            _we.ExecuteCommandCore(quadrantMock.Object);

            _mockEnterprise.Verify(e => e.VaryConditionOfRandomSystem(), Times.Once);
        }

        [Test]
        public void WarpEngines_ExecuteCommandCore_EnterpriseIsDocked_DropsShields()
        {
            _mockIO.SetupSequence(io => io.ReadNumber(It.IsAny<string>()))
                .Returns(5)
                .Returns(1.0f);

            var quadrantMock = new Mock<IQuadrant>();
            quadrantMock.Setup(q => q.KlingonsMoveAndFire()).Returns(new CommandResult(false));

            _mockEnterprise.Setup(e => e.Energy).Returns(100);
            _mockEnterprise.Setup(e => e.IsDocked).Returns(true);
            _mockEnterprise.Setup(e => e.Move(It.IsAny<Course>(), It.IsAny<float>(), It.IsAny<int>()))
                .Returns(1f);

            var shieldControlMock = new Mock<ShieldControl>(_mockEnterprise.Object, _mockIO.Object);
            _mockEnterprise.Object.Add(shieldControlMock.Object);

            var photonTubesMock = new Mock<PhotonTubes>(5, _mockEnterprise.Object, _mockIO.Object);
            _mockEnterprise.Setup(e => e.PhotonTubes).Returns(photonTubesMock.Object);

            _mockEnterprise.Setup(e => e.Quadrant).Returns(quadrantMock.Object);

            _we.ExecuteCommandCore(quadrantMock.Object);

            shieldControlMock.Verify(s => s.DropShields(), Times.Once);
        }

        [Test]
        public void WarpEngines_ExecuteCommandCore_EnterpriseIsDocked_Refuels()
        {
            _mockIO.SetupSequence(io => io.ReadNumber(It.IsAny<string>()))
                .Returns(5)
                .Returns(1.0f);

            var quadrantMock = new Mock<IQuadrant>();
            quadrantMock.Setup(q => q.KlingonsMoveAndFire()).Returns(new CommandResult(false));

            _mockEnterprise.Setup(e => e.Energy).Returns(100);
            _mockEnterprise.Setup(e => e.IsDocked).Returns(true);
            _mockEnterprise.Setup(e => e.Move(It.IsAny<Course>(), It.IsAny<float>(), It.IsAny<int>()))
                .Returns(1f);

            var shieldControlMock = new Mock<ShieldControl>(_mockEnterprise.Object, _mockIO.Object);
            _mockEnterprise.Object.Add(shieldControlMock.Object);

            var photonTubesMock = new Mock<PhotonTubes>(5, _mockEnterprise.Object, _mockIO.Object);
            _mockEnterprise.Setup(e => e.PhotonTubes).Returns(photonTubesMock.Object);

            _mockEnterprise.Setup(e => e.Quadrant).Returns(quadrantMock.Object);

            _we.ExecuteCommandCore(quadrantMock.Object);

            _mockEnterprise.Verify(e => e.Refuel(), Times.Once);
        }

        [Test]
        public void WarpEngines_ExecuteCommandCore_EnterpriseIsDocked_Calls_ReplenishTorpedoes()
        {
            _mockIO.SetupSequence(io => io.ReadNumber(It.IsAny<string>()))
                .Returns(5)
                .Returns(1.0f);

            var quadrantMock = new Mock<IQuadrant>();
            quadrantMock.Setup(q => q.KlingonsMoveAndFire()).Returns(new CommandResult(false));

            _mockEnterprise.Setup(e => e.Energy).Returns(100);
            _mockEnterprise.Setup(e => e.IsDocked).Returns(true);
            _mockEnterprise.Setup(e => e.Move(It.IsAny<Course>(), It.IsAny<float>(), It.IsAny<int>()))
                .Returns(1f);

            var shieldControlMock = new Mock<ShieldControl>(_mockEnterprise.Object, _mockIO.Object);
            _mockEnterprise.Object.Add(shieldControlMock.Object);

            var photonTubesMock = new Mock<PhotonTubes>(5, _mockEnterprise.Object, _mockIO.Object);
            _mockEnterprise.Setup(e => e.PhotonTubes).Returns(photonTubesMock.Object);

            _mockEnterprise.Setup(e => e.Quadrant).Returns(quadrantMock.Object);

            _we.ExecuteCommandCore(quadrantMock.Object);

            photonTubesMock.Verify(p => p.ReplenishTorpedoes(), Times.Once);
        }

        [Test]
        public void WarpEngines_ExecuteCommandCore_Should_Call_Quadrant_Display()
        {
            _mockIO.SetupSequence(io => io.ReadNumber(It.IsAny<string>()))
                .Returns(5)
                .Returns(1.0f);

            var quadrantMock = new Mock<IQuadrant>();
            quadrantMock.Setup(q => q.KlingonsMoveAndFire()).Returns(new CommandResult(false));

            _mockEnterprise.Setup(e => e.Energy).Returns(100);
            _mockEnterprise.Setup(e => e.IsDocked).Returns(true);
            _mockEnterprise.Setup(e => e.Move(It.IsAny<Course>(), It.IsAny<float>(), It.IsAny<int>()))
                .Returns(1f);

            var shieldControlMock = new Mock<ShieldControl>(_mockEnterprise.Object, _mockIO.Object);
            _mockEnterprise.Object.Add(shieldControlMock.Object);

            var photonTubesMock = new Mock<PhotonTubes>(5, _mockEnterprise.Object, _mockIO.Object);
            _mockEnterprise.Setup(e => e.PhotonTubes).Returns(photonTubesMock.Object);

            _mockEnterprise.Setup(e => e.Quadrant).Returns(quadrantMock.Object);

            _we.ExecuteCommandCore(quadrantMock.Object);

            quadrantMock.Verify(q => q.Display(Strings.NowEntering), Times.Once);
        }

        #endregion ExecuteCommandCore

        #region TryGetWarpFactor

        [Test]
        public void WarpEngines_TryGetWarpFactor_TryReadNumberInRange_False_ReturnsFalse()
        {
            _mockIO.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(9);
            Assert.That(_we.TryGetWarpFactor(out float resultWarpFactor), Is.False);
        }

        [Test]
        public void WarpEngines_TryGetWarpFactor_IsDamaged_True_ReturnsFalse()
        {
            _we.TakeDamage(1);
            _mockIO.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(9);
            Assert.That(_we.TryGetWarpFactor(out float resultWarpFactor), Is.False);
        }

        [Test]
        public void WarpEngines_TryGetWarpFactor_IsDamaged_Prints_Correct_Lines()
        {
            _we.TakeDamage(1);
            _mockIO.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(9);
            _we.TryGetWarpFactor(out float resultWarpFactor);

            _mockIO.Verify(
                io => io.WriteLine("Warp engines are damaged.  Maximum speed = warp 0.2"),
                Times.Once
            );
        }

        [Test]
        public void WarpEngines_TryGetWarpFactor_NotDamaged_Prints_Correct_Lines()
        {
            _we.TakeDamage(1);
            _mockIO.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(-0.2f);
            _we.TryGetWarpFactor(out float resultWarpFactor);

            _mockIO.Verify(
                io => io.WriteLine(
                    $"  Chief Engineer Scott reports, 'The engines won't take warp {resultWarpFactor} !'"
                ),
                Times.Once
            );
        }

        #endregion TryGetWarpFactor

        #region TryGetDistanceToMove

        [Test]
        public void WarpEngines_TryGetDistanceToMove_DistanceToTravel_GreaterThan_EnterpriseEnergy_ReturnsFalse()
        {
            _mockEnterprise.Setup(enterprise => enterprise.Energy).Returns(-10);
            Assert.That(_we.TryGetDistanceToMove(1f, out int resultDistanceToTravel), Is.False);
        }

        [Test]
        public void WarpEngines_TryGetDistanceToMove_EnergyDeployedToShields_Prints_Correct_Lines()
        {
            // don't use the _we from SetUp because we need to use IOSpy, not Mock<IReadWrite>
            IOSpy ioSpy = new();
            WarpEngines testWarpEngines = new(_mockEnterprise.Object, ioSpy);

            _mockEnterprise.Setup(enterprise => enterprise.Energy).Returns(-10);
            _mockEnterprise.Object.TotalEnergy = 100;

            Mock<ShieldControl> shieldControlMock = new(_mockEnterprise.Object, ioSpy);
            shieldControlMock.Setup(shieldControl => shieldControl.ShieldEnergy).Returns(0.5f);
            _mockEnterprise.Object.Add(shieldControlMock.Object);

            testWarpEngines.TryGetDistanceToMove(1f, out int resultDistanceToTravel);

            Assert.That(ioSpy.GetOutput(), Is.EqualTo(string.Join(Environment.NewLine,
            [
                "Engineering reports, 'Insufficient energy available",
                "                      for maneuvering at warp 1 !'",
                "Deflector control room acknowledges 0.5 ",
                "units of energy",
                "                         presently deployed to shields.",
                ""
            ])));
        }

        #endregion TryGetDistanceToMove
    }
}