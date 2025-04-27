using SuperStarTrek.Space;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStarTrek.Test.Space
{
    [TestFixture]
    public class CoordinatesTests
    {
        [TestCase(0, 0)]
        [TestCase(7, 7)]
        [TestCase(3, 5)]
        public void Constructor_ValidCoordinates_SetsProperties(int x, int y)
        {
            var coord = new Coordinates(x, y);

            Assert.That(coord.X, Is.EqualTo(x));
            Assert.That(coord.Y, Is.EqualTo(y));
            Assert.That(coord.RegionIndex, Is.EqualTo((x << 1) + (y >> 2)));
            Assert.That(coord.SubRegionIndex, Is.EqualTo(y % 4));
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(8, 0)]
        [TestCase(0, 8)]
        public void Constructor_InvalidCoordinates_ThrowsException(int x, int y)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Coordinates(x, y));
            StringAssert.Contains("Must be 0 to 7 inclusive", ex.Message);
        }

        [TestCase(0f, 0f, true, 0, 0)]
        [TestCase(7f, 7.2f, true, 7, 7)]
        [TestCase(-0.5f, 3f, false, 0, 3)]
        [TestCase(-0.4f, 3f, true, 0, 3)]
        [TestCase(3f, 7.4f, true, 3, 7)]
        [TestCase(3f, 7.5f, false, 3, 8)]
        [TestCase(3.6f, 4.4f, true, 4, 4)]
        [TestCase(-0.4f, 7.4f, true, 0, 7)]
        public void TryCreate_RoundedFloatCoordinates_ReturnsExpected(float x, float y, bool expectedResult, int expectedX, int expectedY)
        {
            var result = Coordinates.TryCreate(x, y, out var coord);

            Assert.That(result, Is.EqualTo(expectedResult));

            if (expectedResult)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(coord.X, Is.EqualTo(expectedX));
                    Assert.That(coord.Y, Is.EqualTo(expectedY));
                });
            }
            else
            {
                Assert.That(coord, Is.Null.Or.EqualTo(default(Coordinates)));
            }
        }

        [TestCase(0, 0, "1 , 1")]
        [TestCase(6, 4, "7 , 5")]
        public void ToString_ReturnsHumanFriendlyFormat(int x, int y, string expected)
        {
            var coord = new Coordinates(x, y);
            Assert.That(coord.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void GetDistanceTo_ComputesDistanceCorrectly()
        {
            var a = new Coordinates(0, 0);
            var b = new Coordinates(3, 4);

            var distance = a.GetDistanceTo(b);

            Assert.That(distance, Is.EqualTo(5).Within(0.01));
        }
    }
}
