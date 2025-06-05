using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Systems.ComputerFunctions;
using Games.Common.IO;
using Games.Common.Randomness;
using SuperStarTrek.Resources;
using SuperStarTrek.Utils;

namespace SuperStarTrek.Test.Systems.ComputerFunctions
{
    [TestFixture]
    public class TorpedoDataCalculatorTests
    {
        private IOSpy _ioSpy;
        private Mock<IRandom> _mockRandom;
        private Mock<Enterprise> _enterprise;
        private TorpedoDataCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            // Mock IO and Random
            _ioSpy = new IOSpy();
            _mockRandom = new Mock<IRandom>();

            // Mock Enterprise
            _enterprise = new Mock<Enterprise>(
                1000, 
                new Coordinates(0, 0), 
                _ioSpy,
                _mockRandom.Object
            );
        }

        [Test]
        public void Execute_NoKlingons_PrintsNoEnemyShips()
        {
            var mockQuadrant = new Mock<IQuadrant>();
            mockQuadrant.Setup(q => q.HasKlingons).Returns(false);

            _calculator = new TorpedoDataCalculator(_enterprise.Object, _ioSpy);
            _calculator.Execute(mockQuadrant.Object);

            string output = _ioSpy.GetOutput();
            Assert.That(output.Trim(), Is.EqualTo(Strings.NoEnemyShips));
        }

        [Test]
        public void Execute_WithKlingons_PrintsDirectionAndDistanceForEach()
        {
            var enterpriseSector = new Coordinates(5, 7);
            var klingonSector = new Coordinates(5, 5);

            // Mock Klingons
            var mockKlingon = new Mock<Klingon>(MockBehavior.Loose, klingonSector, _mockRandom.Object);

            // Mock Quadrant
            var mockQuadrant = new Mock<IQuadrant>();
            mockQuadrant.Setup(q => q.HasKlingons).Returns(true);
            mockQuadrant.Setup(q => q.KlingonCount).Returns(2);
            mockQuadrant.Setup(q => q.Klingons).Returns(new[] {
                mockKlingon.Object
            });

            // Setup Enterprise
            _enterprise.Object.SectorCoordinates = enterpriseSector;

            // Need to get direction and distance from enterprise to klington
            var (direction, distance) = DirectionAndDistance
                .From(_enterprise.Object.SectorCoordinates.X, _enterprise.Object.SectorCoordinates.Y)
                .To(mockKlingon.Object.Sector.X, mockKlingon.Object.Sector.Y);

            // Instantiate calculator
            _calculator = new TorpedoDataCalculator(_enterprise.Object, _ioSpy);

            // Act
            _calculator.Execute(mockQuadrant.Object);

            string output = _ioSpy.GetOutput();
            string[] expectedOutput = new[]
            {
                "From Enterprise to Klingon battle cruisers",
                $"Direction = {direction}",
                $"Distance = {distance}", 
                ""
            };

            Assert.That(output, Is.EqualTo(string.Join(Environment.NewLine, expectedOutput)));
        }
    }
}
