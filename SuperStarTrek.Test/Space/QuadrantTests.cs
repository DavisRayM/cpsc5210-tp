using Moq;
using SuperStarTrek.Objects;
using Games.Common.IO;
using Games.Common.Randomness;

namespace SuperStarTrek.Space
{
    [TestFixture]
    public class QuadrantTests
    {
        private const string Name = "Test-Quadrant";

        [Test]
        public void Create_ExpectedCoordinates_CreateFactory()
        {
            var randomMock = new Mock<IRandom>();
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.0f)
                .Returns(0.0f)
                .Returns(0.0f);

            var expected = new Coordinates(1, 1);
            var quadrantInfo = QuadrantInfo.Create(expected, Name, randomMock.Object);
            Assert.That(quadrantInfo.Coordinates, Is.EqualTo(expected));
        }


        [TestCase(5000)]
        [TestCase(-1)]
        [TestCase(0)]
        public void Create_Enterprise_WithGivenEnergy(int energy)
        {
            var enterprise = new Enterprise(energy, new Coordinates(1, 1), new Mock<IReadWrite>().Object, new Mock<IRandom>().Object);
            Assert.That(enterprise.TotalEnergy, Is.EqualTo(energy));
        }


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

        [Test]
        public void TorpedoCollisonAt_WhenNoCollision_ReturnsDefaultResult()
        {
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterpriseCoords = new Coordinates(2, 2);
            var enterprise = new Enterprise(5000, enterpriseCoords, ioMock.Object, randomMock.Object);
            var galaxy = new Galaxy(randomMock.Object);
            var quadrantInfo = QuadrantInfo.Create(new Coordinates(4, 4), "Test-Quadrant", randomMock.Object);
            var quadrant = new Quadrant(quadrantInfo, enterprise, randomMock.Object, galaxy, ioMock.Object);

            var emptyCoords = new Coordinates(5, 5);

            bool result = quadrant.TorpedoCollisionAt(emptyCoords, out string message, out bool gameOver);

            Assert.That(result, Is.False);
            Assert.That(gameOver, Is.False);
            Assert.That(gameOver, Is.False);

        }

        [Test]
        public void TorpedoCollisonAt_WhenHittingStar_ReturnsTrue()
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);

            var quadrantInfo = QuadrantInfo.Create(new Coordinates(0, 0), Name, mockRandom.Object);
            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            var galaxy = new Galaxy(mockRandomGalaxy.Object);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterprise = new Enterprise(0, new Coordinates(1, 0), ioMock.Object, randomMock.Object);
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
    }
}
