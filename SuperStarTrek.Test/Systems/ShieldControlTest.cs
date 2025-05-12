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
	public class ShieldControlTest
	{
        [Test]
        public void AbsorbHit_ReducesShieldEnergy()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            shieldControl.ShieldEnergy = 500;
            shieldControl.AbsorbHit(200);
            Assert.AreEqual(300, shieldControl.ShieldEnergy);
        }

        [Test]
        public void DropShields_SetsEnergyToZero()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            shieldControl.ShieldEnergy = 500;
            shieldControl.DropShields();
            Assert.AreEqual(0, shieldControl.ShieldEnergy);
        }


        [Test]
        public void CanExecuteCommand_WhenOperational_ReturnsTrue()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            Assert.IsTrue(shieldControl.CanExecuteCommand());
        }

        [Test]
        public void CanExecuteCommand_WhenDamaged_ReturnsFalse()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            shieldControl.TakeDamage(1);
            Assert.IsFalse(shieldControl.CanExecuteCommand());
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

            //mockEnterprise.Setup(e => e.TotalEnergy).Returns(1000);
            mockIO.Setup(io => io.ReadNumber("Number of units to shields")).Returns(500);

            shieldControl.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Energy available = 1000"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_WithNegativeRequest_Rejects()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1000, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);

            //_mockEnterprise.Setup(e => e.TotalEnergy).Returns(1000);
            mockIO.Setup(io => io.ReadNumber("Number of units to shields")).Returns(-100);

            var result = shieldControl.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(0, shieldControl.ShieldEnergy);
            mockIO.Verify(io => io.WriteLine("<SHIELDS UNCHANGED>"), Times.Once);
            Assert.AreEqual(CommandResult.Ok, result);
        }

        [Test]
        public void ExecuteCommandCore_WithSameAsCurrent_Rejects()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1000, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);

            shieldControl.ShieldEnergy = 300;
            //mockEnterprise.Setup(e => e.TotalEnergy).Returns(1000);
            mockIO.Setup(io => io.ReadNumber("Number of units to shields")).Returns(300);

            var result = shieldControl.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(300, shieldControl.ShieldEnergy);
            mockIO.Verify(io => io.WriteLine("<SHIELDS UNCHANGED>"), Times.Once);
            Assert.AreEqual(CommandResult.Ok, result);
        }

        [Test]
        public void ExecuteCommandCore_WithExcessiveRequest_RejectsWithMessage()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1000, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);

            //_mockEnterprise.Setup(e => e.TotalEnergy).Returns(1000);
            mockIO.Setup(io => io.ReadNumber("Number of units to shields")).Returns(1500);

            var result = shieldControl.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(0, shieldControl.ShieldEnergy);
            mockIO.Verify(io => io.WriteLine("Shield Control reports, 'This is not the Federation Treasury.'"), Times.Once);
            mockIO.Verify(io => io.WriteLine("<SHIELDS UNCHANGED>"), Times.Once);
            Assert.AreEqual(CommandResult.Ok, result);
        }

    }
}

