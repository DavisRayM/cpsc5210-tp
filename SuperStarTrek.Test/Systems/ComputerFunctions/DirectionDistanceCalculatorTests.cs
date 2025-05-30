using NUnit.Framework;
using Moq;
using SuperStarTrek.Systems.ComputerFunctions;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using Games.Common.IO;
using Games.Common.Randomness;
using System.Reflection;

namespace SuperStarTrek.Test.Systems.ComputerFunctions
{
    [TestFixture]
    public class DirectionDistanceCalculatorTests
    {
        private Mock<IReadWrite> _mockIo;
        private Mock<IRandom> _mockRandom;
        private Galaxy _galaxy;
        private Enterprise _enterprise;
        private IQuadrant _quadrant;
        private DirectionDistanceCalculator _calculator;

        public void Setup(
            int quadrantX, int quadrantY,
            int sectorX, int sectorY
            //(float, float) initialInput,
            //(float, float) finalInput
        )
        {
            // Setup Io and Random
            _mockIo = new Mock<IReadWrite>();
            _mockRandom = new Mock<IRandom>();

            _mockIo.SetupSequence(io => io.Read2Numbers(It.IsAny<string>()))
                .Returns((1, 2))
                .Returns((3, 4));

            _mockRandom.Setup(r => r.NextFloat()).Returns(0f);

            // Setup Coordinates, Galaxy, Enterprise, and Quadrant
            var quadrantCoords = new Coordinates(quadrantX, quadrantY);
            var sectorCoords = new Coordinates(sectorX, sectorY);

            _galaxy = new Galaxy(_mockRandom.Object);

            _enterprise = new Enterprise(
                1000, 
                sectorCoords, 
                _mockIo.Object, 
                _mockRandom.Object
            );

            var quadrantInfo = new QuadrantInfo(quadrantCoords, "Test Quadrant", 0, 0, false);
            _quadrant = new Quadrant(
                quadrantInfo, 
                _enterprise, 
                _mockRandom.Object, 
                _galaxy, 
                _mockIo.Object
            );

            typeof(Enterprise)
                .GetField("_quadrant", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_enterprise, _quadrant);

            // Setup test
            _calculator = new DirectionDistanceCalculator(_enterprise, _mockIo.Object);
        }

        [TestCase(1, 2, 3, 4)]
        [TestCase(0, 0, 7, 7)]
        [TestCase(5, 6, 1, 1)]
        public void Execute_PrintsExpectedLocationAndPromptsForCoordinates(
            int quadrantX, int quadrantY,
            int sectorX, int sectorY
        )
        {
            Setup(quadrantX, quadrantY, sectorX, sectorY);
            var quadrantCoords = new Coordinates(quadrantX, quadrantY);
            var sectorCoords = new Coordinates(sectorX, sectorY);
            _mockIo.Invocations.ToList().ForEach(i => Console.WriteLine(i));

            _calculator.Execute(_quadrant);

            _mockIo.Verify(io => io.WriteLine("Direction/distance calculator:"), Times.Once);
            _mockIo.Verify(io => io.Write($"You are at quadrant {quadrantCoords}"), Times.Once);
            _mockIo.Verify(io => io.WriteLine($" sector {sectorCoords}"), Times.Once);
            _mockIo.Verify(io => io.WriteLine("Please enter"), Times.Once);
            _mockIo.Verify(io => io.Read2Numbers(It.IsAny<string>()), Times.Exactly(2)); 
        }
    }

}
