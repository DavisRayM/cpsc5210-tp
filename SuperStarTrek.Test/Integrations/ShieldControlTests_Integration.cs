using Games.Common.IO;
using Games.Common.Randomness;
using Moq;
using NUnit.Framework;
using SuperStarTrek.Commands;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Systems;

namespace SuperStarTrek.Test.Systems
{
    public class ShieldControlTests_Integration
    {
        private IOSpy _io;
        private Mock<IRandom> _random;
        private Enterprise _enterprise;
        private Quadrant _quadrant;
        private Galaxy _galaxy;
        private ShieldControl _shieldControl;

        [SetUp]
        public void Setup()
        {
            // Arrange real IO spy and random
            _io = new IOSpy();
            _random = new Mock<IRandom>();
            _random.Setup(r => r.NextFloat()).Returns(0f);

            // Sector coordinates for the enterprise
            var sectorCoords = new Coordinates(1, 1);
            _enterprise = new Enterprise(1000, sectorCoords, _io, _random.Object);

            _shieldControl = new ShieldControl(_enterprise, _io);
        }

        [Test]
        public void AbsorbHit_ReducesShieldEnergy()
        {
            //var mockIO = new Mock<IReadWrite>();
            //var mockRandom = new Mock<IRandom>();
            //var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            //var _shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            _shieldControl.ShieldEnergy = 500;
            _shieldControl.AbsorbHit(200);
            Assert.AreEqual(300, _shieldControl.ShieldEnergy);
        }

        [Test]
        public void DropShields_SetsEnergyToZero()
        {
            //var mockIO = new Mock<IReadWrite>();
            //var mockRandom = new Mock<IRandom>();
            //var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            //var _shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            _shieldControl.ShieldEnergy = 500;
            _shieldControl.DropShields();
            Assert.AreEqual(0, _shieldControl.ShieldEnergy);
        }


        [Test]
        public void CanExecuteCommand_WhenOperational_ReturnsTrue()
        {
            //var _io = new Mock<IReadWrite>();
            //var _random = new Mock<IRandom>();
            //var _enterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), _io, _random.Object);

            //var _shieldControl = new ShieldControl(_enterprise, _io);
            Assert.IsTrue(_shieldControl.CanExecuteCommand());
        }

        [Test]
        public void CanExecuteCommand_WhenDamaged_ReturnsFalse()
        {
            //var _io = new Mock<IReadWrite>();
            //var _random = new Mock<IRandom>();
            //var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), _io, _random.Object);

            //var _shieldControl = new ShieldControl(mockEnterprise.Object, _io);
            _shieldControl.TakeDamage(1);
            Assert.IsFalse(_shieldControl.CanExecuteCommand());
        }

        [Test]
        public void CanExecuteCommand_WhenDamaged_DisplaysInoperableMessage()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            shieldControl.TakeDamage(1);

            shieldControl.CanExecuteCommand();

            mockIO.Verify(io => io.WriteLine("Shield Control inoperable"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_DisplaysAvailableEnergy()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1000, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);


            mockIO.Setup(io => io.ReadNumber("Number of units to shields")).Returns(500);

            shieldControl.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Energy available = 1000"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_WithNegativeRequest_LeavesShieldEnergyUnchanged()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1000, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            mockIO.Setup(io => io.ReadNumber("Number of units to shields")).Returns(-100);

            shieldControl.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(0, shieldControl.ShieldEnergy);
        }

        [Test]
        public void ExecuteCommandCore_WithNegativeRequest_DisplaysUnchangedMessage()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1000, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            mockIO.Setup(io => io.ReadNumber("Number of units to shields")).Returns(-100);

            shieldControl.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("<SHIELDS UNCHANGED>"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_WithNegativeRequest_ReturnsOkResult()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1000, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            mockIO.Setup(io => io.ReadNumber("Number of units to shields")).Returns(-100);

            var result = shieldControl.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(CommandResult.Ok, result);
        }

        [Test]
        public void ExecuteCommandCore_WithSameAsCurrent_LeavesShieldEnergyUnchanged()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1000, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            shieldControl.ShieldEnergy = 300;
            mockIO.Setup(io => io.ReadNumber("Number of units to shields")).Returns(300);

            shieldControl.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(300, shieldControl.ShieldEnergy);
        }

        [Test]
        public void ExecuteCommandCore_WithExcessiveRequest_DisplaysTreasuryMessage()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1000, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            mockIO.Setup(io => io.ReadNumber("Number of units to shields")).Returns(1500);

            shieldControl.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Shield Control reports, 'This is not the Federation Treasury.'"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_WithExcessiveRequest_DisplaysUnchangedMessage()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1000, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            mockIO.Setup(io => io.ReadNumber("Number of units to shields")).Returns(1500);

            shieldControl.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("<SHIELDS UNCHANGED>"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_WithExcessiveRequest_ReturnsOkResult()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1000, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            mockIO.Setup(io => io.ReadNumber("Number of units to shields")).Returns(1500);

            var result = shieldControl.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(CommandResult.Ok, result);
        }

    }
}

