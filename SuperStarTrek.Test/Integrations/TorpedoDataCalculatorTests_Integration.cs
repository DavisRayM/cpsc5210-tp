using NUnit.Framework;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Systems.ComputerFunctions;
using Games.Common.IO;
using Games.Common.Randomness;
using Moq;

namespace SuperStarTrek.Test.Integration
{
    [TestFixture]
    public class TorpedoDataCalculatorTests_Integration
    {
        private IOSpy _io;
        private Enterprise _enterprise;
        private Quadrant _quadrant;
        private Galaxy _galaxy;
        private TorpedoDataCalculator _calculator;

        [SetUp]
        public void SetUp()
        {
            // Arrange real IO spy and random
            _io = new IOSpy();
            var random = new Mock<IRandom>();
            random.Setup(r => r.NextFloat()).Returns(0f); 

            // Sector coordinates for the enterprise
            var sectorCoords = new Coordinates(2, 2);
            _enterprise = new Enterprise(1000, sectorCoords, _io, random.Object);

            // Create a quadrant with Klingons
            var quadrantInfo = new QuadrantInfo(
                coordinates: new Coordinates(3, 3),
                name: "Alpha Region",
                klingonCount: 1,
                starCount: 0,
                hasStarbase: false
            );

            _galaxy = new Galaxy(random.Object);
            _quadrant = new Quadrant(quadrantInfo, _enterprise, random.Object, _galaxy, _io);

            // Inject quadrant into Enterprise
            typeof(Enterprise).GetField(
                "_quadrant", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            )
                ?.SetValue(_enterprise, _quadrant);

            _calculator = new TorpedoDataCalculator(_enterprise, _io);
        }

        [Test]
        public void Execute_PrintsKlingonHeader()
        {
            _calculator.Execute(_quadrant);

            var output = _io.GetOutput();
            StringAssert.Contains("From Enterprise to Klingon battle cruiser", output);
        }

        [Test]
        public void Execute_PrintsDirection()
        {
            _calculator.Execute(_quadrant);

            var output = _io.GetOutput();
            StringAssert.Contains("Direction =", output);
        }

        [Test]
        public void Execute_PrintsDistance()
        {
            _calculator.Execute(_quadrant);

            var output = _io.GetOutput();
            StringAssert.Contains("Distance =", output);
        }
    }
}
