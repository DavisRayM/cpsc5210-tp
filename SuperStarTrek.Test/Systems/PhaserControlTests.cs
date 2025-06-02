using Moq;
using Games.Common.IO;
using Games.Common.Randomness;
using SuperStarTrek.Systems;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Commands;
using SuperStarTrek.Resources;
using SuperStarTrek.Systems.ComputerFunctions;

namespace SuperStarTrek.Test.Systems
{
    public class PhaserControlTests
    {

        [Test]
        public void System_Description_IsExpected()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            Assert.That(control.Name, Is.EqualTo("Phaser Control"));
        }

        [Test]
        [TestCase(0f, ExpectedResult = true, Reason = "CanExecuteCommand returns true if not damaged")]
        [TestCase(1f, ExpectedResult = false, Reason = "CanExecuteCommand returns false if damaged")]
        public bool CanExecuteCommand_DependsOnDamage(float damage)
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            control.TakeDamage(damage);
            return control.CanExecuteCommand();
        }

        [Test]
        public void CanExecuteCommand_NotifiesUser()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            control.TakeDamage(1f);
            control.CanExecuteCommand();

            mockIO.Verify(io => io.WriteLine("Phasers inoperative"));
        }

        [Test]
        public void ExecuteCommandCore_NoKlingon_PrintsNoEnemyShips()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            Mock<IQuadrant> quadrantMock = new();

            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            quadrantMock.Setup(q => q.HasKlingons).Returns(false);

            control.ExecuteCommandCore(quadrantMock.Object);
            mockIO.Verify(io => io.WriteLine(Strings.NoEnemyShips), Times.Once());
        }

        [Test]
        public void ExecuteCommandCore_NoKlingon_ReturnsOk()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            Mock<IQuadrant> quadrantMock = new();

            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            quadrantMock.Setup(q => q.HasKlingons).Returns(false);

            Assert.That(control.ExecuteCommandCore(quadrantMock.Object), Is.EqualTo(CommandResult.Ok));
        }

        [Test]
        public void ExecuteCommandCore_DamagedComputer_PrintsOnScreen()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            Mock<IQuadrant> mockQuadrant = new();
            Mock<ComputerFunction> computerFunctionMock = new(
                "description1",
                mockIO.Object
            );
            Mock<LibraryComputer> mockComputer = new(mockIO.Object, computerFunctionMock.Object);
            mockComputer.Setup(e => e.Condition).Returns(-1f);
            mockQuadrant.Setup(q => q.HasKlingons).Returns(true);
            mockEnterprise.Setup(e => e.Computer).Returns(mockComputer.Object);
            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);

            control.ExecuteCommandCore(mockQuadrant.Object);
            mockIO.Verify(io => io.WriteLine("Computer failure hampers accuracy"), Times.Once());
        }

        [Test]
        public void ExecuteCommandCore_NegativePhaserStrength_ReturnsOk()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            Mock<IQuadrant> mockQuadrant = new();
            Mock<ComputerFunction> computerFunctionMock = new(
                "description1",
                mockIO.Object
            );
            Mock<LibraryComputer> mockComputer = new(mockIO.Object, computerFunctionMock.Object);

            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            mockQuadrant.Setup(q => q.HasKlingons).Returns(true);
            mockEnterprise.Setup(e => e.Computer).Returns(mockComputer.Object);
            mockEnterprise.Setup(e => e.Energy).Returns(10f);

            mockIO.Setup(io => io.ReadNumber("Number of units to fire")).Returns(-1);
            Assert.That(control.ExecuteCommandCore(mockQuadrant.Object), Is.EqualTo(CommandResult.Ok));
        }

        [Test]
        public void ExecuteCommandCore_AvailableEnergy_IsPrinted()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            Mock<IQuadrant> mockQuadrant = new();
            Mock<ComputerFunction> computerFunctionMock = new(
                "description1",
                mockIO.Object
            );
            Mock<LibraryComputer> mockComputer = new(mockIO.Object, computerFunctionMock.Object);

            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            mockQuadrant.Setup(q => q.HasKlingons).Returns(true);
            mockEnterprise.Setup(e => e.Computer).Returns(mockComputer.Object);
            mockEnterprise.Setup(e => e.Energy).Returns(10f);

            mockIO.Setup(io => io.ReadNumber("Number of units to fire")).Returns(0);
            control.ExecuteCommandCore(mockQuadrant.Object);
            mockIO.Verify(io => io.WriteLine($"Energy available = {mockEnterprise.Object.Energy} units"), Times.Exactly(1));
        }

        [Test]
        public void ExecuteCommandCore_ZeroEnergy_StillFires()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            Mock<IQuadrant> mockQuadrant = new();
            Mock<ComputerFunction> computerFunctionMock = new(
                "description1",
                mockIO.Object
            );
            Mock<LibraryComputer> mockComputer = new(mockIO.Object, computerFunctionMock.Object);

            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            mockQuadrant.Setup(q => q.HasKlingons).Returns(true);
            mockEnterprise.Setup(e => e.Computer).Returns(mockComputer.Object);
            mockEnterprise.Setup(e => e.Energy).Returns(10f);

            mockIO.Setup(io => io.ReadNumber("Number of units to fire")).Returns(0f);
            control.ExecuteCommandCore(mockQuadrant.Object);
            mockEnterprise.Verify(e => e.UseEnergy(0f), Times.Once());
        }

        [Test]
        public void ExecuteCommandCore_DecreasesEnergy_IfGreaterThanZeroAndLessThanTotal()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            Mock<IQuadrant> mockQuadrant = new();
            Mock<ComputerFunction> computerFunctionMock = new(
                "description1",
                mockIO.Object
            );
            Mock<LibraryComputer> mockComputer = new(mockIO.Object, computerFunctionMock.Object);

            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            mockQuadrant.Setup(q => q.HasKlingons).Returns(true);
            mockEnterprise.Setup(e => e.Computer).Returns(mockComputer.Object);
            mockEnterprise.Setup(e => e.Energy).Returns(10f);

            mockIO.Setup(io => io.ReadNumber("Number of units to fire")).Returns(5f);
            control.ExecuteCommandCore(mockQuadrant.Object);
            mockEnterprise.Verify(e => e.UseEnergy(5f), Times.Once());
        }

        [Test]
        public void ExecuteCommandCore_DamageMultipliedByRandom_IfComputerDamaged()
        {
            // Setup essential objects and state
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(0, 0), mockIO.Object, mockRandom.Object);
            Mock<IQuadrant> mockQuadrant = new();
            Mock<ComputerFunction> computerFunctionMock = new(
                "description1",
                mockIO.Object
            );
            Mock<LibraryComputer> mockComputer = new(mockIO.Object, computerFunctionMock.Object);
            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            float phaserStrength = 4f;
            int klingonCount = 2;
            Coordinates hitSector = new Coordinates(1, 1);
            var distance = mockEnterprise.Object.SectorCoordinates.GetDistanceTo(hitSector);
            var randNum = 2.0f;
            var hitStrength = (int)(((phaserStrength * randNum) / klingonCount) / distance * (2 + randNum));

            Mock<Klingon> hitKlingon = new(hitSector, mockRandom.Object);
            Mock<Klingon> missedKlingon = new(new Coordinates(1, 2), mockRandom.Object);
            string expectedMessage = $"{hitStrength} unit hit on Klingon at sector {hitSector}";

            hitKlingon.Setup(k => k.TakeHit(It.IsAny<int>())).Returns(true);
            missedKlingon.Setup(k => k.TakeHit(It.IsAny<int>())).Returns(false);
            Klingon[] klingons = [
                hitKlingon.Object,
                missedKlingon.Object,
            ];

            // Setup Mocks; Ensure computer is not Damaged and has specified Klingon Count
            mockQuadrant.Setup(q => q.Klingons).Returns(klingons);
            mockQuadrant.Setup(q => q.HasKlingons).Returns(true);
            mockQuadrant.Setup(q => q.KlingonCount).Returns(klingonCount);
            mockComputer.Setup(e => e.Condition).Returns(-1f);
            mockEnterprise.Setup(e => e.Computer).Returns(mockComputer.Object);
            mockEnterprise.Setup(e => e.Energy).Returns(10f);
            mockRandom.Setup(r => r.NextFloat()).Returns(randNum);
            mockIO.Setup(io => io.ReadNumber("Number of units to fire")).Returns(phaserStrength);

            control.ExecuteCommandCore(mockQuadrant.Object);
            mockIO.Verify(io => io.WriteLine(expectedMessage), Times.Once());
        }

        [Test]
        public void ExecuteCommandCore_SplitsPhaserStrengthEvenly_IfComputerNotDamaged()
        {
            // Setup essential objects and state
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(0, 0), mockIO.Object, mockRandom.Object);
            Mock<IQuadrant> mockQuadrant = new();
            Mock<ComputerFunction> computerFunctionMock = new(
                "description1",
                mockIO.Object
            );
            Mock<LibraryComputer> mockComputer = new(mockIO.Object, computerFunctionMock.Object);
            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            float phaserStrength = 4f;
            int klingonCount = 2;
            Coordinates hitSector = new Coordinates(1, 1);
            var distance = mockEnterprise.Object.SectorCoordinates.GetDistanceTo(hitSector);
            var randNum = 0.1f;
            var hitStrength = (int)((phaserStrength / klingonCount) / distance * (2 + randNum));

            Mock<Klingon> hitKlingon = new(hitSector, mockRandom.Object);
            Mock<Klingon> missedKlingon = new(new Coordinates(1, 2), mockRandom.Object);
            string expectedMessage = $"{hitStrength} unit hit on Klingon at sector {hitSector}";

            hitKlingon.Setup(k => k.TakeHit(It.IsAny<int>())).Returns(true);
            missedKlingon.Setup(k => k.TakeHit(It.IsAny<int>())).Returns(false);
            Klingon[] klingons = [
                hitKlingon.Object,
                missedKlingon.Object,
            ];

            // Setup Mocks; Ensure computer is not Damaged and has specified Klingon Count
            mockQuadrant.Setup(q => q.Klingons).Returns(klingons);
            mockQuadrant.Setup(q => q.HasKlingons).Returns(true);
            mockQuadrant.Setup(q => q.KlingonCount).Returns(klingonCount);
            mockEnterprise.Setup(e => e.Computer).Returns(mockComputer.Object);
            mockEnterprise.Setup(e => e.Energy).Returns(10f);
            mockRandom.Setup(r => r.NextFloat()).Returns(randNum);
            mockIO.Setup(io => io.ReadNumber("Number of units to fire")).Returns(phaserStrength);

            control.ExecuteCommandCore(mockQuadrant.Object);
            mockIO.Verify(io => io.WriteLine(expectedMessage), Times.Once());
        }

        [Test]
        public void ExecuteCommandCore_OnHit_ExpectedText()
        {
            // Setup essential objects and state
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(0, 0), mockIO.Object, mockRandom.Object);
            Mock<IQuadrant> mockQuadrant = new();
            Mock<ComputerFunction> computerFunctionMock = new(
                "description1",
                mockIO.Object
            );
            Mock<LibraryComputer> mockComputer = new(mockIO.Object, computerFunctionMock.Object);
            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            float phaserStrength = 4f;
            int klingonCount = 1;
            float expectedEnergy = 10f;

            Mock<Klingon> hitKlingon = new(new Coordinates(0, 0), mockRandom.Object);
            hitKlingon.Setup(k => k.Energy).Returns(expectedEnergy);
            hitKlingon.Setup(k => k.TakeHit(It.IsAny<int>())).Returns(true);
            Klingon[] klingons = [
                hitKlingon.Object,
            ];

            // Setup Mocks; Ensure computer is not Damaged and has specified Klingon Count
            mockQuadrant.Setup(q => q.Klingons).Returns(klingons);
            mockQuadrant.Setup(q => q.HasKlingons).Returns(true);
            mockQuadrant.Setup(q => q.KlingonCount).Returns(klingonCount);
            mockEnterprise.Setup(e => e.Computer).Returns(mockComputer.Object);
            mockEnterprise.Setup(e => e.Energy).Returns(10f);
            mockIO.Setup(io => io.ReadNumber("Number of units to fire")).Returns(phaserStrength);

            control.ExecuteCommandCore(mockQuadrant.Object);
            mockIO.Verify(io => io.Write("Phasers locked on target;  "), Times.Once());
        }

        [Test]
        public void ExecuteCommandCore_OnHit_ShowsRemainingEnergy()
        {
            // Setup essential objects and state
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(0, 0), mockIO.Object, mockRandom.Object);
            Mock<IQuadrant> mockQuadrant = new();
            Mock<ComputerFunction> computerFunctionMock = new(
                "description1",
                mockIO.Object
            );
            Mock<LibraryComputer> mockComputer = new(mockIO.Object, computerFunctionMock.Object);
            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            float phaserStrength = 4f;
            int klingonCount = 1;
            float expectedEnergy = 10f;

            Mock<Klingon> hitKlingon = new(new Coordinates(0, 0), mockRandom.Object);
            hitKlingon.Setup(k => k.Energy).Returns(expectedEnergy);
            hitKlingon.Setup(k => k.TakeHit(It.IsAny<int>())).Returns(true);
            Klingon[] klingons = [
                hitKlingon.Object,
            ];

            // Setup Mocks; Ensure computer is not Damaged and has specified Klingon Count
            mockQuadrant.Setup(q => q.Klingons).Returns(klingons);
            mockQuadrant.Setup(q => q.HasKlingons).Returns(true);
            mockQuadrant.Setup(q => q.KlingonCount).Returns(klingonCount);
            mockEnterprise.Setup(e => e.Computer).Returns(mockComputer.Object);
            mockEnterprise.Setup(e => e.Energy).Returns(10f);
            mockIO.Setup(io => io.ReadNumber("Number of units to fire")).Returns(phaserStrength);

            control.ExecuteCommandCore(mockQuadrant.Object);
            mockIO.Verify(io => io.WriteLine($"   (sensors show {expectedEnergy} units remaining)"), Times.Once());
        }


        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void ExecuteCommandCore_OnHitEnergyZeroOrLess_RemoveKlingon(float energy)
        {
            // Setup essential objects and state
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(0, 0), mockIO.Object, mockRandom.Object);
            Mock<IQuadrant> mockQuadrant = new();
            Mock<ComputerFunction> computerFunctionMock = new(
                "description1",
                mockIO.Object
            );
            Mock<LibraryComputer> mockComputer = new(mockIO.Object, computerFunctionMock.Object);
            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            float phaserStrength = 4f;
            int klingonCount = 1;

            Mock<Klingon> hitKlingon = new(new Coordinates(0, 0), mockRandom.Object);
            hitKlingon.Setup(k => k.Energy).Returns(energy);
            hitKlingon.Setup(k => k.TakeHit(It.IsAny<int>())).Returns(true);
            Klingon[] klingons = [
                hitKlingon.Object,
            ];

            // Setup Mocks; Ensure computer is not Damaged and has specified Klingon Count
            mockQuadrant.Setup(q => q.Klingons).Returns(klingons);
            mockQuadrant.Setup(q => q.HasKlingons).Returns(true);
            mockQuadrant.Setup(q => q.KlingonCount).Returns(klingonCount);
            mockEnterprise.Setup(e => e.Computer).Returns(mockComputer.Object);
            mockEnterprise.Setup(e => e.Energy).Returns(10f);
            mockIO.Setup(io => io.ReadNumber("Number of units to fire")).Returns(phaserStrength);

            control.ExecuteCommandCore(mockQuadrant.Object);
            mockQuadrant.Verify(q => q.Remove(hitKlingon.Object));
        }


        [Test]
        public void ExecuteCommandCore_OnMiss_PrintsOnScreen()
        {
            // Setup essential objects and state
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(0, 0), mockIO.Object, mockRandom.Object);
            Mock<IQuadrant> mockQuadrant = new();
            Mock<ComputerFunction> computerFunctionMock = new(
                "description1",
                mockIO.Object
            );
            Mock<LibraryComputer> mockComputer = new(mockIO.Object, computerFunctionMock.Object);
            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            float phaserStrength = 4f;
            int klingonCount = 1;


            Coordinates expectedSector = new Coordinates(1, 4);
            Mock<Klingon> missedKlingon = new(expectedSector, mockRandom.Object);
            missedKlingon.Setup(k => k.TakeHit(It.IsAny<int>())).Returns(false);
            Klingon[] klingons = [
                missedKlingon.Object,
            ];

            // Setup Mocks; Ensure computer is not Damaged and has specified Klingon Count
            mockQuadrant.Setup(q => q.Klingons).Returns(klingons);
            mockQuadrant.Setup(q => q.HasKlingons).Returns(true);
            mockQuadrant.Setup(q => q.KlingonCount).Returns(klingonCount);
            mockEnterprise.Setup(e => e.Computer).Returns(mockComputer.Object);
            mockEnterprise.Setup(e => e.Energy).Returns(10f);
            mockIO.Setup(io => io.ReadNumber("Number of units to fire")).Returns(phaserStrength);

            control.ExecuteCommandCore(mockQuadrant.Object);
            mockIO.Verify(io => io.WriteLine($"Sensors show no damage to enemy at {expectedSector}"), Times.Once());
        }

        [Test]
        [TestCase(false, Reason = "Klingons still fire on miss")]
        [TestCase(true, Reason = "Klingons still fire on hit")]
        public void ExecuteCommandCore_OnAttemptedFire_KlingonsFireOnEnterprise(bool isHit)
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(0, 0), mockIO.Object, mockRandom.Object);
            Mock<IQuadrant> mockQuadrant = new();
            Mock<ComputerFunction> computerFunctionMock = new(
                "description1",
                mockIO.Object
            );
            Mock<LibraryComputer> mockComputer = new(mockIO.Object, computerFunctionMock.Object);
            PhaserControl control = new(mockEnterprise.Object, mockIO.Object, mockRandom.Object);
            float phaserStrength = 4f;
            int klingonCount = 1;


            Mock<Klingon> missedKlingon = new(new Coordinates(1, 4), mockRandom.Object);
            missedKlingon.Setup(k => k.TakeHit(It.IsAny<int>())).Returns(false);
            Klingon[] klingons = [
                missedKlingon.Object,
            ];

            // Setup Mocks; Ensure computer is not Damaged and has specified Klingon Count
            mockQuadrant.Setup(q => q.Klingons).Returns(klingons);
            mockQuadrant.Setup(q => q.HasKlingons).Returns(true);
            mockQuadrant.Setup(q => q.KlingonCount).Returns(klingonCount);
            mockEnterprise.Setup(e => e.Computer).Returns(mockComputer.Object);
            mockEnterprise.Setup(e => e.Energy).Returns(10f);
            mockIO.Setup(io => io.ReadNumber("Number of units to fire")).Returns(phaserStrength);

            control.ExecuteCommandCore(mockQuadrant.Object);
            mockQuadrant.Verify(q => q.KlingonsFireOnEnterprise(), Times.Once());
        }
    }
}
