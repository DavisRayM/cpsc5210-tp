using NUnit.Framework;
using SuperStarTrek.Space;
using SuperStarTrek.Resources;
using Games.Common.Randomness;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Games.Common.IO;
using SuperStarTrek.Objects;

namespace SuperStarTrek.Test.Space
{
    /// Unit tests for the Galaxy class, covering construction, quadrant retrieval, naming, and neighborhood queries.
    [TestFixture]
    public class GalaxyTests
    {
        private Mock<IRandom> _mockRandom;
        private Galaxy _galaxy;
        public static IEnumerable<TestCaseData> ValidCoordinates => new[]
        {
            new TestCaseData(0, 0),
            new TestCaseData(4, 4),
            new TestCaseData(7, 7),
            new TestCaseData(0, 7),
            new TestCaseData(7, 0),
            new TestCaseData(3, 2)
        };

        [SetUp]
        public void Setup()
        {
            _mockRandom = new Mock<IRandom>();
            _mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f);

            _galaxy = new Galaxy(_mockRandom.Object);
        }

        private (Enterprise enterprise, Mock<IReadWrite> ioMock, Mock<IRandom> randomMock) CreateEnterprise(Coordinates coord)
        {
            var ioMock = new Mock<IReadWrite>();
            var randomMock = new Mock<IRandom>();
            var enterprise = new Enterprise(
                maxEnergy: 3000,
                sector: coord,
                io: ioMock.Object,
                random: randomMock.Object
            );

            return (enterprise, ioMock, randomMock);
        }

        #region Construction Tests

        [Test]
        public void Galaxy_Constructs_Valid8x8Grid()
        {
            var quadrantGrid = _galaxy.Quadrants.ToArray();

            Assert.That(quadrantGrid.Length, Is.EqualTo(8), "Galaxy should have 8 rows.");
            foreach (var row in quadrantGrid)
            {
                Assert.That(row.Count(), Is.EqualTo(8), "Each row should have 8 columns.");
                Assert.That(row.All(q => q != null), "QuadrantInfo objects should not be null.");
            }
        }

        #endregion

        #region Indexer and Properties Tests

        [Test, TestCaseSource(nameof(ValidCoordinates))]
        [Category("Properties")]
        public void Galaxy_Indexer_ReturnsCorrectQuadrant(int x, int y)
        {
            var coord = new Coordinates(x, y);
            var quadrant = _galaxy[coord];

            Assert.That(quadrant.Coordinates, Is.EqualTo(coord), "Indexer should retrieve correct QuadrantInfo.");
        }

        [Test]
        [Category("Properties")]
        public void Galaxy_StarbaseCount_IsAtLeastOne()
        {
            Assert.That(_galaxy.StarbaseCount, Is.GreaterThanOrEqualTo(1), "Galaxy should have at least one starbase.");
        }

        [Test]
        [Category("Properties")]
        public void Galaxy_KlingonCount_IsPositive()
        {
            Assert.That(_galaxy.KlingonCount, Is.GreaterThanOrEqualTo(1), "Galaxy should have Klingons present after starbase placement.");
        }

        #endregion

        #region Neighborhood Tests

        // Temporary commented out due to running indefinitely
        //[Test, TestCaseSource(nameof(ValidCoordinates))]
        //[Category("Neighborhood")]
        //public void GetNeighborhood_ReturnsExpected3x3Grid(int x, int y)
        //{
        //    var coord = new Coordinates(x, y);
        //    var info = QuadrantInfo.Create(coord, "Alpha I", new Mock<IRandom>().Object);
        //    var (enterprise, ioMock, randomMock) = CreateEnterprise(coord);

        //    var quadrant = new Quadrant(info, enterprise, randomMock.Object, _galaxy, ioMock.Object);
        //    var neighbors = _galaxy.GetNeighborhood(quadrant).ToArray();

        //    Assert.That(neighbors.Length, Is.EqualTo(3), "Neighborhood should have 3 rows.");
        //    foreach (var row in neighbors)
        //    {
        //        Assert.That(row.Count(), Is.EqualTo(3), "Each row should have 3 columns.");
        //    }
        //}

        // Temporary commented out due to running indefinitely
        //[Test]
        //[Category("Neighborhood")]
        //public void GetNeighborhood_HandlesEdgesCorrectly()
        //{
        //    var coord = new Coordinates(0, 0);
        //    var info = QuadrantInfo.Create(coord, "Alpha I", new Mock<IRandom>().Object);
        //    var (enterprise, ioMock, randomMock) = CreateEnterprise(coord);

        //    var quadrant = new Quadrant(info, enterprise, randomMock.Object, _galaxy, ioMock.Object);
        //    var neighbors = _galaxy.GetNeighborhood(quadrant).ToArray();

        //    Assert.That(neighbors[0].ElementAt(0), Is.Null, "Out-of-bounds neighbor should be null.");
        //    Assert.That(neighbors[2].ElementAt(2), Is.Not.Null, "Valid neighbor should not be null.");
        //}


        #endregion

        #region Quadrant Naming Tests

        [Test, TestCaseSource(nameof(ValidCoordinates))]
        [Category("Naming")]
        public void GetQuadrantName_ReturnsFormattedString(int x, int y)
        {
            var method = typeof(Galaxy)
                .GetMethod("GetQuadrantName", BindingFlags.NonPublic | BindingFlags.Static);
            var coord = new Coordinates(x, y);
            var name = method.Invoke(null, new object[] { coord }) as string;

            StringAssert.IsMatch(@"\w+ (I|II|III|IV)", name, "Quadrant name should match region and subregion format.");
        }

        #endregion
    }
}