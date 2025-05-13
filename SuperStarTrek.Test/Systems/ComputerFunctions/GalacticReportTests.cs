using Games.Common.IO;
using Games.Common.Randomness;
using Moq;
using SuperStarTrek.Space;
using SuperStarTrek.Systems.ComputerFunctions;

namespace SuperStarTrek.Test.Systems.ComputerFunctions
{
    internal class GalacticReportTestable(
        string description,
        IReadWrite io,
        Galaxy galaxy,
        List<string> rowData
    ) : GalacticReport(description, io, galaxy)
    {
        public IQuadrant? CapturedQuadrant;
        public bool WriteHeaderCalled = false;
        public List<string> RowDataToReturn = rowData;

        protected override void WriteHeader(IQuadrant quadrant)
        {
            WriteHeaderCalled = true;
            CapturedQuadrant = quadrant;
        }
        protected override IEnumerable<string> GetRowData()
        {
            return RowDataToReturn;
        }
    }

    public class GalacticReportTests
    {
        #region Galaxy

        [Test]
        public void GetGalaxy_Should_Return_Correct_Object()
        {
            Mock<Galaxy> galaxyMock = new(new Mock<IRandom>().Object);

            GalacticReportTestable testGalacticReport = new(
                "",
                new Mock<IReadWrite>().Object,
                galaxyMock.Object,
                []
            );

            Assert.That(testGalacticReport.Galaxy, Is.SameAs(galaxyMock.Object));
        }

        #endregion Galaxy

        #region Execute

        [Test]
        public void Execute_Valid_RowData_Should_Write_Correct_Lines()
        {
            Mock<IReadWrite> ioMock = new();

            GalacticReportTestable testGalacticReport = new(
                "",
                ioMock.Object,
                new Mock<Galaxy>(new Mock<IRandom>().Object).Object,
                ["A", "B", "C"]
            );

            Mock<IQuadrant> quadrantMock = new();

            testGalacticReport.Execute(quadrantMock.Object);

            Assert.Multiple(() =>
            {
                // verify WriteHeader was called and with correct quadrant

                Assert.That(testGalacticReport.WriteHeaderCalled);
                Assert.That(testGalacticReport.CapturedQuadrant, Is.SameAs(quadrantMock.Object));
            });

            // verify IO calls

            ioMock.Verify(
                io => io.WriteLine(
                    "       1     2     3     4     5     6     7     8"
                ),
                Times.Once
            );

            ioMock.Verify(
                io => io.WriteLine(
                    "     ----- ----- ----- ----- ----- ----- ----- -----"
                ),
                Times.Exactly(4)
            );

            ioMock.Verify(
                io => io.WriteLine(
                    " 1   A"
                ),
                Times.Once
            );

            ioMock.Verify(
                io => io.WriteLine(
                    " 2   B"
                ),
                Times.Once
            );

            ioMock.Verify(
                io => io.WriteLine(
                    " 3   C"
                ),
                Times.Once
            );
        }

        [Test]
        public void Execute_Empty_RowData_Should_Write_Correct_Lines()
        {
            Mock<IReadWrite> ioMock = new();

            GalacticReportTestable testGalacticReport = new(
                "",
                ioMock.Object,
                new Mock<Galaxy>(new Mock<IRandom>().Object).Object,
                []
            );

            Mock<IQuadrant> quadrantMock = new();

            testGalacticReport.Execute(quadrantMock.Object);

            Assert.Multiple(() =>
            {
                // verify WriteHeader was called and with correct quadrant

                Assert.That(testGalacticReport.WriteHeaderCalled);
                Assert.That(testGalacticReport.CapturedQuadrant, Is.SameAs(quadrantMock.Object));
            });

            // verify IO calls

            ioMock.Verify(
                io => io.WriteLine(
                    "       1     2     3     4     5     6     7     8"
                ),
                Times.Once
            );

            ioMock.Verify(
                io => io.WriteLine(
                    "     ----- ----- ----- ----- ----- ----- ----- -----"
                ),
                Times.Once
            );
        }

        #endregion Execute
    }
}