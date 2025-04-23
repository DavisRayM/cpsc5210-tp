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
        public void Invalid_Direction_In_Course_Constructor_Should_Throw_ArgumentOutOfRangeException(float direction)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Course(direction));
        }

        [Test]
        public void Valid_Direction_Low_In_Course_Constructor_Should_Correctly_Set_DeltaX_And_DeltaY()
        {
            Course testCourse = new(1);
            Assert.Multiple(() =>
            {
                Assert.That(testCourse.DeltaX, Is.EqualTo(0));
                Assert.That(testCourse.DeltaY, Is.EqualTo(1));
            });
        }

        [Test]
        public void Valid_Direction_Nominal_In_Course_Constructor_Should_Correctly_Set_DeltaX_And_DeltaY()
        {
            Course testCourse = new(5.5f);
            Assert.Multiple(() =>
            {
                Assert.That(testCourse.DeltaX, Is.EqualTo(0.5));
                Assert.That(testCourse.DeltaY, Is.EqualTo(-1));
            });
        }

        [Test]
        public void Valid_Direction_High_In_Course_Constructor_Should_Correctly_Set_DeltaX_And_DeltaY()
        {
            Course testCourse = new(9);
            Assert.Multiple(() =>
            {
                Assert.That(testCourse.DeltaX, Is.EqualTo(0));
                Assert.That(testCourse.DeltaY, Is.EqualTo(1));
            });
        }
    }

}
