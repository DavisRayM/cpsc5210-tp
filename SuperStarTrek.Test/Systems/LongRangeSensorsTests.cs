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
        Mock<IReadWrite> _ioMock;
        Mock<Galaxy> _galaxyMock;
        Mock<IQuadrant> _quadrantMock;
        LongRangeSensors _testLongRangeSensors;

        [SetUp]
        public void SetUp()
        {
            _ioMock = new();

            _galaxyMock = new(new Mock<IRandom>().Object);

            _quadrantMock = new();
            _quadrantMock
                .Setup(quadrant => quadrant.Coordinates)
                .Returns(new Coordinates(0, 0));

            _testLongRangeSensors = new(
                _galaxyMock.Object,
                _ioMock.Object
            );
        }

        #region CanExecuteCommand

        [Test]
        public void CanExecuteCommand_IsDamaged_True_Should_ReturnFalse()
        {
            _testLongRangeSensors.TakeDamage(0.0000000001f);

            Assert.That(_testLongRangeSensors.CanExecuteCommand(), Is.EqualTo(false));
        }

        [Test]
        public void CanExecuteCommand_IsDamaged_True_Should_WriteToIO()
        {
            _testLongRangeSensors.TakeDamage(0.0000000001f);

            _testLongRangeSensors.CanExecuteCommand();

            _ioMock.Verify(io => io.WriteLine("Long Range Sensors are inoperable"), Times.Once);
        }

        [Test]
        public void CanExecuteCommand_IsDamaged_False_Should_ReturnFalse()
        {
            Assert.That(_testLongRangeSensors.CanExecuteCommand(), Is.EqualTo(true));
        }

        [Test]
        public void CanExecuteCommand_IsDamaged_False_Should_Not_WriteToIO()
        {
            _testLongRangeSensors.CanExecuteCommand();

            _ioMock.Verify(io => io.WriteLine("Long Range Sensors are inoperable"), Times.Never);
        }

        #endregion CanExecuteCommand

        #region ExecuteCommandCore

        [Test]
        public void ExecuteCommandCore_Should_Return_OK()
        {
            List<IEnumerable<QuadrantInfo?>>  neighbors =
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
            _galaxyMock
                .Setup(galaxy => galaxy.GetNeighborhood(It.IsAny<IQuadrant>()))
                .Returns(neighbors);

            Assert.That(_testLongRangeSensors.ExecuteCommandCore(_quadrantMock.Object), Is.EqualTo(CommandResult.Ok));
        }

        [Test]
        public void ExecuteCommandCore_Should_PrintAllScans()
        {
            List<IEnumerable<QuadrantInfo?>>  neighbors =
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
            _galaxyMock
                .Setup(galaxy => galaxy.GetNeighborhood(It.IsAny<IQuadrant>()))
                .Returns(neighbors);

            _testLongRangeSensors.ExecuteCommandCore(_quadrantMock.Object);

            _ioMock.Verify(io => io.WriteLine("Long range scan for quadrant 1 , 1"), Times.Once);
            _ioMock.Verify(io => io.WriteLine("-------------------"), Times.Exactly(4));
            _ioMock.Verify(io => io.WriteLine(": *** : 101 : 212 :"), Times.Once);
            _ioMock.Verify(io => io.WriteLine(": 313 : 404 : *** :"), Times.Once);
            _ioMock.Verify(io => io.WriteLine(": 616 : *** : *** :"), Times.Once);
        }

        [Test]
        public void ExecuteCommandCore_NoNeighbors_Should_Return_OK()
        {
            List<IEnumerable<QuadrantInfo?>> neighbors = [];
            _galaxyMock
                .Setup(galaxy => galaxy.GetNeighborhood(It.IsAny<IQuadrant>()))
                .Returns(neighbors);

            Assert.That(_testLongRangeSensors.ExecuteCommandCore(_quadrantMock.Object), Is.EqualTo(CommandResult.Ok));
        }

        [Test]
        public void ExecuteCommandCore_NoNeighbors_Should_PrintTwoLines()
        {
            List<IEnumerable<QuadrantInfo?>> neighbors = [];
            _galaxyMock
                .Setup(galaxy => galaxy.GetNeighborhood(It.IsAny<IQuadrant>()))
                .Returns(neighbors);

            _testLongRangeSensors.ExecuteCommandCore(_quadrantMock.Object);

            _ioMock.Verify(io => io.WriteLine("Long range scan for quadrant 1 , 1"), Times.Once);
            _ioMock.Verify(io => io.WriteLine("-------------------"), Times.Once);
        }

        #endregion ExecuteCommandCore
    }
}
