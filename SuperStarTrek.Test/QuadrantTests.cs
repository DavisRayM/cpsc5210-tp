using System;
using NUnit.Framework;
using Moq;
using SuperStarTrek.Space;
using SuperStarTrek.Objects;
using Games.Common.IO;
using Games.Common.Randomness;
using System.Collections.Generic;

namespace SuperStarTrek.Test.Space
{
    [TestFixture]
    public class QuadrantTests
    {
        private Mock<IReadWrite> _mockIO;
        private Mock<IRandom> _mockRandom;
        private Mock<Galaxy> _mockGalaxy;
        private Mock<QuadrantInfo> _mockQuadrantInfo;
        private Enterprise _enterprise;


        [SetUp]
        public void Setup()
        {
            // Setup mocks and test objects
            _mockIO = new Mock<IReadWrite>();
            _mockRandom = new Mock<IRandom>();
            _mockGalaxy = new Mock<Galaxy>();
            _mockQuadrantInfo = new Mock<QuadrantInfo>();

            // Setup mock QuadrantInfo
            _mockQuadrantInfo.Setup(q => q.Coordinates).Returns(new Coordinates(1, 1));
            _mockQuadrantInfo.Setup(q => q.KlingonCount).Returns(2);
            _mockQuadrantInfo.Setup(q => q.StarCount).Returns(3);
            _mockQuadrantInfo.Setup(q => q.HasStarbase).Returns(true);
            _mockQuadrantInfo.Setup(q => q.Name).Returns("Test Quadrant");

            // Setup enterprise with default position
            _enterprise = new Enterprise(5000, sector: new Coordinates(0, 0), io: _mockRandom.Object, random: _mockIO.Object);
        }

        [Test]
        public void Constructor_ShouldPositionObjectsCorrectly()
        {
            var quadrant = new Quadrant(_mockQuadrantInfo.Object.
                               _enterprise,
                               _mockRandom.Object,
                               _mockGalaxy.Object,
                               _mockIO.Object);

            Assert.That(quadrant.Coordinates, Is.EqualTo(new Coordinates(1, 1)));

        }
    }
}
