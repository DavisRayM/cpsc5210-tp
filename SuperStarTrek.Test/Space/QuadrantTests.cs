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

        [Test]
        public void Quadrant_ToString_FromQuadrantInfoName()
        {
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var quadrantInfoCoord = new Coordinates(1, 1);
            var enterpriseCoord = new Coordinates(2, 2);

            var expectedName = "Milky Way";
            var galaxy = new Galaxy(randomMock.Object);
            var quadrantInfo = QuadrantInfo.Create(quadrantInfoCoord, expectedName, randomMock.Object);
            var enterprise = new Enterprise(5000, enterpriseCoord, ioMock.Object, randomMock.Object);
            var quadrant = new Quadrant(quadrantInfo, enterprise, randomMock.Object, galaxy, ioMock.Object);

            Assert.That(quadrant.ToString(), Is.EqualTo(expectedName));
        }

        // Test: Ensure the Quadrant returns the correct number of Klingons from QuadrantInfo
        [Test]
        public void KlingonCount_FromQuadrantInfo_Returned()
        {
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var quadrantInfoCoord = new Coordinates(1, 1);
            var enterpriseCoord = new Coordinates(2, 2);

            var galaxy = new Galaxy(randomMock.Object);
            var quadrantInfo = QuadrantInfo.Create(quadrantInfoCoord, "Test-Quadrant", randomMock.Object);
            var enterprise = new Enterprise(5000, enterpriseCoord, ioMock.Object, randomMock.Object);
            var quadrant = new Quadrant(quadrantInfo, enterprise, randomMock.Object, galaxy, ioMock.Object);
            quadrantInfo.AddKlingon();

            var actualCount = quadrant.KlingonCount;
            Assert.That(actualCount, Is.EqualTo(quadrantInfo.KlingonCount));
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
            var coordinates = new Coordinates((int)(0.5 * 7.98 + 1.01) - 1, (int)(0 * 7.98 + 1.01) - 1);

            var quadrant = new Quadrant(quadrantInfo, enterprise, randomMock.Object, galaxy, ioMock.Object);

            bool result = quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);

            Assert.True(result);
            Assert.That(message, Is.EqualTo($"Star at {coordinates} absorbed torpedo energy."));
            Assert.False(gameOver);
        }

        // Test: TorpedoCollisionAt should return true when a torpedo hits a Klingon.
        [Test]
        public void TorpedoCollisonAt_WhenHittingKlingon_ReturnsTrue()
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0.81f)
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
                .Returns(0f)
                .Returns(0.9f)
                .Returns(0.4f);
            var coordinates = new Coordinates((int)(0.5 * 7.98 + 1.01) - 1, (int)(0 * 7.98 + 1.01) - 1);

            var quadrant = new Quadrant(quadrantInfo, enterprise, randomMock.Object, galaxy, ioMock.Object);

            bool result = quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);

            Assert.True(result);
        }

        // Test: TorpedoCollisionAt should return message that Klingon was destroyed.
        [Test]
        public void TorpedoCollisonAt_WhenHittingKlingon_ReturnsCorrectMessage()
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0.81f)
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
                .Returns(0f)
                .Returns(0.9f)
                .Returns(0.4f);
            var coordinates = new Coordinates((int)(0.5 * 7.98 + 1.01) - 1, (int)(0 * 7.98 + 1.01) - 1);

            var quadrant = new Quadrant(quadrantInfo, enterprise, randomMock.Object, galaxy, ioMock.Object);

            bool result = quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);

            Assert.That(message, Is.EqualTo("*** Klingon destroyed ***"));
        }

        // Test: TorpedoCollisionAt should remove klingon from Quadrant if hit.
        [Test]
        public void TorpedoCollisonAt_WhenHittingKlingon_RemovesKlingonFromInfo()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);

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
                .Returns(0f)
                .Returns(0.9f)
                .Returns(0.4f);
            var coordinates = new Coordinates((int)(0.5 * 7.98 + 1.01) - 1, (int)(0 * 7.98 + 1.01) - 1);

            var quadrant = new Quadrant(mockInfo.Object, enterprise, randomMock.Object, galaxy, ioMock.Object);

            quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);
            mockInfo.Verify(i => i.RemoveKlingon(), Times.Once());
        }

        // Test: Constructor marks the quadrant info as known
        [Test]
        public void Constructor_QuadrantInfo_MarkedKnown()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);

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
                .Returns(0f)
                .Returns(0.9f)
                .Returns(0.4f);
            var coordinates = new Coordinates((int)(0.5 * 7.98 + 1.01) - 1, (int)(0 * 7.98 + 1.01) - 1);

            var quadrant = new Quadrant(mockInfo.Object, enterprise, randomMock.Object, galaxy, ioMock.Object);
            mockInfo.Verify(i => i.MarkAsKnown(), Times.Once());
        }

        // Test: TorpedoCollisionAt should remove klingon from sector if hit.
        [Test]
        public void TorpedoCollisonAt_WhenHittingKlingon_RemovesKlingonFromSector()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);

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
                .Returns(0f)
                .Returns(0.9f)
                .Returns(0.4f);
            var coordinates = new Coordinates((int)(0.5 * 7.98 + 1.01) - 1, (int)(0 * 7.98 + 1.01) - 1);

            var quadrant = new Quadrant(mockInfo.Object, enterprise, randomMock.Object, galaxy, ioMock.Object);

            quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);
            Assert.That(quadrant.Klingons.Count(), Is.EqualTo(0));
        }

        // Test: TorpedoCollisionAt should end game if last klingon is hit.
        [Test]
        public void TorpedoCollisonAt_WhenHitLastKlingon_GameOver()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(0);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterprise = new Enterprise(0, new Coordinates(1, 1), ioMock.Object, randomMock.Object);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.5f)
                .Returns(0f)
                .Returns(0.9f)
                .Returns(0.4f);
            var coordinates = new Coordinates((int)(0.5 * 7.98 + 1.01) - 1, (int)(0 * 7.98 + 1.01) - 1);

            var quadrant = new Quadrant(mockInfo.Object, enterprise, randomMock.Object, mockGalaxy.Object, ioMock.Object);

            quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);
            Assert.True(gameOver);
        }

        // Test: TorpedoCollisionAt when hit last starbase should return expected message.
        [Test]
        public void TorpedoCollisonAt_WhenHitLastStarbase_MessageRelievedOfCommand()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);
            mockInfo.Setup(i => i.HasStarbase).Returns(true);
            mockInfo.Setup(i => i.StarCount).Returns(1);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(0);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterprise = new Enterprise(0, new Coordinates(1, 1), ioMock.Object, randomMock.Object);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.5f)
                .Returns(0f)
                .Returns(0.35f)
                .Returns(0.56f)
                .Returns(0.2f)
                .Returns(0.1f);
            var coordinates = new Coordinates(4, 1);

            var quadrant = new Quadrant(mockInfo.Object, enterprise, randomMock.Object, mockGalaxy.Object, ioMock.Object);

            quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);
            Assert.That(message, Is.EqualTo("*** Starbase destroyed ***" + Strings.RelievedOfCommand));
        }

        // Test: TorpedoCollisionAt when hit last starbase game over.
        [Test]
        public void TorpedoCollisonAt_WhenHitLastStarbase_GameOver()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);
            mockInfo.Setup(i => i.HasStarbase).Returns(true);
            mockInfo.Setup(i => i.StarCount).Returns(1);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(0);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterprise = new Enterprise(0, new Coordinates(1, 1), ioMock.Object, randomMock.Object);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.5f)
                .Returns(0f)
                .Returns(0.35f)
                .Returns(0.56f)
                .Returns(0.2f)
                .Returns(0.1f);
            var coordinates = new Coordinates(4, 1);

            var quadrant = new Quadrant(mockInfo.Object, enterprise, randomMock.Object, mockGalaxy.Object, ioMock.Object);

            quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);
            Assert.True(gameOver);
        }

        // Test: TorpedoCollisionAt when hit last starbase should return expected message.
        [Test]
        public void TorpedoCollisonAt_WhenStarbase_ReturnsTrue()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);
            mockInfo.Setup(i => i.HasStarbase).Returns(true);
            mockInfo.Setup(i => i.StarCount).Returns(1);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(0);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterprise = new Enterprise(0, new Coordinates(1, 1), ioMock.Object, randomMock.Object);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.5f)
                .Returns(0f)
                .Returns(0.35f)
                .Returns(0.56f)
                .Returns(0.2f)
                .Returns(0.1f);
            var coordinates = new Coordinates(4, 1);

            var quadrant = new Quadrant(mockInfo.Object, enterprise, randomMock.Object, mockGalaxy.Object, ioMock.Object);

            var result = quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);
            Assert.True(result);
        }

        // Test: TorpedoCollisionAt when hit starbase should return expected message.
        [Test]
        public void TorpedoCollisonAt_WhenHitStarbaseAndNotLast_GameContinues()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);
            mockInfo.Setup(i => i.HasStarbase).Returns(true);
            mockInfo.Setup(i => i.StarCount).Returns(1);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(1);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterprise = new Enterprise(0, new Coordinates(1, 1), ioMock.Object, randomMock.Object);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.5f)
                .Returns(0f)
                .Returns(0.35f)
                .Returns(0.56f)
                .Returns(0.2f)
                .Returns(0.1f);
            var coordinates = new Coordinates(4, 1);

            var quadrant = new Quadrant(mockInfo.Object, enterprise, randomMock.Object, mockGalaxy.Object, ioMock.Object);

            quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);
            Assert.False(gameOver);
        }

        // Test: TorpedoCollisionAt when hit starbase should return expected message.
        [Test]
        public void TorpedoCollisonAt_WhenHitStarbase_MessageCourtMartialed()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);
            mockInfo.Setup(i => i.HasStarbase).Returns(true);
            mockInfo.Setup(i => i.StarCount).Returns(1);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(1);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterprise = new Enterprise(0, new Coordinates(1, 1), ioMock.Object, randomMock.Object);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.5f)
                .Returns(0f)
                .Returns(0.35f)
                .Returns(0.56f)
                .Returns(0.2f)
                .Returns(0.1f);
            var coordinates = new Coordinates(4, 1);

            var quadrant = new Quadrant(mockInfo.Object, enterprise, randomMock.Object, mockGalaxy.Object, ioMock.Object);

            quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);
            Assert.That(message, Is.EqualTo("*** Starbase destroyed ***" + Strings.CourtMartial));
        }

        // Test: TorpedoCollisionAt when hit starbase should remove starbase from info.
        [Test]
        public void TorpedoCollisonAt_WhenHitStarbase_RemoveStarbaseInfo()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);
            mockInfo.Setup(i => i.HasStarbase).Returns(true);
            mockInfo.Setup(i => i.StarCount).Returns(1);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(1);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterprise = new Enterprise(0, new Coordinates(1, 1), ioMock.Object, randomMock.Object);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.5f)
                .Returns(0f)
                .Returns(0.35f)
                .Returns(0.56f)
                .Returns(0.2f)
                .Returns(0.1f);
            var coordinates = new Coordinates(4, 1);

            var quadrant = new Quadrant(mockInfo.Object, enterprise, randomMock.Object, mockGalaxy.Object, ioMock.Object);

            quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);
            mockInfo.Verify(i => i.RemoveStarbase(), Times.Once());
        }

        // Test: TorpedoCollisionAt when hit starbase should remove starbase from sector.
        [Test]
        public void TorpedoCollisonAt_WhenHitStarbase_RemoveStarbaseFromSector()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);
            mockInfo.Setup(i => i.HasStarbase).Returns(true);
            mockInfo.Setup(i => i.StarCount).Returns(1);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(1);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterprise = new Enterprise(0, new Coordinates(1, 1), ioMock.Object, randomMock.Object);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.5f)
                .Returns(0f)
                .Returns(0.35f)
                .Returns(0.56f)
                .Returns(0.2f)
                .Returns(0.1f);
            var coordinates = new Coordinates(4, 1);

            var quadrant = new Quadrant(mockInfo.Object, enterprise, randomMock.Object, mockGalaxy.Object, ioMock.Object);

            quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);
            Assert.False(quadrant.HasObjectAt(coordinates));
        }

        // Test: TorpedoCollisionAt should continue game if klingon remain.
        [Test]
        public void TorpedoCollisonAt_WhenHitKlingon_GameContinues()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterprise = new Enterprise(0, new Coordinates(1, 1), ioMock.Object, randomMock.Object);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.5f)
                .Returns(0f)
                .Returns(0.9f)
                .Returns(0.4f);
            var coordinates = new Coordinates((int)(0.5 * 7.98 + 1.01) - 1, (int)(0 * 7.98 + 1.01) - 1);

            var quadrant = new Quadrant(mockInfo.Object, enterprise, randomMock.Object, mockGalaxy.Object, ioMock.Object);

            quadrant.TorpedoCollisionAt(coordinates, out string message, out bool gameOver);
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
        [Test]
        public void QuadrantHasStarbaseAtConstruction()
        {
            QuadrantInfo q = new QuadrantInfo(new Coordinates(5, 7), "test quadrant", 0, 0, true);
            var mockRandom = new Mock<IRandom>();
            var mockIo = new Mock<IReadWrite>();
            Enterprise e = new Enterprise(1000, new Coordinates(5, 5), mockIo.Object, mockRandom.Object);
            Galaxy g = new Galaxy(mockRandom.Object);

            Quadrant _quadrant = new Quadrant(q, e, mockRandom.Object, g, mockIo.Object);
            Assert.AreEqual(_quadrant.HasStarbase, true);
        }
        [Test]
        public void QuadrantHasNoStarBaseAtConstruction()
        {
            QuadrantInfo q = new QuadrantInfo(new Coordinates(5, 7), "test quadrant", 0, 0, false);
            var mockRandom = new Mock<IRandom>();
            var mockIo = new Mock<IReadWrite>();
            Enterprise e = new Enterprise(1000, new Coordinates(5, 5), mockIo.Object, mockRandom.Object);
            Galaxy g = new Galaxy(mockRandom.Object);

            Quadrant _quadrant = new Quadrant(q, e, mockRandom.Object, g, mockIo.Object);
            Assert.AreEqual(_quadrant.HasStarbase, false);
        }
        [Test]
        public void QuadrantEnterpriseNotNextToStarbaseAtConstruction()
        {
            QuadrantInfo q = new QuadrantInfo(new Coordinates(5, 7), "test quadrant", 0, 0, true);
            var mockRandom = new Mock<IRandom>();
            var mockIo = new Mock<IReadWrite>();
            Enterprise e = new Enterprise(1000, new Coordinates(5, 5), mockIo.Object, mockRandom.Object);
            Galaxy g = new Galaxy(mockRandom.Object);

            Quadrant _quadrant = new Quadrant(q, e, mockRandom.Object, g, mockIo.Object);
            Assert.AreEqual(_quadrant.EnterpriseIsNextToStarbase, false);
        }
        [Test]
        public void QuadrantHasCorrectGalaxy()
        {
            QuadrantInfo q = new QuadrantInfo(new Coordinates(5, 7), "test quadrant", 0, 0, true);
            var mockRandom = new Mock<IRandom>();
            var mockIo = new Mock<IReadWrite>();
            Enterprise e = new Enterprise(1000, new Coordinates(5, 5), mockIo.Object, mockRandom.Object);
            Galaxy g = new Galaxy(mockRandom.Object);

            Quadrant _quadrant = new Quadrant(q, e, mockRandom.Object, g, mockIo.Object);
            Assert.AreEqual(_quadrant.Galaxy, g);
        }
        [Test]
        public void QuadrantHasKlingonsAtConstruction()
        {
            QuadrantInfo q = new QuadrantInfo(new Coordinates(5, 7), "test quadrant", 1, 0, false);
            var mockRandom = new Mock<IRandom>();
            var mockIo = new Mock<IReadWrite>();
            Enterprise e = new Enterprise(1000, new Coordinates(5, 5), mockIo.Object, mockRandom.Object);
            Galaxy g = new Galaxy(mockRandom.Object);

            Quadrant _quadrant = new Quadrant(q, e, mockRandom.Object, g, mockIo.Object);
            Assert.AreEqual(_quadrant.HasKlingons, true);
        }

        [Test]
        public void QuadrantHasNoKlingonsAtConstruction()
        {
            QuadrantInfo q = new QuadrantInfo(new Coordinates(5, 7), "test quadrant", 0, 0, false);
            var mockRandom = new Mock<IRandom>();
            var mockIo = new Mock<IReadWrite>();
            Enterprise e = new Enterprise(1000, new Coordinates(5, 5), mockIo.Object, mockRandom.Object);
            Galaxy g = new Galaxy(mockRandom.Object);

            Quadrant _quadrant = new Quadrant(q, e, mockRandom.Object, g, mockIo.Object);
            Assert.AreEqual(_quadrant.HasKlingons, false);
        }

        [Test]
        public void SetEnterpriseSector_Changes_Coordinates()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(0);
            mockInfo.Setup(i => i.HasStarbase).Returns(false);
            mockInfo.Setup(i => i.StarCount).Returns(0);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(1);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterpriseCoords = new Coordinates(1, 1);
            Mock<Enterprise> mockEnterprise = new(1, enterpriseCoords, ioMock.Object, randomMock.Object);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.5f)
                .Returns(0f);
            Coordinates coords = new(7, 7);

            var quadrant = new Quadrant(mockInfo.Object, mockEnterprise.Object, randomMock.Object, mockGalaxy.Object, ioMock.Object);
            quadrant.SetEnterpriseSector(new Coordinates(4, 5));

            Assert.False(quadrant.HasObjectAt(enterpriseCoords));
        }

        [Test]
        public void GetDisplayLines_Quadrant_ExpectedString()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);
            mockInfo.Setup(i => i.HasStarbase).Returns(true);
            mockInfo.Setup(i => i.StarCount).Returns(1);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(1);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            Mock<Enterprise> mockEnterprise = new(1, new Coordinates(1, 1), ioMock.Object, randomMock.Object);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.5f)
                .Returns(0f)
                .Returns(0.9f)
                .Returns(1f)
                .Returns(0.2f)
                .Returns(0.1f);

            var quadrant = new Quadrant(mockInfo.Object, mockEnterprise.Object, randomMock.Object, mockGalaxy.Object, ioMock.Object);
            var result = string.Join("\n", quadrant.GetDisplayLines());
            var expected = string.Join("\n", [
            " *                             ",
            "                               ",
            "                               ",
            "                               ",
            "+K+                            ",
            "                               ",
            "                               ",
            "    >!<                        ",
            ]);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void KlingonsFireOnEnterprise_NoKlingons_ReturnsOk()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(0);
            mockInfo.Setup(i => i.HasStarbase).Returns(true);
            mockInfo.Setup(i => i.StarCount).Returns(0);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(1);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            Mock<Enterprise> mockEnterprise = new(1, new Coordinates(1, 1), ioMock.Object, randomMock.Object);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.5f)
                .Returns(0f);

            var quadrant = new Quadrant(mockInfo.Object, mockEnterprise.Object, randomMock.Object, mockGalaxy.Object, ioMock.Object);
            Assert.That(quadrant.KlingonsFireOnEnterprise(), Is.EqualTo(CommandResult.Ok));
        }

        [Test]
        public void KlingonsFireOnEnterprise_CloseToStarbase_EnterpriseProtected()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);
            mockInfo.Setup(i => i.HasStarbase).Returns(true);
            mockInfo.Setup(i => i.StarCount).Returns(0);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(1);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            Mock<Enterprise> mockEnterprise = new(1, new Coordinates(0, 1), ioMock.Object, randomMock.Object);
            mockEnterprise.Setup(e => e.TakeHit(It.IsAny<Coordinates>(), It.IsAny<int>())).Returns(CommandResult.Ok);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.3f)
                .Returns(0.2f)
                .Returns(0.5f)
                .Returns(0f);

            var quadrant = new Quadrant(mockInfo.Object, mockEnterprise.Object, randomMock.Object, mockGalaxy.Object, ioMock.Object);
            quadrant.KlingonsFireOnEnterprise();

            ioMock.Verify(io => io.WriteLine(Strings.Protected), Times.Once());
        }

        [Test]
        public void KlingonsFireOnEnterprise_CloseToStarbase_ReturnsOk()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);
            mockInfo.Setup(i => i.HasStarbase).Returns(true);
            mockInfo.Setup(i => i.StarCount).Returns(0);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(1);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            Mock<Enterprise> mockEnterprise = new(1, new Coordinates(0, 1), ioMock.Object, randomMock.Object);
            mockEnterprise.Setup(e => e.TakeHit(It.IsAny<Coordinates>(), It.IsAny<int>())).Returns(CommandResult.Ok);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.3f)
                .Returns(0.2f)
                .Returns(0.5f)
                .Returns(0f);

            var quadrant = new Quadrant(mockInfo.Object, mockEnterprise.Object, randomMock.Object, mockGalaxy.Object, ioMock.Object);
            var result = quadrant.KlingonsFireOnEnterprise();

            Assert.That(result, Is.EqualTo(CommandResult.Ok));
        }

        [Test]
        public void KlingonsFireOnEnterprise_NotCloseToStarbase_EnterpriseTakesDamage()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);
            mockInfo.Setup(i => i.HasStarbase).Returns(false);
            mockInfo.Setup(i => i.StarCount).Returns(0);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(1);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterpriseCoords = new Coordinates(2, 4);
            Mock<Enterprise> mockEnterprise = new(1, enterpriseCoords, ioMock.Object, randomMock.Object);
            mockEnterprise.Setup(e => e.TakeHit(It.IsAny<Coordinates>(), It.IsAny<int>())).Returns(CommandResult.Ok);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.3f)
                .Returns(0.2f);
            var klingonCoords = new Coordinates((int)(0.3 * 7.98 + 1.01) - 1, (int)(0.2 * 7.98 + 1.01) - 1);

            var quadrant = new Quadrant(mockInfo.Object, mockEnterprise.Object, randomMock.Object, mockGalaxy.Object, ioMock.Object);
            var result = quadrant.KlingonsFireOnEnterprise();

            mockEnterprise.Verify(e => e.TakeHit(klingonCoords, It.IsAny<int>()), Times.Exactly(1));
        }

        [Test]
        public void KlingonsFireOnEnterprise_ReturnsGameOver_IfFireOnEndsGame()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);
            mockInfo.Setup(i => i.HasStarbase).Returns(false);
            mockInfo.Setup(i => i.StarCount).Returns(0);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(1);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterpriseCoords = new Coordinates(2, 4);
            Mock<Enterprise> mockEnterprise = new(1, enterpriseCoords, ioMock.Object, randomMock.Object);
            mockEnterprise.Setup(e => e.TakeHit(It.IsAny<Coordinates>(), It.IsAny<int>())).Returns(CommandResult.GameOver);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.3f)
                .Returns(0.2f);
            var klingonCoords = new Coordinates((int)(0.3 * 7.98 + 1.01) - 1, (int)(0.2 * 7.98 + 1.01) - 1);

            var quadrant = new Quadrant(mockInfo.Object, mockEnterprise.Object, randomMock.Object, mockGalaxy.Object, ioMock.Object);
            var result = quadrant.KlingonsFireOnEnterprise();

            Assert.That(result, Is.EqualTo(CommandResult.GameOver));
        }

        [Test]
        public void KlingonsMoveAndFire_MovesKlingon_ToNewSector()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);
            mockInfo.Setup(i => i.HasStarbase).Returns(false);
            mockInfo.Setup(i => i.StarCount).Returns(0);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(1);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterpriseCoords = new Coordinates(2, 4);
            Mock<Enterprise> mockEnterprise = new(1, enterpriseCoords, ioMock.Object, randomMock.Object);
            mockEnterprise.Setup(e => e.TakeHit(It.IsAny<Coordinates>(), It.IsAny<int>())).Returns(CommandResult.GameOver);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.3f)
                .Returns(0f);
            var klingonCoords = new Coordinates(2, 0);

            var quadrant = new Quadrant(mockInfo.Object, mockEnterprise.Object, randomMock.Object, mockGalaxy.Object, ioMock.Object);
            quadrant.KlingonsMoveAndFire();
            Assert.False(quadrant.HasObjectAt(klingonCoords));
        }

        [Test]
        public void KlingonsMoveAndFire_MovesKlingon_SectorUpdated()
        {
            var mockRandom = new Mock<IRandom>();
            Mock<QuadrantInfo> mockInfo = new(new Coordinates(0, 0), "Test-Quadrant", 1, 1, false);
            mockInfo.Setup(i => i.KlingonCount).Returns(1);
            mockInfo.Setup(i => i.HasStarbase).Returns(false);
            mockInfo.Setup(i => i.StarCount).Returns(0);

            var mockRandomGalaxy = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);
            Mock<Galaxy> mockGalaxy = new(mockRandomGalaxy.Object);
            mockGalaxy.Setup(g => g.KlingonCount).Returns(1);
            mockGalaxy.Setup(g => g.StarbaseCount).Returns(1);
            var randomMock = new Mock<IRandom>();
            var ioMock = new Mock<IReadWrite>();
            var enterpriseCoords = new Coordinates(2, 4);
            Mock<Enterprise> mockEnterprise = new(1, enterpriseCoords, ioMock.Object, randomMock.Object);
            mockEnterprise.Setup(e => e.TakeHit(It.IsAny<Coordinates>(), It.IsAny<int>())).Returns(CommandResult.GameOver);
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.3f)
                .Returns(0f);
            var klingonCoords = new Coordinates(2, 0);

            var quadrant = new Quadrant(mockInfo.Object, mockEnterprise.Object, randomMock.Object, mockGalaxy.Object, ioMock.Object);
            quadrant.KlingonsMoveAndFire();
            Assert.That(quadrant.Klingons.First().Sector, !Is.EqualTo(klingonCoords));
        }
    }
}
