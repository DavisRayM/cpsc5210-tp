using Moq;
using Games.Common.IO;
using Games.Common.Randomness;
using SuperStarTrek.Systems;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Commands;

namespace SuperStarTrek.Test.Systems
{
    public class DamageControlTests
    {
        [Test]
        public void ExecuteCommandCore_IsDamaged_PrintsOnScreen()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            DamageControl damageControl = new(mockEnterprise.Object, mockIO.Object);
            damageControl.TakeDamage(1);
            mockEnterprise.Setup(e => e.DamagedSystemCount)
                .Returns(0);
            mockEnterprise.Setup(e => e.IsDocked)
                .Returns(false);

            var result = damageControl.ExecuteCommandCore(mockQuadrant.Object);
            Assert.That(result, Is.EqualTo(CommandResult.Ok));
            mockIO.Verify(io => io.WriteLine("Damage Control report not available"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCode_NotDamaged_PrintsDamageReport()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            DamageControl damageControl = new(mockEnterprise.Object, mockIO.Object);
            mockEnterprise.Setup(e => e.DamagedSystemCount)
                .Returns(0);
            mockEnterprise.Setup(e => e.IsDocked)
                .Returns(false);

            var result = damageControl.ExecuteCommandCore(mockQuadrant.Object);
            Assert.That(result, Is.EqualTo(CommandResult.Ok));
            mockIO.Verify(io => io.WriteLine("Device             State of Repair"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_DamagedSystems_AttemptsRepair()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var mockStarbase = new Mock<Starbase>(new Coordinates(1, 1), mockRandom.Object, mockIO.Object);
            float expectedRepairTime = 1.25f;

            DamageControl damageControl = new(mockEnterprise.Object, mockIO.Object);
            mockEnterprise.Setup(e => e.DamagedSystemCount)
                .Returns(1);
            mockEnterprise.Setup(e => e.IsDocked)
                .Returns(true);
            mockQuadrant.Setup(q => q.Starbase).Returns(mockStarbase.Object);
            mockStarbase.Setup(s => s.TryRepair(It.IsAny<Enterprise>(), out expectedRepairTime))
                .Returns(true);


            var result = damageControl.ExecuteCommandCore(mockQuadrant.Object);
            float repairTime = 0;
            mockStarbase.Verify(st => st.TryRepair(mockEnterprise.Object, out repairTime), Times.Once());
            Assert.That(result.TimeElapsed, Is.EqualTo(expectedRepairTime));
            mockIO.Verify(io => io.WriteLine("Device             State of Repair"), Times.Exactly(2));
        }

        [Test]
        public void WriteDamageReport_Prints_SystemInformation()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var mockSubsystem = new Mock<Subsystem>("test", Command.DAM, mockIO.Object);


            DamageControl damageControl = new(mockEnterprise.Object, mockIO.Object);

            mockEnterprise.Object.Add(mockSubsystem.Object);
            mockEnterprise.Setup(e => e.DamagedSystemCount)
                .Returns(1);
            mockEnterprise.Setup(e => e.IsDocked)
                .Returns(true);

            damageControl.WriteDamageReport();
            mockIO.Verify(io => io.WriteLine("Device             State of Repair"), Times.Once());
            mockIO.Verify(io => io.Write(mockSubsystem.Object.Name + "                     "), Times.Once());
            mockIO.Verify(io => io.WriteLine((mockSubsystem.Object.Condition * 100) * 0.01F), Times.Once());
        }
    }
}
