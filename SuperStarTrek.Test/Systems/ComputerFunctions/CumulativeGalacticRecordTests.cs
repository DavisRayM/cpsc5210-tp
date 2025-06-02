using SuperStarTrek.Space;
using SuperStarTrek.Systems.ComputerFunctions;
using Games.Common.IO;
using Games.Common.Randomness;
using Moq;

namespace SuperStarTrek.Test.Systems.ComputerFunctions
{
    internal class TestableCumulativeGalacticRecord : CumulativeGalacticRecord
    {
        public TestableCumulativeGalacticRecord(IReadWrite io, Galaxy galaxy)
            : base(io, galaxy)
        {
        }

        public void CallWriteHeader(IQuadrant quadrant) => WriteHeader(quadrant);

        public IEnumerable<string> CallGetRowData() => GetRowData();
    }

    public class CumulativeGalacticRecordTests
    {
        [Test]
        public void System_Description_IsExpected()
        {
            Mock<IReadWrite> mockIO = new();
            Mock<Galaxy> mockGalaxy = new(new Mock<IRandom>().Object);
            Mock<IQuadrant> mockQuadrant = new();
            Coordinates expectedCoords = new(1, 5);

            TestableCumulativeGalacticRecord record = new(mockIO.Object, mockGalaxy.Object);
            Assert.That(record.Description, Is.EqualTo("Cumulative galactic record"));
        }

        [Test]
        public void WriteHeader_Record_WritesToIO()
        {
            Mock<IReadWrite> mockIO = new();
            Mock<Galaxy> mockGalaxy = new(new Mock<IRandom>().Object);
            Mock<IQuadrant> mockQuadrant = new();
            Coordinates expectedCoords = new(1, 5);

            TestableCumulativeGalacticRecord record = new(mockIO.Object, mockGalaxy.Object);
            mockQuadrant.Setup(q => q.Coordinates)
                .Returns(expectedCoords);

            record.CallWriteHeader(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine($"Computer record of galaxy for quadrant {expectedCoords}"), Times.Once());
        }

        [Test]
        public void WriteHeader_Record_PrintsSpace()
        {
            Mock<IReadWrite> mockIO = new();
            Mock<Galaxy> mockGalaxy = new(new Mock<IRandom>().Object);
            Mock<IQuadrant> mockQuadrant = new();
            Coordinates expectedCoords = new(1, 5);

            TestableCumulativeGalacticRecord record = new(mockIO.Object, mockGalaxy.Object);
            mockQuadrant.Setup(q => q.Coordinates)
                .Returns(expectedCoords);

            record.CallWriteHeader(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine(""), Times.Exactly(2));
        }

        [Test]
        public void GetRowData_Record_CapturesQuadrantInfo()
        {
            List<string> expected = [
                " ***   ***",
                " ***"
            ];
            QuadrantInfo[][] infos = new[]
            {
                new[]
                {
                    new QuadrantInfo(new Coordinates(0, 0), "test 1", 0, 0, false),
                    new QuadrantInfo(new Coordinates(0, 1), "test 2", 0, 0, false),
                },
                new[]
                {
                    new QuadrantInfo(new Coordinates(1, 0), "test 3", 0, 0, false),
                },
            };

            Mock<IReadWrite> mockIO = new();
            Mock<Galaxy> mockGalaxy = new(new Mock<IRandom>().Object);
            Mock<IQuadrant> mockQuadrant = new();

            TestableCumulativeGalacticRecord record = new(mockIO.Object, mockGalaxy.Object);
            mockGalaxy.Setup(q => q.Quadrants).Returns(infos);

            Assert.That(record.CallGetRowData(), Is.EqualTo(expected));
        }
    }
}
