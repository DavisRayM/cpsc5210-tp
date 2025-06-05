using Games.Common.IO;
using Games.Common.Randomness;
using Moq;
using NuGet.Frameworks;
using NUnit.Framework;
using SuperStarTrek.Commands;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Systems;

namespace SuperStarTrek.Test.Integration
{
    public class ShieldControlTests_Integration
    {
        private IOSpy _io;
        private Mock<IRandom> _random;
        private Enterprise _enterprise;
        private ShieldControl _shieldControl;

        [SetUp]
        public void Setup()
        {
            // Arrange real IO spy and random
            _io = new IOSpy();
            _random = new Mock<IRandom>();
            _random.Setup(r => r.NextFloat()).Returns(0f);

            // Sector coordinates for the enterprise
            var sectorCoords = new Coordinates(1, 1);
            _enterprise = new Enterprise(1000, sectorCoords, _io, _random.Object);

            _shieldControl = new ShieldControl(_enterprise, _io);

            _enterprise.Add(_shieldControl);
        }

        [Test]
        public void TakeHit_ReducesShieldEnergy()
        {
            _shieldControl.ShieldEnergy = 500;
            _enterprise.TakeHit(new Coordinates(1,1), 100);

            Assert.AreEqual(400, _shieldControl.ShieldEnergy);
        }

        [Test]
        public void TakeHit_ShieldEnergyBelowZero_EndGame()
        {
            _shieldControl.ShieldEnergy = 500;
            var result = _enterprise.TakeHit(new Coordinates(1,1), 600);
            Assert.That(result, Is.EqualTo(CommandResult.GameOver));
        }



    }
}

