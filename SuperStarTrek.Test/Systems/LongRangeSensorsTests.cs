using Games.Common.IO;
using Games.Common.Randomness;
using Moq;
using SuperStarTrek.Space;
using SuperStarTrek.Systems;

namespace SuperStarTrek.Test.Systems
{
    public class LongRangeSensorsTests
    {
        #region CanExecuteCommand

        [Test]
        public void CanExecuteCommand_IsDamaged_True_Should_WriteToIO_And_ReturnFalse()
        {
            Mock<IReadWrite> ioMock = new();

            LongRangeSensors testLongRangeSensors = new(
                new Mock<Galaxy>(new Mock<IRandom>().Object).Object,
                ioMock.Object
            );
            
            testLongRangeSensors.TakeDamage(0.0000000001f);

            Assert.That(testLongRangeSensors.CanExecuteCommand(), Is.EqualTo(false));

            ioMock.Verify(io => io.WriteLine("Long Range Sensors are inoperable"), Times.Once);
        }

        [Test]
        public void CanExecuteCommand_IsDamaged_False_Should_ReturnFalse()
        {
            LongRangeSensors testLongRangeSensors = new(
                new Mock<Galaxy>(new Mock<IRandom>().Object).Object,
                new Mock<IReadWrite>().Object
            );
            
            Assert.That(testLongRangeSensors.CanExecuteCommand(), Is.EqualTo(true));
        }

        #endregion CanExecuteCommand
    }
}
