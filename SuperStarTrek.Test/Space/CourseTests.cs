using SuperStarTrek.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStarTrek.Test.Space
{
    public class CourseTests
    {
        [Test]
        [TestCase(-10000000)]
        [TestCase(0.999999f)]
        [TestCase(9.000001f)]
        [TestCase(100000000)]
        public void Invalid_Direction_In_Course_Constructor_Should_Throw_ArgumentOutOfRangeException(
            float direction
        )
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Course(direction));
        }

        [Test]
        [TestCase(1, 0, 1)]
        [TestCase(5.5f, 0.5f, -1)]
        [TestCase(9, 0, 1)]
        public void Valid_Direction_In_Course_Constructor_Should_Correctly_Set_DeltaX_And_DeltaY(
            float direction, float expectedDeltaX, float expectedDeltaY
        )
        {
            Course testCourse = new(direction);
            Assert.Multiple(() =>
            {
                Assert.That(testCourse.DeltaX, Is.EqualTo(expectedDeltaX));
                Assert.That(testCourse.DeltaY, Is.EqualTo(expectedDeltaY));
            });
        }

        [Test]
        [TestCase(1, 0, 0, ExpectedResult = new[] { 
            "1 , 2", "1 , 3", "1 , 4", "1 , 5", "1 , 6", "1 , 7", "1 , 8" 
        })]
        [TestCase(5.5f, 0, 3, ExpectedResult = new[] { "2 , 3", "2 , 2", "3 , 1" })]
        [TestCase(9, 0, 7, ExpectedResult = new string[0])]
        [TestCase(9, 3, 3, ExpectedResult = new[] { "4 , 5", "4 , 6", "4 , 7", "4 , 8" })]
        [TestCase(1, 3, 7, ExpectedResult = new string[0])]
        [TestCase(5.5f, 3, 0, ExpectedResult = new string[0])]
        [TestCase(5.5f, 7, 7, ExpectedResult = new string[0])]
        [TestCase(9, 7, 0, ExpectedResult = new[] { 
            "8 , 2", "8 , 3", "8 , 4", "8 , 5", "8 , 6", "8 , 7", "8 , 8" 
        })]
        [TestCase(1, 7, 3, ExpectedResult = new[] { "8 , 5", "8 , 6", "8 , 7", "8 , 8" })]
        public string[] Valid_GetSectorFrom_Returns_Correct_Coordinates(
            float direction, int startX, int startY
        )
        {
            Course testCourse = new(direction);
            return [.. testCourse
                .GetSectorsFrom(new Coordinates(startX, startY))
                .Select(c => c.ToString())];
        }

        [Test]
        [TestCase(0, 0, -1, false, 0, 0)]
        [TestCase(0, 2, 0, true, 0, 2)]
        [TestCase(2, 4, 4, true, 3, 0)]
        [TestCase(7, 0, 0, true, 7, 0)]
        [TestCase(7, 7, 1, false, 7, 7)]
        [TestCase(6, 6, 20, false, 7, 7)]
        public void Valid_GetNewCoordinate_Returns_Correct_Quadrant_And_Sector(
            int quadrant,
            int sector,
            float sectorsTravelled,
            bool expectedValidQuadrant,
            int expectedNewQuadrant,
            int expectedNewSector
        )
        {
            (bool validQuadrant, int newQuadrant, int newSector) = Course.GetNewCoordinate(
                quadrant, sector, sectorsTravelled
            );
            Assert.Multiple(() =>
            {
                Assert.That(validQuadrant, Is.EqualTo(expectedValidQuadrant));
                Assert.That(newQuadrant, Is.EqualTo(expectedNewQuadrant));
                Assert.That(newSector, Is.EqualTo(expectedNewSector));
            });
        }
    }

}
