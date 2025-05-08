using Moq;
using Games.Common.IO;
using Games.Common.Randomness;
using SuperStarTrek.Systems;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Commands;
using NUnit.Framework;

namespace SuperStarTrek.Test.Systems
{
	[TestFixture]
	public class PhotonTubesTest
	{
		
		private Mock<IReadWrite> _mockIO;
		private Enterprise _enterprise;
		private Mock<IQuadrant> _mockQuadrant;
		private PhotonTubes _photonTubes;
		private const int INITIAL_TORPEDO_COUNT = 10;


        [SetUp]
        public void Setup()
        {

            _mockIO = new Mock<IReadWrite>();
            var _mockRandom = new Mock<IRandom>();
            _enterprise = new Enterprise(1000, new Coordinates(2, 3), _mockIO.Object, _mockRandom.Object);
            _mockQuadrant = new Mock<IQuadrant>();

            _photonTubes = new PhotonTubes(INITIAL_TORPEDO_COUNT, _enterprise, _mockIO.Object);

        }


        [Test]
        public void Constructor_InitializesWithCorrectValues()
        {
            
            Assert.AreEqual(INITIAL_TORPEDO_COUNT, _photonTubes.TorpedoCount);
            Assert.AreEqual("Photon Tubes", _photonTubes.Name);
            Assert.AreEqual(Command.TOR, _photonTubes.Command);
        }




    }
}

