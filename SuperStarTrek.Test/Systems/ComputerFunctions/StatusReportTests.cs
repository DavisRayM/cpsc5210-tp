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
        [Test]
        public void Execute_StarbaseCount_GreaterThan0_ShouldPrint_StarbaseCount()
        {
            // Setup Mocks for game, galaxy, enterprise, and io
            
            Mock<Game> gameMock = new(
                new Mock<IReadWrite>().Object,
                new Mock<IRandom>().Object
            );
            gameMock.SetupGet(game => game.StardatesRemaining).Returns(1);
            
            Mock<Galaxy> galaxyMock = new(new Mock<IRandom>().Object);
            galaxyMock.SetupGet(galaxy => galaxy.KlingonCount).Returns(1);
            galaxyMock.SetupGet(galaxy => galaxy.StarbaseCount).Returns(1);

            Mock<Enterprise> enterpriseMock = new(
                1,
                new Coordinates(1, 1),
                new Mock<IReadWrite>().Object,
                new Mock<IRandom>().Object
            );

            Mock<IReadWrite> ioMock = new();

            // Object to test
            StatusReport testStatusReport = new(
                gameMock.Object,
                galaxyMock.Object,
                enterpriseMock.Object,
                ioMock.Object
            );

            // Method to test
            testStatusReport.Execute(new Mock<IQuadrant>().Object);

            // Verify IO called correctly

            ioMock.Verify(
                io => io.WriteLine(
                    "   Status report:"
                ),
                Times.Once
            );
            ioMock.Verify(
                io => io.Write(
                    "Klingon".Pluralize(galaxyMock.Object.KlingonCount)
                ),
                Times.Once
            );
            ioMock.Verify(
                io => io.WriteLine(
                    $" left:  {galaxyMock.Object.KlingonCount}"
                ),
                Times.Once
            );
            ioMock.Verify(
                io => io.WriteLine(
                    $"Mission must be completed in {gameMock.Object.StardatesRemaining:0.#} stardates."
                ), 
                Times.Once
            );
            ioMock.Verify(
                io => io.Write(
                    $"The Federation is maintaining {galaxyMock.Object.StarbaseCount} "
                ), 
                Times.Once
            );
            ioMock.Verify(
                io => io.Write(
                    "starbase".Pluralize(galaxyMock.Object.StarbaseCount)
                ), 
                Times.Once
            );
            ioMock.Verify(
                io => io.WriteLine(
                    " in the galaxy."
                ), 
                Times.Once
            );

            // Verify enterprise called correctly
            enterpriseMock.Verify(
                enterprise => enterprise.Execute(Command.DAM)
            );
        }

        [Test]
        public void Execute_StarbaseCount_EqualTo0_ShouldPrint_NoStarbases()
        {
            // Setup Mocks for game, galaxy, enterprise, and io
            
            Mock<Game> gameMock = new(
                new Mock<IReadWrite>().Object,
                new Mock<IRandom>().Object
            );
            gameMock.SetupGet(game => game.StardatesRemaining).Returns(1);
            
            Mock<Galaxy> galaxyMock = new(new Mock<IRandom>().Object);
            galaxyMock.SetupGet(galaxy => galaxy.KlingonCount).Returns(1);
            galaxyMock.SetupGet(galaxy => galaxy.StarbaseCount).Returns(0);

            Mock<Enterprise> enterpriseMock = new(
                1,
                new Coordinates(1, 1),
                new Mock<IReadWrite>().Object,
                new Mock<IRandom>().Object
            );

            Mock<IReadWrite> ioMock = new();

            // Object to test
            StatusReport testStatusReport = new(
                gameMock.Object,
                galaxyMock.Object,
                enterpriseMock.Object,
                ioMock.Object
            );

            // Method to test
            testStatusReport.Execute(new Mock<IQuadrant>().Object);

            // Verify IO called correctly

            ioMock.Verify(
                io => io.WriteLine(
                    "   Status report:"
                ),
                Times.Once
            );
            ioMock.Verify(
                io => io.Write(
                    "Klingon".Pluralize(galaxyMock.Object.KlingonCount)
                ),
                Times.Once
            );
            ioMock.Verify(
                io => io.WriteLine(
                    $" left:  {galaxyMock.Object.KlingonCount}"
                ),
                Times.Once
            );
            ioMock.Verify(
                io => io.WriteLine(
                    $"Mission must be completed in {gameMock.Object.StardatesRemaining:0.#} stardates."
                ), 
                Times.Once
            );
            ioMock.Verify(
                io => io.WriteLine(
                    "Your stupidity has left you on your own in"
                ), 
                Times.Once
            );
            ioMock.Verify(
                io => io.WriteLine(
                    "  the galaxy -- you have no starbases left!"
                ), 
                Times.Once
            );

            // Verify enterprise called correctly
            enterpriseMock.Verify(
                enterprise => enterprise.Execute(Command.DAM)
            );
        }

        [Test]
        public void Execute_StarbaseCount_LessThan0_ShouldPrint_NoStarbases()
        {
            // Setup Mocks for game, galaxy, enterprise, and io
            
            Mock<Game> gameMock = new(
                new Mock<IReadWrite>().Object,
                new Mock<IRandom>().Object
            );
            gameMock.SetupGet(game => game.StardatesRemaining).Returns(1);
            
            Mock<Galaxy> galaxyMock = new(new Mock<IRandom>().Object);
            galaxyMock.SetupGet(galaxy => galaxy.KlingonCount).Returns(1);
            galaxyMock.SetupGet(galaxy => galaxy.StarbaseCount).Returns(0);

            Mock<Enterprise> enterpriseMock = new(
                1,
                new Coordinates(1, 1),
                new Mock<IReadWrite>().Object,
                new Mock<IRandom>().Object
            );

            Mock<IReadWrite> ioMock = new();

            // Object to test
            StatusReport testStatusReport = new(
                gameMock.Object,
                galaxyMock.Object,
                enterpriseMock.Object,
                ioMock.Object
            );

            // Method to test
            testStatusReport.Execute(new Mock<IQuadrant>().Object);

            // Verify IO called correctly

            ioMock.Verify(
                io => io.WriteLine(
                    "   Status report:"
                ),
                Times.Once
            );
            ioMock.Verify(
                io => io.Write(
                    "Klingon".Pluralize(galaxyMock.Object.KlingonCount)
                ),
                Times.Once
            );
            ioMock.Verify(
                io => io.WriteLine(
                    $" left:  {galaxyMock.Object.KlingonCount}"
                ),
                Times.Once
            );
            ioMock.Verify(
                io => io.WriteLine(
                    $"Mission must be completed in {gameMock.Object.StardatesRemaining:0.#} stardates."
                ), 
                Times.Once
            );
            ioMock.Verify(
                io => io.WriteLine(
                    "Your stupidity has left you on your own in"
                ), 
                Times.Once
            );
            ioMock.Verify(
                io => io.WriteLine(
                    "  the galaxy -- you have no starbases left!"
                ), 
                Times.Once
            );

            // Verify enterprise called correctly
            enterpriseMock.Verify(
                enterprise => enterprise.Execute(Command.DAM)
            );
        }
    }
}
