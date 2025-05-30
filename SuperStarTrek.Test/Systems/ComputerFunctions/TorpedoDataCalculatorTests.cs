using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Systems.ComputerFunctions;
using Games.Common.IO;
using Games.Common.Randomness;
using SuperStarTrek.Resources;

namespace SuperStarTrek.Test.Systems.ComputerFunctions
{
    [TestFixture]
    public class TorpedoDataCalculatorTests
    {
        private Mock<IReadWrite> _mockIo = null!;
        private Mock<IRandom> _mockRandom = null!;
        private Enterprise _enterprise = null!;
        private TorpedoDataCalculator _calculator = null!;

        [SetUp]
        public void Setup()
        {
            _mockIo = new Mock<IReadWrite>();
            _mockRandom = new Mock<IRandom>();

            var sectorCoords = new Coordinates(3, 3);
            _enterprise = new Enterprise(1000, sectorCoords, _mockIo.Object, _mockRandom.Object);

            _calculator = new TorpedoDataCalculator(_enterprise, _mockIo.Object);
        }

        [Test]
        public void Execute_NoKlingons_PrintsNoEnemyShips()
        {
            var mockQuadrant = new Mock<IQuadrant>();
            mockQuadrant.Setup(q => q.HasKlingons).Returns(false);

            _calculator.Execute(mockQuadrant.Object);

            _mockIo.Verify(io => io.WriteLine(Strings.NoEnemyShips), Times.Once);
        }

        [Test]
        public void Execute_WithKlingons_PrintsDirectionAndDistanceForEach()
        {
            var klingon1Sector = new Coordinates(5, 5);
            var klingon2Sector = new Coordinates(1, 7);

            var mockKlingon1 = new Mock<Klingon>(MockBehavior.Loose, klingon1Sector, _mockRandom.Object);
            var mockKlingon2 = new Mock<Klingon>(MockBehavior.Loose, klingon2Sector, _mockRandom.Object);

            var mockQuadrant = new Mock<IQuadrant>();
            mockQuadrant.Setup(q => q.HasKlingons).Returns(true);
            mockQuadrant.Setup(q => q.KlingonCount).Returns(2);
            mockQuadrant.Setup(q => q.Klingons).Returns(new[] {
                mockKlingon1.Object,
                mockKlingon2.Object
            });

            _calculator.Execute(mockQuadrant.Object);

            _mockIo.Verify(io => io.WriteLine("From Enterprise to Klingon battle cruisers"), Times.Once);
            _mockIo.Verify(io => io.WriteLine(It.Is<string>(s => s.StartsWith("Direction ="))), Times.Exactly(2));
            _mockIo.Verify(io => io.WriteLine(It.Is<string>(s => s.StartsWith("Distance ="))), Times.Exactly(2));
        }
    }
}
