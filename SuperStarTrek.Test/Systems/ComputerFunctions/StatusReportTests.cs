using Games.Common.IO;
using Games.Common.Randomness;
using Moq;
using SuperStarTrek.Commands;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Systems.ComputerFunctions;

namespace SuperStarTrek.Test.Systems.ComputerFunctions
{
    public class StatusReportTests
    {
        Mock<Game> _gameMock;
        Mock<Galaxy> _galaxyMock;
        Mock<Enterprise> _enterpriseMock;
        IOSpy _ioSpy;
        StatusReport _testStatusReport; // object to test
        
        [SetUp]
        public void SetUp()
        {
            // Setup Mocks for game, galaxy, enterprise, and io

            _gameMock = new(
                new Mock<IReadWrite>().Object,
                new Mock<IRandom>().Object
            );
            _gameMock.SetupGet(game => game.StardatesRemaining).Returns(1);

            _galaxyMock = new(new Mock<IRandom>().Object);
            _galaxyMock.SetupGet(galaxy => galaxy.KlingonCount).Returns(1);

            _enterpriseMock = new(
                1,
                new Coordinates(1, 1),
                new Mock<IReadWrite>().Object,
                new Mock<IRandom>().Object
            );

            _ioSpy = new();

            // Object to test
            _testStatusReport = new(
                _gameMock.Object,
                _galaxyMock.Object,
                _enterpriseMock.Object,
                _ioSpy
            );
        }

        #region Constructor

        [Test]
        public void StatusReport_Constructor_Should_CorrectlySet_Description()
        {
            Assert.That(_testStatusReport.Description, Is.EqualTo("Status report"));
        }

        [Test]
        public void StatusReport_Constructor_Should_CorrectlySet_IO()
        {
            Assert.That(_testStatusReport.IO, Is.EqualTo(_ioSpy));
        }

        [Test]
        public void StatusReport_Constructor_Should_CorrectlySet_Game()
        {
            Assert.That(_testStatusReport._game, Is.EqualTo(_gameMock.Object));
        }

        [Test]
        public void StatusReport_Constructor_Should_CorrectlySet_Galaxy()
        {
            Assert.That(_testStatusReport._galaxy, Is.EqualTo(_galaxyMock.Object));
        }

        [Test]
        public void StatusReport_Constructor_Should_CorrectlySet_Enterprise()
        {
            Assert.That(_testStatusReport._enterprise, Is.EqualTo(_enterpriseMock.Object));
        }

        #endregion Constructor

        #region Execute

        [Test]
        public void Execute_StarbaseCount_GreaterThan0_ShouldPrint_CorrectLines()
        {
            _galaxyMock.SetupGet(galaxy => galaxy.StarbaseCount).Returns(1);

            // Method to test
            _testStatusReport.Execute(new Mock<IQuadrant>().Object);

            // Verify IO called correctly

            Assert.That(_ioSpy.GetOutput(), Is.EqualTo(string.Join(Environment.NewLine,
            [
                "   Status report:",
                "Klingon",
                " left:  1",
                "Mission must be completed in 1 stardates.",
                "The Federation is maintaining 1 ",
                "starbase",
                " in the galaxy.",
                ""
            ])));
        }

        [Test]
        public void Execute_StarbaseCount_GreaterThan0_Should_Execute_Correct_Command()
        {
            _galaxyMock.SetupGet(galaxy => galaxy.StarbaseCount).Returns(1);

            // Method to test
            _testStatusReport.Execute(new Mock<IQuadrant>().Object);

            // Verify enterprise called correctly
            _enterpriseMock.Verify(
                enterprise => enterprise.Execute(Command.DAM)
            );
        }

        [Test]
        public void Execute_StarbaseCount_EqualTo0_ShouldPrint_CorrectLines()
        {
            _galaxyMock.SetupGet(galaxy => galaxy.StarbaseCount).Returns(0);

            // Method to test
            _testStatusReport.Execute(new Mock<IQuadrant>().Object);

            // Verify IO called correctly

            Assert.That(_ioSpy.GetOutput(), Is.EqualTo(string.Join(Environment.NewLine,
            [
                "   Status report:",
                "Klingon",
                " left:  1",
                "Mission must be completed in 1 stardates.",
                "Your stupidity has left you on your own in",
                "  the galaxy -- you have no starbases left!",
                ""
            ])));
        }

        [Test]
        public void Execute_StarbaseCount_EqualTo0_Should_Execute_Correct_Command()
        {
            _galaxyMock.SetupGet(galaxy => galaxy.StarbaseCount).Returns(0);

            // Method to test
            _testStatusReport.Execute(new Mock<IQuadrant>().Object);

            // Verify enterprise called correctly
            _enterpriseMock.Verify(
                enterprise => enterprise.Execute(Command.DAM)
            );
        }

        [Test]
        public void Execute_StarbaseCount_LessThan0_ShouldPrint_CorrectLines()
        {
            _galaxyMock.SetupGet(galaxy => galaxy.StarbaseCount).Returns(0);

            // Method to test
            _testStatusReport.Execute(new Mock<IQuadrant>().Object);

            // Verify IO called correctly
            Assert.That(_ioSpy.GetOutput(), Is.EqualTo(string.Join(Environment.NewLine,
            [
                "   Status report:",
                "Klingon",
                " left:  1",
                "Mission must be completed in 1 stardates.",
                "Your stupidity has left you on your own in",
                "  the galaxy -- you have no starbases left!",
                ""
            ])));
        }

        [Test]
        public void Execute_StarbaseCount_LessThan0_Should_Execute_Correct_Commands()
        {
            _galaxyMock.SetupGet(galaxy => galaxy.StarbaseCount).Returns(0);

            // Method to test
            _testStatusReport.Execute(new Mock<IQuadrant>().Object);

            // Verify enterprise called correctly
            _enterpriseMock.Verify(
                enterprise => enterprise.Execute(Command.DAM)
            );
        }

        #endregion Execute
    }
}
