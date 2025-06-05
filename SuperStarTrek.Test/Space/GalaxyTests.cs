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
        public void Galaxy_StarbaseCount_IsExpectedNumber()
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0.98f) // Adds 1 starbase in the first QuadrantInfo
                .Returns(0f)
                .Returns(0f)
                .Returns(0.98f) // Adds 1 starbase in the first QuadrantInfo
                .Returns(0f);
            var galaxy = new Galaxy(mockRandom.Object);

            Assert.That(galaxy.StarbaseCount, Is.EqualTo(2), "Galaxy should have at least one starbase.");
        }

        [Test]
        [Category("Properties")]
        public void Galaxy_KlingonCount_IsExpectedNumber()
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0.99f) // Creates 3 Klingons in the first QuadrantInfo
                .Returns(0f)
                .Returns(0f)
                .Returns(0.98f) // Adds 2 Klingons in the second QuadrantInfo
                .Returns(0f)
                .Returns(0f);

            var galaxy = new Galaxy(mockRandom.Object);

            Assert.That(galaxy.KlingonCount, Is.EqualTo(5), "Galaxy should have Klingons present after starbase placement.");
        }


        [Test]
        [Category("Properties")]
        public void Galaxy_CreatesStarbase_IfNone()
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);

            var galaxy = new Galaxy(mockRandom.Object);

            Assert.That(galaxy.StarbaseCount, Is.EqualTo(1), "Galaxy should have Klingons present after starbase placement.");
        }


        [Test]
        [TestCase(0.96f, ExpectedResult = 2, Reason = "Has 2 Klingons none should be added")]
        [TestCase(0.81f, ExpectedResult = 2, Reason = "Adds 1 Klingon Should have 2 total at the end")]
        [TestCase(0f, ExpectedResult = 1, Reason = "Adds 0 Klingon Should have 1 total at the end")]
        [Category("Properties")]
        public int Galaxy_AddsKlingon_WhenLessThanTwo(float klingonRandom)
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(klingonRandom)
                .Returns(0f);

            var galaxy = new Galaxy(mockRandom.Object);

            return galaxy.KlingonCount;
        }

        #endregion

        #region Neighborhood Tests

        [Test, TestCaseSource(nameof(ValidCoordinates))]
        [Category("Neighborhood")]
        public void GetNeighborhood_ReturnsExpected3RowGrid(int x, int y)
        {
            var coord = new Coordinates(x, y);

            Mock<IQuadrant> mockQuadrant = new();
            mockQuadrant.Setup(q => q.Coordinates).Returns(coord);

            var neighbors = _galaxy.GetNeighborhood(mockQuadrant.Object).ToArray();

            Assert.That(neighbors.Length, Is.EqualTo(3), "Neighborhood should have 3 rows.");
        }

        private QuadrantInfo? CheckBoundsNeighbor(List<List<QuadrantInfo>> quadrants, int x, int y)
        {
            return (x < 0 || x > 7 || y < 0 || y > 7) ? null : quadrants[x][y];
        }

        [Test, TestCaseSource(nameof(ValidCoordinates))]
        [Category("Neighborhood")]
        public void GetNeighborhood_ReturnsExpected3Rows(int x, int y)
        {
            var coord = new Coordinates(x, y);
            var quadrants = _galaxy.Quadrants.Select(row => row.ToList()).ToList();

            var expected = new List<List<QuadrantInfo?>>
            {
                new()
                {
                    CheckBoundsNeighbor(quadrants, coord.X - 1, coord.Y - 1),
                    CheckBoundsNeighbor(quadrants, coord.X - 1, coord.Y),
                    CheckBoundsNeighbor(quadrants, coord.X - 1, coord.Y + 1)
                },
                new()
                {
                    CheckBoundsNeighbor(quadrants, coord.X, coord.Y - 1),
                    CheckBoundsNeighbor(quadrants, coord.X, coord.Y),
                    CheckBoundsNeighbor(quadrants, coord.X, coord.Y + 1)
                },
                new()
                {
                    CheckBoundsNeighbor(quadrants, coord.X + 1, coord.Y - 1),
                    CheckBoundsNeighbor(quadrants, coord.X + 1, coord.Y),
                    CheckBoundsNeighbor(quadrants, coord.X + 1, coord.Y + 1)
                }
            };

            Mock<IQuadrant> mockQuadrant = new();
            mockQuadrant.Setup(q => q.Coordinates).Returns(coord);

            var neighbors = _galaxy.GetNeighborhood(mockQuadrant.Object).ToArray();

            Assert.That(neighbors, Is.EqualTo(expected), "Neighborhood should have 3 rows.");
        }

        [Test]
        [Category("Neighborhood")]
        public void GetNeighborhood_OutOfBounds_IsNull()
        {
            var coord = new Coordinates(0, 0);
            Mock<IQuadrant> mockQuadrant = new();
            mockQuadrant.Setup(q => q.Coordinates).Returns(coord);
            var neighbors = _galaxy.GetNeighborhood(mockQuadrant.Object).ToArray();

            Assert.That(neighbors[0].ElementAt(0), Is.Null, "Out-of-bounds neighbor should be null.");
        }

        [Test]
        [Category("Neighborhood")]
        public void GetNeighborhood_Neighbor_IsNotNull()
        {
            var coord = new Coordinates(0, 0);
            Mock<IQuadrant> mockQuadrant = new();
            mockQuadrant.Setup(q => q.Coordinates).Returns(coord);
            var neighbors = _galaxy.GetNeighborhood(mockQuadrant.Object).ToArray();

            Assert.That(neighbors[2].ElementAt(2), Is.Not.Null, "Valid neighbor should not be null.");
        }


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
