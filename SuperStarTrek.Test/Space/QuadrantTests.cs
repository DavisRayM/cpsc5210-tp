using System;
using NUnit.Framework;
using Moq;
using SuperStarTrek.Space;
using SuperStarTrek.Objects;
using Games.Common.IO;
using Games.Common.Randomness;
using System.Collections.Generic;

namespace SuperStarTrek.Space
{
    [TestFixture]
    public class QuadrantTests
    {
        private const string Name = "Test-Quadrant";
        private Mock<IRandom> randomMock;
        private Mock<IReadWrite> ioMock;
        private Quadrant quadrant;
        

        [Test]
        public void Create_ExpectedCoordinates_CreateFactory()
        {
            randomMock = new Mock<IRandom>();
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
            randomMock = new Mock<IRandom>();
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
            randomMock = new Mock<IRandom>();
            ioMock = new Mock<IReadWrite>();
            var quadrantInfoCoord = new Coordinates(1, 1);
            var enterpriseCoord = new Coordinates(2, 2);

            var galaxy = new Galaxy(randomMock.Object);
            var quadrantInfo = QuadrantInfo.Create(quadrantInfoCoord, "Test-Quadrant", randomMock.Object);
            var enterprise = new Enterprise(5000, enterpriseCoord, ioMock.Object, randomMock.Object);
            var quadrant = new Quadrant(quadrantInfo, enterprise, randomMock.Object, galaxy, ioMock.Object);

            Assert.AreEqual(new Coordinates(1, 1), quadrant.Coordinates);
        }

        [Ignore("Incomplete")]
        [Test]
        public void TorpedoCollisionAt_WithKlingon_ShouldRemoveKlingonAndReturnTrueAndNotEndGame()
        {
            randomMock = new Mock<IRandom>();
            randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.80f)
                .Returns(0.80f)
                .Returns(0.80f);
            ioMock = new Mock<IReadWrite>();
            var quadrantInfoCoord = new Coordinates(1, 1);
            var enterpriseCoord = new Coordinates(2, 2);
            var klingonCoord = new Coordinates(3, 3);

            var galaxy = new Galaxy(randomMock.Object);
            var quadrantInfo = QuadrantInfo.Create(quadrantInfoCoord, "Test-Quadrant", randomMock.Object);
            var enterprise = new Enterprise(5000, enterpriseCoord, ioMock.Object, randomMock.Object);
            var quadrant = new Quadrant(quadrantInfo, enterprise, randomMock.Object, galaxy, ioMock.Object);

            bool hit = quadrant.TorpedoCollisionAt(klingonCoord, out string message, out bool gameOver);

            //Assert.That(quadrantInfo.KlingonCount, Is.EqualTo(1));
            //Assert.That(galaxy.KlingonCount, Is.EqualTo(1));
            //Assert.IsTrue(hit);
        }



        [Test]
        public void TorpedoCollisionAt_WithKlingon_ShouldRemoveKlingonAndReturnTrueAndEndGame()
        { }

        [Test]
        public void TorpedoCollisionAt_WithStar_ShouldReturnTrue()
        { }

        [Test]
        public void TorpedoCollisionAt_WithStarbase_ShouldRemoveSectorAndReturnTrue()
        { }

        [Test]
        public void TorpedoCollisionAt_WithStarbase_ShouldRemoveStarbaseAndReturnTrue()
        { }

        [Test]
        public void TorpedoCollisionAt_WithStarbase_ShouldRemoveStarbaseAndEndGame()
        { }

        [Test]
        public void TorpedoCollisonAt_Default_ShouldReturnFalse()
        { }

        [Ignore("Incomplete")]
        [Test]
        public void EnterpriseIsNextToStarbase_WhenAdjacent_ShouldReturnTrue()
        {
            randomMock = new Mock<IRandom>();
            ioMock = new Mock<IReadWrite>();
            var galaxy = new Galaxy(randomMock.Object);
            var quadrantInfoCoord = new Coordinates(1, 1);
            var enterpriseCoord = new Coordinates(2, 2);
            randomMock.Setup(r => r.NextCoordinate()).Returns(new Coordinates(3, 3));
            var quadrantInfo = QuadrantInfo.Create(quadrantInfoCoord, "Test-Quadrant", randomMock.Object);
            var enterprise = new Enterprise(5000, new Coordinates(2, 3), ioMock.Object, randomMock.Object);
            var quadrant = new Quadrant(quadrantInfo, enterprise, randomMock.Object, galaxy, ioMock.Object);

            Assert.IsTrue(quadrant.EnterpriseIsNextToStarbase);
        }
    }
}
