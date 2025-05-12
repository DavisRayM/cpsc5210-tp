using Moq;
using Games.Common.IO;
using Games.Common.Randomness;
using SuperStarTrek.Systems;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Commands;


namespace SuperStarTrek.Test.Systems
{
    public class PhotonTubesTest
    {

        [Test]
        public void Repair_FixesSystem()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(10, mockEnterprise.Object, mockIO.Object);
            photonTubes.TakeDamage(0.5f);

            photonTubes.Repair();


            Assert.AreEqual(0.0f, photonTubes.Condition);
        }

        [Test]
        public void TakeDamage_DamagesSystem()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(10, mockEnterprise.Object, mockIO.Object);
            photonTubes.TakeDamage(0.5f);

            Assert.AreEqual(-0.5f, photonTubes.Condition);
        }

        [Test]
        public void ReplenishTorpedoes_ResetsToInitialTorpedoCount()
        {

            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(10, mockEnterprise.Object, mockIO.Object);
            photonTubes.TorpedoCount = 3;

            photonTubes.ReplenishTorpedoes(); // TODO: verify if making set non-private is ok

            Assert.AreEqual(10, photonTubes.TorpedoCount);
            mockIO.Verify(io => io.WriteLine(It.IsAny<string>()), Times.Never);

        }

        [Test]
        public void CanExecuteCommand_WithNoTorpedoes_ReturnsFalseAndPrintsMessage()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(0, mockEnterprise.Object, mockIO.Object);

            bool result = photonTubes.CanExecuteCommand();

            Assert.IsFalse(result);

            mockIO.Verify(io => io.WriteLine("All photon torpedoes expended"), Times.Once);
        }

        [Test]
        public void CanExecuteCommand_WithTorpedoesAndOperational_ReturnsTrue()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(10, mockEnterprise.Object, mockIO.Object);

            bool result = photonTubes.CanExecuteCommand();

            Assert.IsTrue(result);
        }

        [Test]
        public void CanExecuteCommand_WhenDamaged_ReturnsFalse()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var photonTubes = new PhotonTubes(10, mockEnterprise.Object, mockIO.Object);

            photonTubes.TakeDamage(1.0f);

            bool result = photonTubes.CanExecuteCommand();


            Assert.IsFalse(result);
            mockIO.Verify(io => io.WriteLine("Photon Tubes are not operational"), Times.Once);
        }


    }

}

