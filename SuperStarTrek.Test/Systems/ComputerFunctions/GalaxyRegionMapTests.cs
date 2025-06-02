using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SuperStarTrek.Space;
using SuperStarTrek.Systems.ComputerFunctions;
using Games.Common.IO;
using Games.Common.Randomness;

namespace SuperStarTrek.Test.Systems.ComputerFunctions
{
    internal class TestableGalaxyRegionMap : GalaxyRegionMap
    {
        public TestableGalaxyRegionMap(IReadWrite io, Galaxy galaxy)
            : base(io, galaxy) { }

        public void CallWriteHeader(IQuadrant quadrant) => WriteHeader(quadrant);

        public IEnumerable<string> CallGetRowData() => GetRowData();
    }

    [TestFixture]
    public class GalaxyRegionMapTests
    {
        private Mock<IReadWrite> _mockIo;
        private Galaxy _galaxy;
        private TestableGalaxyRegionMap _regionMap;

        [SetUp]
        public void Setup()
        {
            _mockIo = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            _galaxy = new Galaxy(mockRandom.Object);
            _regionMap = new TestableGalaxyRegionMap(_mockIo.Object, _galaxy);
        }

        [Test]
        public void WriteHeader_PrintsGalaxyTitle()
        {
            var mockQuadrant = new Mock<IQuadrant>();

            _regionMap.CallWriteHeader(mockQuadrant.Object);

            _mockIo.Verify(io => io.WriteLine("                        The Galaxy"), Times.Once);
        }

        [Test]
        public void GetRowData_ParsesRegionNamesCorrectly()
        {
            var expected = SuperStarTrek.Resources.Strings.RegionNames
                .Split('\n')
                .Select(n => n.TrimEnd('\r'));

            var result = _regionMap.CallGetRowData();

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
