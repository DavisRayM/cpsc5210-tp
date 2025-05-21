using Games.Common.IO;
using Games.Common.Randomness;
using Moq;
using SuperStarTrek.Commands;
using SuperStarTrek.Objects;
using SuperStarTrek.Resources;
using SuperStarTrek.Space;
using SuperStarTrek.Systems;

namespace SuperStarTrek.Test.Objects
{
    public class StarbaseTests
    {
        [Test]
        public void TryRepair_YesToRepair_RepairsAllSystems_And_ReturnsTrue()
        {
            Mock<IRandom> randomMock = new();
            randomMock
                .Setup(random => random.NextFloat())
                .Returns(1f);

            Mock<IReadWrite> ioMock = new();
            ioMock
                .Setup(io => io.ReadString(It.IsAny<string>()))
                .Returns("Y");

            Starbase testStarbase = new(
                new Coordinates(0, 0),
                randomMock.Object,
                ioMock.Object
            );

            Mock<Subsystem> systemMock1 = new("subsystem1", Command.XXX, new Mock<IReadWrite>().Object);
            Mock<Subsystem> systemMock2 = new("subsystem2", Command.XXX, new Mock<IReadWrite>().Object);
            Mock<Subsystem> systemMock3 = new("subsystem3", Command.XXX, new Mock<IReadWrite>().Object);
            Mock<Subsystem> systemMock4 = new("subsystem4", Command.XXX, new Mock<IReadWrite>().Object);
            Mock<Subsystem> systemMock5 = new("subsystem5", Command.XXX, new Mock<IReadWrite>().Object);

            Mock<Enterprise> mockEnterprise = new(
                1,
                new Coordinates(1, 1),
                new Mock<IReadWrite>().Object,
                new Mock<IRandom>().Object
            );
            mockEnterprise
                .Setup(enterprise => enterprise.DamagedSystemCount)
                .Returns(5);
            mockEnterprise.Object.Add(systemMock1.Object);
            mockEnterprise.Object.Add(systemMock2.Object);
            mockEnterprise.Object.Add(systemMock3.Object);
            mockEnterprise.Object.Add(systemMock4.Object);
            mockEnterprise.Object.Add(systemMock5.Object);

            Assert.Multiple(() =>
            {
                Assert.That(testStarbase.TryRepair(mockEnterprise.Object, out float repairTimeResult));

                // repairTime should be truncated to 0.9f if >= 1
                Assert.That(repairTimeResult, Is.EqualTo(0.9f));
            });

            ioMock.Verify(io => io.Write(Strings.RepairEstimate, 0.9f), Times.Once());

            // all systems repaired
            systemMock1.Verify(system => system.Repair(), Times.Once);
            systemMock2.Verify(system => system.Repair(), Times.Once);
            systemMock3.Verify(system => system.Repair(), Times.Once);
            systemMock4.Verify(system => system.Repair(), Times.Once);
        }

        [Test]
        public void TryRepair_NoToRepair_DoesNotRepairSystems_And_ReturnsFalse()
        {
            Mock<IRandom> randomMock = new();
            randomMock
                .Setup(random => random.NextFloat())
                .Returns(0.4f);

            Mock<IReadWrite> ioMock = new();
            ioMock
                .Setup(io => io.ReadString(It.IsAny<string>()))
                .Returns("N");

            Starbase testStarbase = new(
                new Coordinates(0, 0),
                randomMock.Object,
                ioMock.Object
            );

            Mock<Subsystem> systemMock1 = new("subsystem1", Command.XXX, new Mock<IReadWrite>().Object);
            Mock<Subsystem> systemMock2 = new("subsystem2", Command.XXX, new Mock<IReadWrite>().Object);
            Mock<Subsystem> systemMock3 = new("subsystem3", Command.XXX, new Mock<IReadWrite>().Object);
            Mock<Subsystem> systemMock4 = new("subsystem4", Command.XXX, new Mock<IReadWrite>().Object);

            Mock<Enterprise> mockEnterprise = new(
                1,
                new Coordinates(1, 1),
                new Mock<IReadWrite>().Object,
                new Mock<IRandom>().Object
            );
            mockEnterprise
                .Setup(enterprise => enterprise.DamagedSystemCount)
                .Returns(4);
            mockEnterprise.Object.Add(systemMock1.Object);
            mockEnterprise.Object.Add(systemMock2.Object);
            mockEnterprise.Object.Add(systemMock3.Object);
            mockEnterprise.Object.Add(systemMock4.Object);

            Assert.Multiple(() =>
            {
                Assert.That(
                    testStarbase.TryRepair(mockEnterprise.Object, out float repairTimeResult),
                    Is.False
                );

                Assert.That(
                    repairTimeResult,
                    Is.EqualTo(0)
                );
            });

            ioMock.Verify(
                io => io.Write(Strings.RepairEstimate,
                0.6f
            ), Times.Once());

            // systems not repaired
            systemMock1.Verify(system => system.Repair(), Times.Never);
            systemMock2.Verify(system => system.Repair(), Times.Never);
            systemMock3.Verify(system => system.Repair(), Times.Never);
            systemMock4.Verify(system => system.Repair(), Times.Never);
        }

        [Test]
        public void TryRepair_NoSystems_BehavesCorrectly()
        {
            Mock<IRandom> randomMock = new();
            randomMock
                .Setup(random => random.NextFloat())
                .Returns(0.4f);

            Mock<IReadWrite> ioMock = new();
            ioMock
                .Setup(io => io.ReadString(It.IsAny<string>()))
                .Returns("Y");

            Starbase testStarbase = new(
                new Coordinates(0, 0),
                randomMock.Object,
                ioMock.Object
            );

            Mock<Enterprise> mockEnterprise = new(
                1,
                new Coordinates(1, 1),
                new Mock<IReadWrite>().Object,
                new Mock<IRandom>().Object
            );
            mockEnterprise
                .Setup(enterprise => enterprise.DamagedSystemCount)
                .Returns(4);

            Assert.Multiple(() =>
            {
                Assert.That(testStarbase.TryRepair(mockEnterprise.Object, out float repairTimeResult));

                // repairTime should be truncated to 0.9f if >= 1
                Assert.That(repairTimeResult, Is.EqualTo(0.6f));
            });

            ioMock.Verify(io => io.Write(Strings.RepairEstimate, 0.6f), Times.Once());
        }
    }
}
