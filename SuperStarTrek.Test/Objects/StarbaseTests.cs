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
        Mock<IReadWrite> _ioMock;
        Mock<IRandom> _randomMock;

        // Can't do this in SetUp because it depends on parameters
        private Starbase CreateStarbase(float randomNextFloat, string IOYesNo)
        {
            _randomMock = new();
            _randomMock
                .Setup(random => random.NextFloat())
                .Returns(randomNextFloat);

            _ioMock = new();
            _ioMock
                .Setup(io => io.ReadString(It.IsAny<string>()))
                .Returns(IOYesNo);

            return new Starbase(
                new Coordinates(0, 0),
                _randomMock.Object,
                _ioMock.Object
            );
        }

        // Used in TryRepair tests
        private static (
            Mock<Enterprise> mockEnterprise,
            List<Mock<Subsystem>> subsystemMocks
        ) CreateEnterprise(int numSystems)
        {
            Mock<Enterprise> mockEnterprise = new(
                1,
                new Coordinates(1, 1),
                new Mock<IReadWrite>().Object,
                new Mock<IRandom>().Object
            );
            mockEnterprise
                .Setup(enterprise => enterprise.DamagedSystemCount)
                .Returns(numSystems);

            List<Mock<Subsystem>> subsystemMocks = [];
            for (int i = 0; i < numSystems; i++)
            {
                Mock<Subsystem> subsystemMock = new(
                    "subystem",
                    Command.XXX,
                    new Mock<IReadWrite>().Object
                );

                subsystemMocks.Add(subsystemMock);
                mockEnterprise.Object.Add(subsystemMock.Object);
            }

            return (mockEnterprise, subsystemMocks);
        }

        #region ToString

        [Test]
        public void Starbase_ToString_ReturnsCorrectString()
        {
            Assert.That(CreateStarbase(0f, "Y").ToString(), Is.EqualTo(">!<"));
        }

        #endregion ToString

        #region TryRepair

        [Test]
        [TestCase(1f, "Y", 5)]
        [TestCase(0.4f, "N", 4)]
        [TestCase(0.4f, "Y", 0)]
        public void TryRepair_Returns_TrueIfY_Or_FalseIfN(
            float randomNextFloat,
            string IOYesNo,
            int numSystems
        )
        {
            Assert.That(CreateStarbase(randomNextFloat, IOYesNo).TryRepair(
                CreateEnterprise(numSystems).mockEnterprise.Object,
                out float repairTimeResult
            ), Is.EqualTo(IOYesNo == "Y"));
        }

        [Test]
        [TestCase(1f, "Y", 5, 0.9f)]
        [TestCase(0.4f, "N", 4, 0f)]
        [TestCase(0.4f, "Y", 0, 0.2f)]
        public void TryRepair_Returns_Correct_RepairTime(
            float randomNextFloat,
            string IOYesNo,
            int numSystems,
            float expectedRepairTimeReturned
        )
        {
            CreateStarbase(randomNextFloat, IOYesNo).TryRepair(
                CreateEnterprise(numSystems).mockEnterprise.Object,
                out float repairTimeResult
            );

            Assert.That(repairTimeResult, Is.EqualTo(expectedRepairTimeReturned));
        }

        [Test]
        [TestCase(1f, "Y", 5, 0.9f)]
        [TestCase(0.4f, "N", 4, 0.6f)]
        [TestCase(0.4f, "Y", 0, 0.2f)]
        public void TryRepair_Prints_Correct_RepairEstimate(
            float randomNextFloat,
            string IOYesNo,
            int numSystems,
            float expectedRepairTimePrinted
        )
        {
            CreateStarbase(randomNextFloat, IOYesNo).TryRepair(
                CreateEnterprise(numSystems).mockEnterprise.Object,
                out float repairTimeResult
            );

            _ioMock.Verify(
                io => io.Write(
                    Strings.RepairEstimate,
                    expectedRepairTimePrinted
                ), Times.Once()
            );
        }

        [Test]
        [TestCase(1f, "Y", 5)]
        [TestCase(0.4f, "N", 4)]
        [TestCase(0.4f, "Y", 0)]
        public void TryRepair_RepairsSubsystems(
            float randomNextFloat,
            string IOYesNo,
            int numSystems
        )
        {
            (
                Mock<Enterprise> mockEnterprise,
                List<Mock<Subsystem>> subsystemMocks
            ) = CreateEnterprise(numSystems);

            CreateStarbase(randomNextFloat, IOYesNo).TryRepair(
                mockEnterprise.Object,
                out float repairTimeResult
            );

            // all systems repaired only if IO got "Y"
            foreach (Mock<Subsystem> subsystemMock in subsystemMocks)
            {
                subsystemMock.Verify(
                    system => system.Repair(),
                    IOYesNo == "Y" ? Times.Once : Times.Never
                );
            }
        }

        #endregion TryRepair

        #region ProtectEnterprise

        [Test]
        public void ProtectEnterprise_WritesTo_IO()
        {
            CreateStarbase(0f, "Y").ProtectEnterprise();

            _ioMock.Verify(io => io.WriteLine(Strings.Protected), Times.Once());
        }

        #endregion ProtectEnterprise
    }
}
