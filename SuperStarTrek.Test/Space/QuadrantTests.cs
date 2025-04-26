using Moq;
using SuperStarTrek.Objects;
using SuperStarTrek.Commands;
using SuperStarTrek.Resources;
using SuperStarTrek.Systems;
using Games.Common.IO;
using Games.Common.Randomness;

namespace SuperStarTrek.Space
{
    [TestFixture]
    public class QuadrantTests
    {

        // Test: Create a QuadrantInfo with expected coordinates and check if factory method works correctly.
        [Test]
        public void Create_ExpectedCoordinates_CreateFactory()
        {
            var randomMock = new Mock<IRandom>();
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.0f)
                .Returns(0.0f)
                .Returns(0.0f);

            var expected = new Coordinates(1, 1);
            var quadrantInfo = QuadrantInfo.Create(expected, "Test-Quadrant", randomMock.Object);
            Assert.That(quadrantInfo.Coordinates, Is.EqualTo(expected));
        }


        // Test: Create an Enterprise with a given initial energy and ensure the energy value is correctly set.
        [TestCase(5000)]
        [TestCase(-1)]
        [TestCase(0)]
        public void Create_Enterprise_WithGivenEnergy(int energy)
        {
            var enterprise = new Enterprise(energy, new Coordinates(1, 1), new Mock<IReadWrite>().Object, new Mock<IRandom>().Object);
            Assert.That(enterprise.TotalEnergy, Is.EqualTo(energy));
        }


        // Test: Creating a Galaxy with mocked random should result in 1 Klingon being placed (based on random behavior).
        [Test]
        public void Create_Galaxy_WithSuppliedValues()
        {
            var randomMock = new Mock<IRandom>();
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.0f)
                .Returns(0.0f)
                .Returns(0.0f);

            var galaxy = new Galaxy(randomMock.Object);

            Assert.That(galaxy.KlingonCount, Is.EqualTo(1));
        }


        // Test: Ensure the Quadrant constructor correctly sets the quadrant's coordinates.
        [Test]
        public void Constructor_ShouldPositionObjectsCorrectly()
        {
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var quadrantInfoCoord = new Coordinates(1, 1);
            var enterpriseCoord = new Coordinates(2, 2);

            var galaxy = new Galaxy(randomMock.Object);
            var quadrantInfo = QuadrantInfo.Create(quadrantInfoCoord, "Test-Quadrant", randomMock.Object);
            var enterprise = new Enterprise(5000, enterpriseCoord, ioMock.Object, randomMock.Object);
            var quadrant = new Quadrant(quadrantInfo, enterprise, randomMock.Object, galaxy, ioMock.Object);

            Assert.AreEqual(new Coordinates(1, 1), quadrant.Coordinates);
        }

        // Test: TorpedoCollisionAt should return false when firing at an empty space (no objects at the coordinates).
        [Test]
        public void TorpedoCollisonAt_WhenDefault_ReturnsFalse()
        {
            var randomMock = new Mock<IRandom>();
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            var ioMock = new Mock<IReadWrite>();


            var galaxy = new Galaxy(randomMock.Object);
            var quadrantInfo = QuadrantInfo.Create(new Coordinates(2, 2), "Test-Quadrant", randomMock.Object);


            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
               .Returns(0.5f)
               .Returns(0);

            var enterprise = new Enterprise(5000, new Coordinates(1, 1), ioMock.Object, mockRandom.Object);

            var quadrant = new Quadrant(quadrantInfo, enterprise, mockRandom.Object, galaxy, ioMock.Object);

            var emptyCoords = new Coordinates(3, 3);

            bool result = quadrant.TorpedoCollisionAt(emptyCoords, out string message, out bool gameOver);

            Assert.That(result, Is.False);
            Assert.That(gameOver, Is.False);
            Assert.That(gameOver, Is.False);

        }


        // Test: TorpedoCollisionAt should return true when a torpedo hits a star, and it should output the correct message.
        [Test]
        public void TorpedoCollisonAt_WhenHittingStar_ReturnsTrue()
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);

            var quadrantInfo = QuadrantInfo.Create(new Coordinates(0, 0), "Test-Quadrant", mockRandom.Object);
            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            var galaxy = new Galaxy(mockRandomGalaxy.Object);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterprise = new Enterprise(0, new Coordinates(1, 1), ioMock.Object, randomMock.Object);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.5f)
                .Returns(0);
            var coordinates = new Coordinates(4, 0);

            var quadrant = new Quadrant(quadrantInfo, enterprise, randomMock.Object, galaxy, ioMock.Object);

            bool result = quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);

            Assert.True(result);
            Assert.That(message, Is.EqualTo($"Star at {coordinates} absorbed torpedo energy."));
            Assert.False(gameOver);
        }

        /**
         * Test that `Display` writes the correct strings to IO depending on the current state
         * of the enterprise.
         */
        [TestCase(0, 0f, 1, 0, 0, Reason = "Prints only the Quadrant when Quadrant has no Klingon")] // 0 Klingons 
        [TestCase(201, 0.81f, 1, 1, 0, Reason = "Prints the Combat Area text when a Klingon is in the sector and no Low shields message when shield is above 200")] // 1 Klingon with Shield at 201
        [TestCase(200, 0.81f, 1, 1, 1, Reason = "Prints the Low Shields warning when shields are below or equal to 200")] // 1 Klingon with Shield at 200
        [TestCase(0, 0.81f, 1, 1, 1, Reason = "Prints the Low Shields warning when shields are below or equal to 200")] // 1 Klingon with Shield at 200
        public void Display_WhenCalled_WritesExpectedString(float shieldEnergy, float klingonCountRandom, int quadrantFormatCalledCount, int combatAreaFormatCalledCount, int lowShieldsFormatCalledCount)
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(klingonCountRandom)
                .Returns(0f)
                .Returns(0f);


            var quadrantInfo = QuadrantInfo.Create(new Coordinates(0, 0), "Test-Quadrant", mockRandom.Object);
            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            var galaxy = new Galaxy(mockRandomGalaxy.Object);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterprise = new Mock<Enterprise>(1, new Coordinates(1, 1), ioMock.Object, randomMock.Object);
            var shieldControl = new ShieldControl(enterprise.Object, ioMock.Object);
            shieldControl.ShieldEnergy = shieldEnergy;

            enterprise.Object.Add(shieldControl);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.5f)
                .Returns(0);
            var quadrant = new Quadrant(quadrantInfo, enterprise.Object, randomMock.Object, galaxy, ioMock.Object);
            string textFormat = "{0}";
            enterprise.Setup(e => e.Execute(Command.SRS))
                .Returns(CommandResult.Ok);

            quadrant.Display(textFormat);

            enterprise.Verify(e => e.Execute(Command.SRS), Times.Exactly(1));
            ioMock.Verify(io => io.Write(textFormat, quadrant), Times.Exactly(quadrantFormatCalledCount));
            ioMock.Verify(io => io.Write(Strings.CombatArea), Times.Exactly(combatAreaFormatCalledCount));
            ioMock.Verify(io => io.Write(Strings.LowShields), Times.Exactly(lowShieldsFormatCalledCount));
        }
    }
}
