using Games.Common.IO;
using Games.Common.Randomness;
using Moq;
using SuperStarTrek.Space;
using SuperStarTrek.Systems.ComputerFunctions;

namespace SuperStarTrek.Test.Systems.ComputerFunctions
{
    // implementation of GalacticReport to allow testing of it
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
        IOSpy _ioSpy;
        Mock<Galaxy> _galaxyMock;
        Mock<IQuadrant> _quadrantMock;
        GalacticReportTestable _testGalacticReport;

        [SetUp]
        public void SetUp()
        {
            _ioSpy = new();
            _galaxyMock = new(new Mock<IRandom>().Object);

            _quadrantMock = new();

            _testGalacticReport = new(
                "",
                _ioSpy,
                _galaxyMock.Object,
                []
            );
        }

        #region Galaxy

        [Test]
        public void GetGalaxy_Should_Return_Correct_Object()
        {
            Assert.That(_testGalacticReport.Galaxy, Is.SameAs(_galaxyMock.Object));
        }

        #endregion Galaxy

        #region Execute

        [Test]
        public void Execute_Valid_RowData_Should_Call_WriteHeader()
        {
            _testGalacticReport.RowDataToReturn = ["A", "B", "C"];

            _testGalacticReport.Execute(_quadrantMock.Object);

            Assert.That(_testGalacticReport.WriteHeaderCalled);
        }

        [Test]
        public void Execute_Valid_RowData_Should_Send_CorrectQuadrant_To_WriteHeader()
        {
            _testGalacticReport.RowDataToReturn = ["A", "B", "C"];

            _testGalacticReport.Execute(_quadrantMock.Object);

            Assert.That(_testGalacticReport.CapturedQuadrant, Is.SameAs(_quadrantMock.Object));
        }

        [Test]
        public void Execute_Valid_RowData_Should_Write_Correct_Lines()
        {
            _testGalacticReport.RowDataToReturn = ["A", "B", "C"];

            _testGalacticReport.Execute(_quadrantMock.Object);

            // verify IO calls
            Assert.That(_ioSpy.GetOutput(), Is.EqualTo(string.Join(Environment.NewLine,
            [
                "       1     2     3     4     5     6     7     8",
                "     ----- ----- ----- ----- ----- ----- ----- -----",
                " 1   A",
                "     ----- ----- ----- ----- ----- ----- ----- -----",
                " 2   B",
                "     ----- ----- ----- ----- ----- ----- ----- -----",
                " 3   C",
                "     ----- ----- ----- ----- ----- ----- ----- -----",
                ""
            ])));
        }

        [Test]
        public void Execute_Empty_RowData_Should_Call_WriteHeader()
        {
            _testGalacticReport.Execute(_quadrantMock.Object);

            Assert.That(_testGalacticReport.WriteHeaderCalled);
        }

        [Test]
        public void Execute_Empty_RowData_Should_Send_CorrectQuadrant_To_WriteHeader()
        {
            _testGalacticReport.Execute(_quadrantMock.Object);

            Assert.That(_testGalacticReport.CapturedQuadrant, Is.SameAs(_quadrantMock.Object));
        }

        [Test]
        public void Execute_Empty_RowData_Should_Write_Correct_Lines()
        {
            _testGalacticReport.Execute(_quadrantMock.Object);

            // verify IO calls
            Assert.That(_ioSpy.GetOutput(), Is.EqualTo(string.Join(Environment.NewLine,
            [
                "       1     2     3     4     5     6     7     8",
                "     ----- ----- ----- ----- ----- ----- ----- -----",
                ""
            ])));
        }

        #endregion Execute
    }
}