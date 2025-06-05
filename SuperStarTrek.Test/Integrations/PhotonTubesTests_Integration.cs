using Games.Common.IO;
using Games.Common.Randomness;
using Moq;
using NuGet.Frameworks;
using NUnit.Framework;
using SuperStarTrek.Commands;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Systems;
      

namespace SuperStarTrek.Test.Integrations
{
    internal class PhotonTubesTests_Integration
    {
        private IOSpy _io;
        private Mock<IRandom> _random;
        private Enterprise _enterprise;
        private Quadrant _quadrant;
        private Galaxy _galaxy;
        private PhotonTubes _photonTubes;

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

            //_shieldControl = new ShieldControl(_enterprise, _io);
            _photonTubes = new PhotonTubes(2, _enterprise, _io);

            _enterprise.Add(_photonTubes);
        }
    }
}
