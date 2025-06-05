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
        private IOSpy _ioSpy;
        private Mock<IRandom> _mockRandom;
        private Enterprise _enterprise;
        private TorpedoDataCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _ioSpy = new IOSpy();
            _mockRandom = new Mock<IRandom>();

            var sectorCoords = new Coordinates(3, 3);
            _enterprise = new Enterprise(
                1000, 
                sectorCoords, 
                _ioSpy,
                _mockRandom.Object
            );

            _calculator = new TorpedoDataCalculator(_enterprise, _ioSpy);
        }

        [Test]
        public void Execute_NoKlingons_PrintsNoEnemyShips()
        {
            var mockQuadrant = new Mock<IQuadrant>();
            mockQuadrant.Setup(q => q.HasKlingons).Returns(false);

            _calculator.Execute(mockQuadrant.Object);

            string output = _ioSpy.GetOutput();
            Assert.That(output.Trim(), Is.EqualTo(Strings.NoEnemyShips));
        }

        [Test]
        public void Execute_WithKlingons_PrintsDirectionAndDistanceForEach()
        {
            var klingon1Sector = new Coordinates(5, 5);
            var klingon2Sector = new Coordinates(5, 1);

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

            string output = _ioSpy.GetOutput();
            string[] lines = output.Trim().Split('\n');

            Assert.That(lines.Length, Is.EqualTo(5));
        }
    }
}
