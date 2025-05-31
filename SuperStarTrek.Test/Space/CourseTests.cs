using SuperStarTrek.Space;

namespace SuperStarTrek.Test.Space
{
    #region Constructor

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
            Assert.That(() => new Course(direction),
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Message.EqualTo(string.Join(Environment.NewLine,
                        [
                            "Must be between 1 and 9, inclusive. (Parameter 'direction')",
                            "Actual value was " + direction + "."
                        ])));
        }

        [Test]
        [TestCase(1, 0)]
        [TestCase(5.5f, 0.5f)]
        [TestCase(6.5f, 1)]
        [TestCase(9, 0)]
        public void Valid_Direction_In_Course_Constructor_Should_Correctly_Set_DeltaX(
            float direction, float expectedDeltaX
        )
        {
            Course testCourse = new(direction);
            Assert.That(testCourse.DeltaX, Is.EqualTo(expectedDeltaX));
        }

        [Test]
        [TestCase(1, 1)]
        [TestCase(5.5f, -1)]
        [TestCase(6.5f, -0.5f)]
        [TestCase(9, 1)]
        public void Valid_Direction_In_Course_Constructor_Should_Correctly_Set_DeltaY(
            float direction, float expectedDeltaY
        )
        {
            Course testCourse = new(direction);
            Assert.That(testCourse.DeltaY, Is.EqualTo(expectedDeltaY));
        }

        #endregion Constructor
        
        #region GetSectorFrom

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

        #endregion GetSectorFrom

        #region GetNewCoordinate

        [Test]
        [TestCase(0, 0, -1, false)]
        [TestCase(0, 2, 0, true)]
        [TestCase(2, 4, 4, true)]
        [TestCase(7, 0, 0, true)]
        [TestCase(7, 7, 1, false)]
        [TestCase(6, 6, 20, false)]
        public void Valid_GetNewCoordinate_Returns_Correct_Valid_Quadrant(
            int quadrant,
            int sector,
            float sectorsTravelled,
            bool expectedValidQuadrant
        )
        {
            (
                bool validQuadrant, _, _
            ) = Course.GetNewCoordinate(quadrant, sector, sectorsTravelled);
            Assert.That(validQuadrant, Is.EqualTo(expectedValidQuadrant));
        }

        [Test]
        [TestCase(0, 0, -1, 0)]
        [TestCase(0, 2, 0, 0)]
        [TestCase(2, 4, 4, 3)]
        [TestCase(7, 0, 0, 7)]
        [TestCase(7, 7, 1, 7)]
        [TestCase(6, 6, 20, 7)]
        public void Valid_GetNewCoordinate_Returns_Correct_New_Quadrant(
            int quadrant,
            int sector,
            float sectorsTravelled,
            int expectedNewQuadrant
        )
        {
            (
                _, int newQuadrant, _
            ) = Course.GetNewCoordinate(quadrant, sector, sectorsTravelled);
            Assert.That(newQuadrant, Is.EqualTo(expectedNewQuadrant));
        }

        [Test]
        [TestCase(0, 0, -1, 0)]
        [TestCase(0, 2, 0, 2)]
        [TestCase(2, 4, 4, 0)]
        [TestCase(7, 0, 0, 0)]
        [TestCase(7, 7, 1, 7)]
        [TestCase(6, 6, 20, 7)]
        public void Valid_GetNewCoordinate_Returns_Correct_Sector(
            int quadrant,
            int sector,
            float sectorsTravelled,
            int expectedNewSector
        )
        {
            (
                _, _, int newSector
            ) = Course.GetNewCoordinate(quadrant, sector, sectorsTravelled);
            Assert.That(newSector, Is.EqualTo(expectedNewSector));
        }

        #endregion GetNewCoordinate

        #region GetDestination

        [Test]
        [TestCase(1, 4, 4, 4, 4, 0, true)]
        [TestCase(1, 4, 4, 4, 4, 4, true)]
        [TestCase(1, 4, 4, 4, 4, 5, true)]
        [TestCase(1.5f, 4, 4, 4, 4, 8, true)]
        [TestCase(9, 4, 4, 4, 4, 8, true)]
        [TestCase(3, 4, 4, 4, 4, 40, false)]
        [TestCase(4, 0, 0, 0, 0, 5, false)]
        [TestCase(8, 7, 7, 7, 7, 3, false)]
        public void GetDestination_Returns_Correct_ValidPosition(
            float direction,
            int quadrantX,
            int quadrantY,
            int sectorX,
            int sectorY,
            int distance,
            bool expectedValidPosition
        )
        {
            Coordinates quadrant = new(quadrantX, quadrantY);
            Coordinates sector = new(sectorX, sectorY);
            
            Course testCourse = new(direction);
            (
                bool validPosition, _, _
            ) = testCourse.GetDestination(quadrant, sector, distance);
            Assert.That(validPosition, Is.EqualTo(expectedValidPosition));
        }

        [Test]
        [TestCase(1, 4, 4, 4, 4, 0, "5 , 5")]
        [TestCase(1, 4, 4, 4, 4, 4, "5 , 6")]
        [TestCase(1, 4, 4, 4, 4, 5, "5 , 6")]
        [TestCase(1.5f, 4, 4, 4, 4, 8, "5 , 6")]
        [TestCase(9, 4, 4, 4, 4, 8, "5 , 6")]
        [TestCase(3, 4, 4, 4, 4, 40, "1 , 5")]
        [TestCase(4, 0, 0, 0, 0, 5, "1 , 1")]
        [TestCase(8, 7, 7, 7, 7, 3, "8 , 8")]
        public void GetDestination_Returns_Correct_Quadrant(
            float direction,
            int quadrantX,
            int quadrantY,
            int sectorX,
            int sectorY,
            int distance,
            string expectedQuadrant
        )
        {
            Coordinates quadrant = new(quadrantX, quadrantY);
            Coordinates sector = new(sectorX, sectorY);
            
            Course testCourse = new(direction);
            (
                _, Coordinates quadrantResult, _
            ) = testCourse.GetDestination(quadrant, sector, distance);
            Assert.That(quadrantResult.ToString(), Is.EqualTo(expectedQuadrant));
        }

        [Test]
        [TestCase(1, 4, 4, 4, 4, 0, "5 , 5")]
        [TestCase(1, 4, 4, 4, 4, 4, "5 , 1")]
        [TestCase(1, 4, 4, 4, 4, 5, "5 , 2")]
        [TestCase(1.5f, 4, 4, 4, 4, 8, "1 , 5")]
        [TestCase(9, 4, 4, 4, 4, 8, "5 , 5")]
        [TestCase(3, 4, 4, 4, 4, 40, "1 , 5")]
        [TestCase(4, 0, 0, 0, 0, 5, "1 , 1")]
        [TestCase(8, 7, 7, 7, 7, 3, "8 , 8")]
        public void GetDestination_Returns_Correct_Sector(
            float direction,
            int quadrantX,
            int quadrantY,
            int sectorX,
            int sectorY,
            int distance,
            string expectedSector
        )
        {
            Coordinates quadrant = new(quadrantX, quadrantY);
            Coordinates sector = new(sectorX, sectorY);
            
            Course testCourse = new(direction);
            (
                _, _, Coordinates sectorResult
            ) = testCourse.GetDestination(quadrant, sector, distance);
            Assert.That(sectorResult.ToString(), Is.EqualTo(expectedSector));
        }
    }

    #endregion GetDestination
}
