using Games.Common.IO;
using Games.Common.Randomness;
using Moq;
using SuperStarTrek.Commands;
using SuperStarTrek.Space;
using SuperStarTrek.Systems;

namespace SuperStarTrek.Test.Systems
{
    public class LongRangeSensorsTests
    {
        #region CanExecuteCommand

        [Test]
        public void CanExecuteCommand_IsDamaged_True_Should_WriteToIO_And_ReturnFalse()
        {
            Mock<IReadWrite> ioMock = new();

            LongRangeSensors testLongRangeSensors = new(
                new Mock<Galaxy>(new Mock<IRandom>().Object).Object,
                ioMock.Object
            );
            
            testLongRangeSensors.TakeDamage(0.0000000001f);

            Assert.That(testLongRangeSensors.CanExecuteCommand(), Is.EqualTo(false));

            ioMock.Verify(io => io.WriteLine("Long Range Sensors are inoperable"), Times.Once);
        }

        [Test]
        public void CanExecuteCommand_IsDamaged_False_Should_ReturnFalse()
        {
            LongRangeSensors testLongRangeSensors = new(
                new Mock<Galaxy>(new Mock<IRandom>().Object).Object,
                new Mock<IReadWrite>().Object
            );
            
            Assert.That(testLongRangeSensors.CanExecuteCommand(), Is.EqualTo(true));
        }

        #endregion CanExecuteCommand

        #region ExecuteCommandCore

        [Test]
        public void ExecuteCommandCore_Should_PrintAllScans()
        {
            Mock<Galaxy> galaxyMock = new(new Mock<IRandom>().Object);
            List<IEnumerable<QuadrantInfo?>> neighbors =
            [
                [ 
                    null, 
                    new QuadrantInfo(new Coordinates(1, 1), "Quadrant1", 1, 1, false), 
                    new QuadrantInfo(new Coordinates(2, 2), "Quadrant2", 2, 2, true), 
                ],
                [
                    new QuadrantInfo(new Coordinates(3, 3), "Quadrant3", 3, 3, true),
                    new QuadrantInfo(new Coordinates(4, 4), "Quadrant4", 4, 4, false),
                    null
                ],
                [
                    new QuadrantInfo(new Coordinates(6, 6), "Quadrant6", 6, 6, true),
                    null, 
                    null 
                ],
            ];
            galaxyMock
                .Setup(galaxy => galaxy.GetNeighborhood(It.IsAny<IQuadrant>()))
                .Returns(neighbors);

            Mock<IReadWrite> ioMock = new();

            LongRangeSensors testLongRangeSensors = new(
                galaxyMock.Object,
                ioMock.Object
            );

            Mock<IQuadrant> quadrantMock = new();
            quadrantMock
                .Setup(quadrant => quadrant.Coordinates)
                .Returns(new Coordinates(0, 0));

            Assert.That(testLongRangeSensors.ExecuteCommandCore(quadrantMock.Object), Is.EqualTo(CommandResult.Ok));

            ioMock.Verify(io => io.WriteLine("Long range scan for quadrant 1 , 1"), Times.Once);
            ioMock.Verify(io => io.WriteLine("-------------------"), Times.Exactly(4));
            ioMock.Verify(io => io.WriteLine(": *** : 101 : 212 :"), Times.Once);
            ioMock.Verify(io => io.WriteLine(": 313 : 404 : *** :"), Times.Once);
            ioMock.Verify(io => io.WriteLine(": 616 : *** : *** :"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_NoNeighbors_Should_PrintTwoLines()
        {
            Mock<Galaxy> galaxyMock = new(new Mock<IRandom>().Object);
            List<IEnumerable<QuadrantInfo?>> neighbors = [];
            galaxyMock
                .Setup(galaxy => galaxy.GetNeighborhood(It.IsAny<IQuadrant>()))
                .Returns(neighbors);

            Mock<IReadWrite> ioMock = new();

            LongRangeSensors testLongRangeSensors = new(
                galaxyMock.Object,
                ioMock.Object
            );

            Mock<IQuadrant> quadrantMock = new();
            quadrantMock
                .Setup(quadrant => quadrant.Coordinates)
                .Returns(new Coordinates(0, 0));

            Assert.That(testLongRangeSensors.ExecuteCommandCore(quadrantMock.Object), Is.EqualTo(CommandResult.Ok));

            ioMock.Verify(io => io.WriteLine("Long range scan for quadrant 1 , 1"), Times.Once);
            ioMock.Verify(io => io.WriteLine("-------------------"), Times.Once);
        }

        #endregion ExecuteCommandCore
    }
}
