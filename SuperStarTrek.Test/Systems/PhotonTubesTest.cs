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
        public void Constructor_InitializesTorpedoCountToTubeCount()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(15, mockEnterprise.Object, mockIO.Object);

            Assert.AreEqual(15, photonTubes.TorpedoCount);
        }

        [Test]
        public void Constructor_SetsCorrectSystemName()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(10, mockEnterprise.Object, mockIO.Object);

            Assert.AreEqual("Photon Tubes", photonTubes.Name);
        }

        

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
        public void Repair_AfterDamage_RestoresOperationalStatus()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(5, mockEnterprise.Object, mockIO.Object);
            photonTubes.TakeDamage(1.0f);

            Assert.IsFalse(photonTubes.CanExecuteCommand());

            photonTubes.Repair();

            Assert.IsTrue(photonTubes.CanExecuteCommand());
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
        public void TakeDamage_MultipleTimes_AccumulatesDamage()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(5, mockEnterprise.Object, mockIO.Object);

            photonTubes.TakeDamage(0.5f);
            photonTubes.TakeDamage(0.5f);

            Assert.AreEqual(-1.0f, photonTubes.Condition);
        }

        [Test]
        public void ReplenishTorpedoes_WhenCompletelyEmpty_RestoresToFullCount()
        {

            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(10, mockEnterprise.Object, mockIO.Object);
            photonTubes.TorpedoCount = 0;

            photonTubes.ReplenishTorpedoes(); 

            Assert.AreEqual(10, photonTubes.TorpedoCount);
            mockIO.Verify(io => io.WriteLine(It.IsAny<string>()), Times.Never);

        }

        [Test]
        public void ReplenishTorpedoes_WhenAlreadyFull_MaintainsFullCount()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(8, mockEnterprise.Object, mockIO.Object);

            photonTubes.ReplenishTorpedoes();

            Assert.AreEqual(8, photonTubes.TorpedoCount);
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
        public void CanExecuteCommand_BothConditionsTrue_ReturnsTrue()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(10, mockEnterprise.Object, mockIO.Object);

            bool result = photonTubes.CanExecuteCommand();

            Assert.IsTrue(result);
            mockIO.Verify(io => io.WriteLine(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void CanExecuteCommand_WithTorpedoesButDamaged_ReturnsFalseWithCorrectMessage()
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

        [Test]
        public void ExecuteCommandCore_WithGameOverHit_ReturnsGameOver()
        {
           
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.SetupSequence(io => io.ReadNumber(It.IsAny<string>()))
                .Returns(1)  
                .Returns(1); 

            
            string hitMessage = "Critical hit! Game over!";
            bool gameOver = true;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out hitMessage, out gameOver))
                       .Returns(true);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            var result = photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(CommandResult.GameOver, result);
            Assert.AreEqual(9, photonTubes.TorpedoCount);
            mockIO.Verify(io => io.WriteLine(hitMessage), Times.Once);
            mockQuadrant.Verify(q => q.KlingonsFireOnEnterprise(), Times.Never);
        }

        [Test]
        public void ExecuteCommandCore_WithValidCourse_ProcessesTorpedo()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.SetupSequence(io => io.ReadNumber(It.IsAny<string>()))
                .Returns(1);  

            string message = "";
            bool gameOver = false;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out message, out gameOver))
                       .Returns(false);

            mockQuadrant.Setup(q => q.KlingonsFireOnEnterprise())
                       .Returns(CommandResult.Ok);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            var result = photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(CommandResult.Ok, result);
            Assert.AreEqual(9, photonTubes.TorpedoCount);
            mockIO.Verify(io => io.WriteLine("Torpedo track:"), Times.Once);
            mockIO.Verify(io => io.WriteLine("                5 , 6"), Times.Once);
            mockIO.Verify(io => io.WriteLine("Torpedo missed!"), Times.Once);
            mockQuadrant.Verify(q => q.KlingonsFireOnEnterprise(), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_WithHit_ReturnsOkAndPrintsMessage()
        {

            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();


            mockIO.SetupSequence(io => io.ReadNumber(It.IsAny<string>()))
                .Returns(1)
                .Returns(1);


            string hitMessage = "Klingon destroyed!";
            bool gameOver = false;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out hitMessage, out gameOver))
                       .Returns(true);

            mockQuadrant.Setup(q => q.KlingonsFireOnEnterprise())
               .Returns(CommandResult.Ok);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            var result = photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(CommandResult.Ok, result);
            Assert.AreEqual(9, photonTubes.TorpedoCount);
            mockIO.Verify(io => io.WriteLine("Torpedo track:"), Times.Once);
            mockIO.Verify(io => io.WriteLine(It.Is<string>(s => s.Contains("5 , 6"))), Times.Once);
            mockIO.Verify(io => io.WriteLine(hitMessage), Times.Once);
            mockQuadrant.Verify(q => q.KlingonsFireOnEnterprise(), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_WithInvalidCourse_ReturnsOkWithoutFiring()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(10);
            mockIO.Setup(io => io.WriteLine(It.IsAny<string>()));

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            var result = photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(CommandResult.Ok, result);
            Assert.AreEqual(10, photonTubes.TorpedoCount); 
            mockQuadrant.Verify(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out It.Ref<string>.IsAny, out It.Ref<bool>.IsAny), Times.Never);
        }

        [Test]
        public void ExecuteCommandCore_WithMissAndKlingonGameOver_ReturnsGameOver()
        {
           
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            
            mockIO.Setup(io => io.ReadNumber(It.IsAny<string>()))
                  .Returns(1);  

            string message = "";
            bool gameOver = false;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out message, out gameOver))
                       .Returns(false);

            mockQuadrant.Setup(q => q.KlingonsFireOnEnterprise())
                       .Returns(CommandResult.GameOver);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            var result = photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(CommandResult.GameOver, result);
            Assert.AreEqual(9, photonTubes.TorpedoCount); 
            mockIO.Verify(io => io.WriteLine("Torpedo track:"), Times.Once);
            mockIO.Verify(io => io.WriteLine("Torpedo missed!"), Times.Once);
            mockQuadrant.Verify(q => q.KlingonsFireOnEnterprise(), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_TorpedoTravelsMultipleSectors_PrintsAllSectors()
        {
            
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            
            mockIO.Setup(io => io.ReadNumber(It.IsAny<string>()))
                  .Returns(1);  

            
            var callCount = 0;
            string hitMessage = "Target destroyed!";
            bool gameOver = false;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out hitMessage, out gameOver))
                       .Returns(() => ++callCount > 2); 

            
            mockQuadrant.Setup(q => q.KlingonsFireOnEnterprise())
                       .Returns(CommandResult.Ok);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            
            var result = photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            
            Assert.AreEqual(CommandResult.Ok, result);
            Assert.AreEqual(9, photonTubes.TorpedoCount);

            
            mockIO.Verify(io => io.WriteLine("Torpedo track:"), Times.Once);

            mockIO.Verify(io => io.WriteLine("                5 , 6"), Times.Once);
            mockIO.Verify(io => io.WriteLine("                5 , 7"), Times.Once);
            mockIO.Verify(io => io.WriteLine("                5 , 8"), Times.Once);

            mockIO.Verify(io => io.WriteLine(hitMessage), Times.Once);
            mockIO.Verify(io => io.WriteLine("Torpedo missed!"), Times.Never);

            mockQuadrant.Verify(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out It.Ref<string>.IsAny, out It.Ref<bool>.IsAny), Times.Exactly(3));

            mockQuadrant.Verify(q => q.KlingonsFireOnEnterprise(), Times.Once);
        }

    }

}

