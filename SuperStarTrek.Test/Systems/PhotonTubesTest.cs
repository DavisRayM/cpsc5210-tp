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
        #region ConstructorTests

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
        public void Constructor_WithZeroTorpedoes_InitializesCorrectly()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(0, mockEnterprise.Object, mockIO.Object);

            Assert.AreEqual(0, photonTubes.TorpedoCount);
        }

        [Test]
        public void Constructor_WithZeroTorpedoes_CannotExecuteCommand()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(0, mockEnterprise.Object, mockIO.Object);

            Assert.IsFalse(photonTubes.CanExecuteCommand());
        }

        [Test]
        public void Constructor_WithMaximumTorpedoes_InitializesTorpedoCountCorrectly()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(int.MaxValue, mockEnterprise.Object, mockIO.Object);

            Assert.AreEqual(int.MaxValue, photonTubes.TorpedoCount);
        }

        [Test]
        public void Constructor_WithMaximumTorpedoes_CanExecuteCommand()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(int.MaxValue, mockEnterprise.Object, mockIO.Object);

            Assert.IsTrue(photonTubes.CanExecuteCommand());
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

        #endregion ConstructorTests


        #region DamageRepairTests

        [Test]
        public void TakeDamage_DamagesSystemCondition()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(10, mockEnterprise.Object, mockIO.Object);
            photonTubes.TakeDamage(0.5f);

            Assert.AreEqual(-0.5f, photonTubes.Condition);
        }

        [Test]
        public void TakeDamage_PreventsCommandExecution()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(5, mockEnterprise.Object, mockIO.Object);
            photonTubes.TakeDamage(1.0f);

            Assert.IsFalse(photonTubes.CanExecuteCommand());
        }

        [Test]
        public void Repair_FixesSystemCondition()
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

            photonTubes.Repair();

            Assert.IsTrue(photonTubes.CanExecuteCommand());
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

        #endregion DamageRepairTests


        #region TorpedoManagementTests

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

        #endregion TorpedoManagementTests


        #region CanExecuteCommandTests

        [Test]
        public void CanExecuteCommand_WithNoTorpedoes_ReturnsFalse()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(0, mockEnterprise.Object, mockIO.Object);

            bool result = photonTubes.CanExecuteCommand();

            Assert.IsFalse(result);
        }

        [Test]
        public void CanExecuteCommand_WithNoTorpedoes_PrintsExpendedMessage()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(0, mockEnterprise.Object, mockIO.Object);

            photonTubes.CanExecuteCommand();

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
        public void CanExecuteCommand_WithTorpedoesAndOperational_DoesNotPrintMessage()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var photonTubes = new PhotonTubes(10, mockEnterprise.Object, mockIO.Object);

            photonTubes.CanExecuteCommand();

            mockIO.Verify(io => io.WriteLine(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void CanExecuteCommand_WithTorpedoesButDamaged_ReturnsFalse()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var photonTubes = new PhotonTubes(10, mockEnterprise.Object, mockIO.Object);

            photonTubes.TakeDamage(1.0f);

            bool result = photonTubes.CanExecuteCommand();

            Assert.IsFalse(result);
        }

        [Test]
        public void CanExecuteCommand_WithTorpedoesButDamaged_PrintsNotOperationalMessage()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var photonTubes = new PhotonTubes(10, mockEnterprise.Object, mockIO.Object);

            photonTubes.TakeDamage(1.0f);

            photonTubes.CanExecuteCommand();

            mockIO.Verify(io => io.WriteLine("Photon Tubes are not operational"), Times.Once);
        }

        #endregion CanExecuteCommandTests


        #region ExecuteCommandCoreGameOverTests

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
        }

        [Test]
        public void ExecuteCommandCore_WithGameOverHit_DecrementsTorpedoCount()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(9, photonTubes.TorpedoCount);
        }

        [Test]
        public void ExecuteCommandCore_WithGameOverHit_PrintsHitMessage()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine(hitMessage), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_WithGameOverHit_DoesNotCallKlingonsFire()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockQuadrant.Verify(q => q.KlingonsFireOnEnterprise(), Times.Never);
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
        }

        #endregion ExecuteCommandCoreGameOverTests


        #region ExecuteCommandCoreValidCourseTests

        [Test]
        public void ExecuteCommandCore_WithValidCourse_ReturnsOk()
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
        }

        [Test]
        public void ExecuteCommandCore_WithValidCourse_DecrementsTorpedoCount()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(9, photonTubes.TorpedoCount);
        }

        [Test]
        public void ExecuteCommandCore_WithValidCourseMiss_CallsKlingonsFire()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockQuadrant.Verify(q => q.KlingonsFireOnEnterprise(), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_WithValidCourse_DisplaysTorpedoTrackHeader()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Torpedo track:"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_WithValidCourse_DisplaysTorpedoCoordinates()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("                5 , 6"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_WithValidCourseWhenTorpedoMisses_DisplaysMissMessage()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Torpedo missed!"), Times.Once);
        }

        #endregion ExecuteCommandCoreValidCourseTests


        #region ExecuteCommandCoreHitTests

        [Test]
        public void ExecuteCommandCore_WithHit_CallsKlingonsFire()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockQuadrant.Verify(q => q.KlingonsFireOnEnterprise(), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_WithHit_ReturnsOk()
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
        }

        [Test]
        public void ExecuteCommandCore_WithHit_DecrementsTorpedoCount()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(9, photonTubes.TorpedoCount);
        }

        [Test]
        public void ExecuteCommandCore_WithHit_PrintsTorpedoTrackHeader()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Torpedo track:"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_WithHit_PrintsTorpedoPosition()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine(It.Is<string>(s => s.Contains("5 , 6"))), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_WithHit_PrintsHitMessage()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine(hitMessage), Times.Once);
        }

        #endregion ExecuteCommandCoreHitTests


        #region ExecuteCommandCoreInvalidCourseTests

        [Test]
        public void ExecuteCommandCore_WithInvalidCourse_ReturnsOk()
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
        }

        [Test]
        public void ExecuteCommandCore_WithInvalidCourse_DoesNotDecrementTorpedoCount()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(10);
            mockIO.Setup(io => io.WriteLine(It.IsAny<string>()));

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(10, photonTubes.TorpedoCount);
        }

        [Test]
        public void ExecuteCommandCore_WithInvalidCourse_DoesNotCheckForTorpedoCollision()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(10);
            mockIO.Setup(io => io.WriteLine(It.IsAny<string>()));

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockQuadrant.Verify(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out It.Ref<string>.IsAny, out It.Ref<bool>.IsAny), Times.Never);
        }

        #endregion ExecuteCommandCoreInvalidCourseTests


        #region ExecuteCommandCoreTorpedoTravelsMultipleSectors

        [Test]
        public void ExecuteCommandCore_TorpedoTravelsMultipleSectors_ReturnsOk()
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
        }

        [Test]
        public void ExecuteCommandCore_TorpedoTravelsMultipleSectors_DecrementsTorpedoCount()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(9, photonTubes.TorpedoCount);
        }

        [Test]
        public void ExecuteCommandCore_TorpedoTravelsMultipleSectors_PrintsTorpedoTrackHeader()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Torpedo track:"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_TorpedoTravelsMultipleSectors_PrintsAllSectorPositions()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("                5 , 6"), Times.Once);
            mockIO.Verify(io => io.WriteLine("                5 , 7"), Times.Once);
            mockIO.Verify(io => io.WriteLine("                5 , 8"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_TorpedoTravelsMultipleSectors_PrintsHitMessage()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine(hitMessage), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_TorpedoTravelsMultipleSectors_DoesNotPrintMissMessage()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Torpedo missed!"), Times.Never);
        }

        [Test]
        public void ExecuteCommandCore_TorpedoTravelsMultipleSectors_ChecksCollisionCorrectNumberOfTimes()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockQuadrant.Verify(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out It.Ref<string>.IsAny, out It.Ref<bool>.IsAny), Times.Exactly(3));
        }

        [Test]
        public void ExecuteCommandCore_TorpedoTravelsMultipleSectors_CallsKlingonsFire()
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

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockQuadrant.Verify(q => q.KlingonsFireOnEnterprise(), Times.Once);
        }

        #endregion ExecuteCommandCoreTorpedoTravelsMultipleSectors


        #region ExecuteCommandCoreReadInputValidCourseTests

        [Test]
        public void ExecuteCommandCore_ReadNumberReturnsValidCourse_ReturnsOk()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(5);

            string message = "";
            bool gameOver = false;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out message, out gameOver))
                       .Returns(false);
            mockQuadrant.Setup(q => q.KlingonsFireOnEnterprise()).Returns(CommandResult.Ok);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            var result = photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(CommandResult.Ok, result);
        }

        [Test]
        public void ExecuteCommandCore_ReadNumberReturnsValidCourse_DecrementsTorpedoCount()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(5);

            string message = "";
            bool gameOver = false;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out message, out gameOver))
                       .Returns(false);
            mockQuadrant.Setup(q => q.KlingonsFireOnEnterprise()).Returns(CommandResult.Ok);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(9, photonTubes.TorpedoCount);
        }

        [Test]
        public void ExecuteCommandCore_ReadNumberReturnsValidCourse_DoesNotShowErrorMessage()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(5);

            string message = "";
            bool gameOver = false;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out message, out gameOver))
                       .Returns(false);
            mockQuadrant.Setup(q => q.KlingonsFireOnEnterprise()).Returns(CommandResult.Ok);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Ensign Chekov reports, 'Incorrect course data, sir!'"), Times.Never);
        }

        [Test]
        public void ExecuteCommandCore_ReadNumberReturnsValidCourse_ChecksForTorpedoCollision()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(5);

            string message = "";
            bool gameOver = false;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out message, out gameOver))
                       .Returns(false);
            mockQuadrant.Setup(q => q.KlingonsFireOnEnterprise()).Returns(CommandResult.Ok);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockQuadrant.Verify(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(),
                out It.Ref<string>.IsAny, out It.Ref<bool>.IsAny), Times.AtLeastOnce);
        }

        [Test]
        public void ExecuteCommandCore_ReadNumberReturnsValidCourse_PrintsTorpedoTrackHeader()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(5);

            string message = "";
            bool gameOver = false;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out message, out gameOver))
                       .Returns(false);
            mockQuadrant.Setup(q => q.KlingonsFireOnEnterprise()).Returns(CommandResult.Ok);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Torpedo track:"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_ReadNumberReturnsValidCourse_CallsKlingonsFire()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(5);

            string message = "";
            bool gameOver = false;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out message, out gameOver))
                       .Returns(false);
            mockQuadrant.Setup(q => q.KlingonsFireOnEnterprise()).Returns(CommandResult.Ok);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockQuadrant.Verify(q => q.KlingonsFireOnEnterprise(), Times.Once);
        }

        #endregion ExecuteCommandCoreReadInputValidCourseTests


        #region ExecuteCommandCoreReadInputFloatingPointValidCourseTests

        [Test]
        public void ExecuteCommandCore_FloatingPointValidCourse_ReturnsOk()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(5.7f);

            string message = "";
            bool gameOver = false;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out message, out gameOver))
                       .Returns(false);
            mockQuadrant.Setup(q => q.KlingonsFireOnEnterprise()).Returns(CommandResult.Ok);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            var result = photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(CommandResult.Ok, result);
        }

        [Test]
        public void ExecuteCommandCore_FloatingPointValidCourse_FiresTorpedo()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(5.7f);

            string message = "";
            bool gameOver = false;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out message, out gameOver))
                       .Returns(false);
            mockQuadrant.Setup(q => q.KlingonsFireOnEnterprise()).Returns(CommandResult.Ok);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(9, photonTubes.TorpedoCount);
        }

        [Test]
        public void ExecuteCommandCore_FloatingPointValidCourse_DoesNotShowErrorMessage()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(5.7f);

            string message = "";
            bool gameOver = false;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out message, out gameOver))
                       .Returns(false);
            mockQuadrant.Setup(q => q.KlingonsFireOnEnterprise()).Returns(CommandResult.Ok);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Ensign Chekov reports, 'Incorrect course data, sir!'"), Times.Never);
        }

        [Test]
        public void ExecuteCommandCore_FloatingPointValidCourse_PrintsTorpedoTrack()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(5.7f);

            string message = "";
            bool gameOver = false;
            mockQuadrant.Setup(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(), out message, out gameOver))
                       .Returns(false);
            mockQuadrant.Setup(q => q.KlingonsFireOnEnterprise()).Returns(CommandResult.Ok);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Torpedo track:"), Times.Once);
        }

        #endregion ExecuteCommandCoreReadInputFloatingPointValidCourseTests


        #region ExecuteCommandCoreReadInputInvalidCourseTests

        [Test]
        public void ExecuteCommandCore_ReadNumberPromptFormat_UsesCorrectPromptString()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(0);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);
            photonTubes.ExecuteCommandCore(mockQuadrant.Object);


            mockIO.Verify(io => io.ReadNumber("Photon torpedo course (1-9)"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_ReadNumberReturnsInvalidCourse_ReturnsOk()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(0);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            var result = photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(CommandResult.Ok, result);
        }

        [Test]
        public void ExecuteCommandCore_ReadNumberReturnsInvalidCourse_DoesNotDecrementTorpedoCount()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(0);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            Assert.AreEqual(10, photonTubes.TorpedoCount);
        }

        [Test]
        public void ExecuteCommandCore_ReadNumberReturnsInvalidCourse_ShowsErrorMessage()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(0);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Ensign Chekov reports, 'Incorrect course data, sir!'"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_ReadNumberReturnsInvalidCourse_DoesNotCheckForTorpedoCollision()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(0);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockQuadrant.Verify(q => q.TorpedoCollisionAt(It.IsAny<Coordinates>(),
                out It.Ref<string>.IsAny, out It.Ref<bool>.IsAny), Times.Never);
        }

        [Test]
        public void ExecuteCommandCore_ReadNumberReturnsInvalidCourse_DoesNotCallKlingonsFire()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(0);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockQuadrant.Verify(q => q.KlingonsFireOnEnterprise(), Times.Never);
        }

        [Test]
        public void ExecuteCommandCore_ReadNumberReturnsInvalidCourse_DoesNotPrintTorpedoTrack()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(1000, new Coordinates(4, 4), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();

            mockIO.Setup(io => io.ReadNumber("Photon torpedo course (1-9)")).Returns(0);

            var photonTubes = new PhotonTubes(10, enterprise, mockIO.Object);

            photonTubes.ExecuteCommandCore(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Torpedo track:"), Times.Never);
        }

        #endregion ExecuteCommandCoreReadInputInvalidCourseTests


    }

}

