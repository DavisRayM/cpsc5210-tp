using System;
using NUnit.Framework;
using Moq;
using SuperStarTrek.Space;
using SuperStarTrek.Objects;
using Games.Common.IO;
using Games.Common.Randomness;
using System.Collections.Generic;

namespace SuperStarTrek.Space
{
    [TestFixture]
    public class QuadrantTests
    {
        private Mock<IReadWrite> _ioMock;
        private Mock<IRandom> _randomMock;
        private Mock<Galaxy> _galaxyMock;
        private Enterprise _enterprise;
        private QuadrantInfo _quadrantInfo;
        private Quadrant _quadrant;


        [SetUp]
        public void Setup()
        {
            _randomMock = new Mock<IRandom>();
            _ioMock = new Mock<IReadWrite>();
            _galaxyMock = new Mock<Galaxy>(_randomMock.Object) {CallBase = true};

            _quadrantInfo = QuadrantInfo.Create(new Coordinates(0, 0), "Test-Quadrant", _randomMock.Object);

            _enterprise = new Enterprise(5000, new Coordinates(1, 1), _ioMock.Object, _randomMock.Object);

            _galaxyMock.Setup(g => g.GetNeighborhood(It.IsAny<Quadrant>()))
                .Returns(new List<IEnumerable<QuadrantInfo>>());
            _galaxyMock.Setup(g => g[It.IsAny<Coordinates>()]).Returns(_quadrantInfo);

            // Sequence of random values:
            // 1st: for KlingonCount = 2 (needs > 0.95 and ≤ 0.98)
            // 2nd: for HasStarbase = true (needs > 0.96)
            _randomMock.SetupSequence(r => r.NextFloat())
                .Returns(0.96f)   // KlingonCount → 2
                .Returns(0.97f);  // HasStarbase → true

            _randomMock.Setup(r => r.Next(1,9)).Returns(5); // StarCount = 5
           

            
                
            _quadrant = new Quadrant(_quadrantInfo, _enterprise, _randomMock.Object, _galaxyMock.Object, _ioMock.Object);

            

        }

        [Test]
        public void Quadrant_InitializesCorrectly_WithControlledRandomValues()
        {
            Assert.AreEqual(2, _quadrant.KlingonCount);
            Assert.IsTrue(_quadrant.HasStarbase);
        }
    }
}
