using Moq;
using Games.Common.Randomness;
using SuperStarTrek.Space;

namespace SuperStarTrek.Test.Space
{
    public class QuadrantInfoTests
    {
        private const String Name = "Test";
        private Coordinates Coords = new Coordinates(0, 0);

        /**
         * Test that `Create` sets the correct `KlingonCount` for the floats returned
         */
        [TestCase(0.80f, ExpectedResult = 0)]
        [TestCase(0f, ExpectedResult = 0)]
        [TestCase(0.81f, ExpectedResult = 1)]
        [TestCase(0.95f, ExpectedResult = 1)]
        [TestCase(0.96f, ExpectedResult = 2)]
        [TestCase(0.98f, ExpectedResult = 2)]
        [TestCase(0.99f, ExpectedResult = 3)]
        [TestCase(1f, ExpectedResult = 3)]
        public int Create_ExpectedKlingonCount_ForFloatValue(float randomValue)
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(randomValue)
                .Returns(0f)
                .Returns(0f);

            var quadrantInfo = QuadrantInfo.Create(Coords, Name, mockRandom.Object);

            return quadrantInfo.KlingonCount;
        }

        /**
         * Test that `Create` sets the correct `HasStarbase` flag depending on returned
         * float
         */
        [TestCase(0.96f, ExpectedResult = false)]
        [TestCase(0f, ExpectedResult = false)]
        [TestCase(0.97f, ExpectedResult = true)]
        [TestCase(1f, ExpectedResult = true)]
        public bool Create_ExpectedHasStarbase_ForFloatValue(float randomValue)
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(randomValue)
                .Returns(0f);

            var quadrantInfo = QuadrantInfo.Create(Coords, Name, mockRandom.Object);

            return quadrantInfo.HasStarbase;
        }

        /**
         * Test that `Create` sets the correct `StarCount` quantity depending on returned float.
         */
        [TestCase(0.0f, ExpectedResult = 1)]
        [TestCase(0.5f, ExpectedResult = 5)]
        [TestCase(1.0f, ExpectedResult = 8)]
        public int Create_ExpectedStarCount_ForFloatValue(float randomValue)
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(randomValue);

            var quadrantInfo = QuadrantInfo.Create(Coords, Name, mockRandom.Object);

            return quadrantInfo.StarCount;
        }

        /**
         * Test that `AddKlingon` increases the KlingonCount
         */
        [TestCase(0.80f, ExpectedResult = 1)]
        [TestCase(0.99f, ExpectedResult = 4)]
        public int AddKlingon_ValidQuadrantInfo_IncreasesKlingonCount(float randomValue)
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(randomValue)
                .Returns(0f)
                .Returns(0f);

            var quadrantInfo = QuadrantInfo.Create(Coords, Name, mockRandom.Object);

            quadrantInfo.AddKlingon();

            return quadrantInfo.KlingonCount;
        }

        /**
         * Test that `AddStarbase` switches the `HasStarbase` flag
         */
        [Test]
        public void AddStarbase_WithQuadrantInfoWithoutStarbase_SetsHasStarBase()
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);

            var quadrantInfo = QuadrantInfo.Create(Coords, Name, mockRandom.Object);
            Assert.False(quadrantInfo.HasStarbase, "Initially created quadrantInfo already has starbase");

            quadrantInfo.AddStarbase();
            Assert.True(quadrantInfo.HasStarbase);
        }

        /**
         * Test `MarkAsKnown` marks the QuadrantInfo as Known
         */
        [Test]
        public void MarkAsKnown_SetsIsKnown()
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);

            var quadrantInfo = QuadrantInfo.Create(Coords, Name, mockRandom.Object);

            Assert.AreEqual("***", quadrantInfo.ToString());
            quadrantInfo.MarkAsKnown();
            Assert.AreEqual("001", quadrantInfo.ToString());
        }

        /**
         * Test that `Scan` returns the expected string when called
         */
        [Test]
        public void Scan_ReturnsExpectedString()
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);

            var quadrantInfo = QuadrantInfo.Create(Coords, Name, mockRandom.Object);
            Assert.AreEqual("001", quadrantInfo.Scan());
        }

        /**
         * Test that `ToString` returns expected string depending on `_isKnown`, `klingonCount`,
         * `hasStarBase`, & `starCount`.
         */
        [TestCase(0f, 0f, 0f, false, ExpectedResult = "***")]
        [TestCase(0f, 0f, 0f, true, ExpectedResult = "001")]
        [TestCase(0f, 0f, 0.5f, true, ExpectedResult = "005")]
        [TestCase(0f, 0.97f, 0f, true, ExpectedResult = "011")]
        [TestCase(0.99f, 0f, 0f, true, ExpectedResult = "301")]
        public string ToString_DependingOnState_ReturnsString(float klingonRand, float hasStarBaseRand, float starCountRand, bool isKnown)
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(klingonRand)
                .Returns(hasStarBaseRand)
                .Returns(starCountRand);

            var quadrantInfo = QuadrantInfo.Create(Coords, Name, mockRandom.Object);
            if (isKnown) { quadrantInfo.MarkAsKnown(); }
            return quadrantInfo.ToString();
        }


        /**
         * Test that `RemoveStarbase` switches the `HasStarbase` flag
         */
        [Test]
        public void RemoveStarbase_WithQuadrantInfoWithoutStarbase_UnsetsHasStarBase()
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(0f)
                .Returns(0f)
                .Returns(0f);

            var quadrantInfo = QuadrantInfo.Create(Coords, Name, mockRandom.Object);
            quadrantInfo.AddStarbase();
            quadrantInfo.RemoveStarbase();
            Assert.False(quadrantInfo.HasStarbase);
        }

        /**
         * Test that `RemoveKlingon` only modifies the `KlingonCount` when it's Greater
         * than 0.
         */
        [TestCase(0f, 0, ExpectedResult = 0)]
        [TestCase(0.81f, 1, ExpectedResult = 0)]
        [TestCase(0.96f, 2, ExpectedResult = 1)]
        public int RemoveKlingon_ModifiesKlingonCount_WhenGreaterThanZero(float randomValue, int initial)
        {
            var mockRandom = new Mock<IRandom>();
            mockRandom.SetupSequence(r => r.NextFloat())
                .Returns(randomValue)
                .Returns(0f)
                .Returns(0f);

            var quadrantInfo = QuadrantInfo.Create(Coords, Name, mockRandom.Object);

            Assert.AreEqual(quadrantInfo.KlingonCount, initial);
            quadrantInfo.RemoveKlingon();

            return quadrantInfo.KlingonCount;
        }
    }
}
