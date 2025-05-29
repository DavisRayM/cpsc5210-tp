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
        Mock<IReadWrite> _ioMock;
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

            _ioMock = new();

            // Object to test
            _testStatusReport = new(
                _gameMock.Object,
                _galaxyMock.Object,
                _enterpriseMock.Object,
                _ioMock.Object
            );
        }

        [Test]
        public void Execute_StarbaseCount_GreaterThan0_ShouldPrint_CorrectLines()
        {
            _galaxyMock.SetupGet(galaxy => galaxy.StarbaseCount).Returns(1);

            // Method to test
            _testStatusReport.Execute(new Mock<IQuadrant>().Object);

            // Verify IO called correctly

            _ioMock.Verify(
                io => io.WriteLine(
                    "   Status report:"
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.Write(
                    "Klingon".Pluralize(_galaxyMock.Object.KlingonCount)
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.WriteLine(
                    $" left:  {_galaxyMock.Object.KlingonCount}"
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.WriteLine(
                    $"Mission must be completed in {_gameMock.Object.StardatesRemaining:0.#} stardates."
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.Write(
                    $"The Federation is maintaining {_galaxyMock.Object.StarbaseCount} "
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.Write(
                    "starbase".Pluralize(_galaxyMock.Object.StarbaseCount)
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.WriteLine(
                    " in the galaxy."
                ),
                Times.Once
            );
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

            _ioMock.Verify(
                io => io.WriteLine(
                    "   Status report:"
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.Write(
                    "Klingon".Pluralize(_galaxyMock.Object.KlingonCount)
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.WriteLine(
                    $" left:  {_galaxyMock.Object.KlingonCount}"
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.WriteLine(
                    $"Mission must be completed in {_gameMock.Object.StardatesRemaining:0.#} stardates."
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.WriteLine(
                    "Your stupidity has left you on your own in"
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.WriteLine(
                    "  the galaxy -- you have no starbases left!"
                ),
                Times.Once
            );
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

            _ioMock.Verify(
                io => io.WriteLine(
                    "   Status report:"
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.Write(
                    "Klingon".Pluralize(_galaxyMock.Object.KlingonCount)
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.WriteLine(
                    $" left:  {_galaxyMock.Object.KlingonCount}"
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.WriteLine(
                    $"Mission must be completed in {_gameMock.Object.StardatesRemaining:0.#} stardates."
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.WriteLine(
                    "Your stupidity has left you on your own in"
                ),
                Times.Once
            );
            _ioMock.Verify(
                io => io.WriteLine(
                    "  the galaxy -- you have no starbases left!"
                ),
                Times.Once
            );
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
    }
}
