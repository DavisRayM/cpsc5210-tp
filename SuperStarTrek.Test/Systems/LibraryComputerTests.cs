using Games.Common.IO;
using Games.Common.Randomness;
using Moq;
using SuperStarTrek.Commands;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Systems;
using SuperStarTrek.Systems.ComputerFunctions;
using System;

namespace SuperStarTrek.Test.Systems
{
    public class LibraryComputerTests
    {
        internal LibraryComputer _testLibraryComputer;
        public Mock<IReadWrite> _ioMock;
        internal ComputerFunction[] _computerFunctionMocks;

        [SetUp]
        public void SetUp()
        {
            _ioMock = new();

            Mock<IReadWrite> ioMockForComputerFunctions = new();

            _computerFunctionMocks = [
                new Mock<ComputerFunction>("description1", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description2", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description3", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description4", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description5", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description6", ioMockForComputerFunctions.Object).Object
            ];

            _testLibraryComputer = new(
                _ioMock.Object,
                _computerFunctionMocks
            );
        }

        #region CanExecuteCommand

        [Test]
        public void CanExecuteCommand_Damaged_ShouldReturnFalse()
        {
            _ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(2);

            _testLibraryComputer.TakeDamage(1);

            Assert.That(_testLibraryComputer.CanExecuteCommand(), Is.EqualTo(false));
        }

        [Test]
        public void CanExecuteCommand_Damaged_Should_Print_Disabled()
        {
            _ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(2);

            _testLibraryComputer.TakeDamage(1);

            _testLibraryComputer.CanExecuteCommand();

            _ioMock.Verify(io => io.WriteLine("Computer disabled"), Times.Once);
        }

        // is not damaged
        [Test]
        public void CanExecuteCommand_NotDamaged_ShouldReturnTrue()
        {
            _ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(2);

            Assert.That(_testLibraryComputer.CanExecuteCommand(), Is.EqualTo(true));
        }

        [Test]
        public void CanExecuteCommand_NotDamaged_Should_Print_Disabled()
        {
            _ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(2);

            _ioMock.Verify(io => io.WriteLine("Computer disabled"), Times.Never);
        }

        #endregion CanExecuteCommand

        #region ExecuteCommandCore

        [Test]
        public void Valid_ExecuteCommandCore_Returns_Ok()
        {
            _ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(0);

            Mock<IQuadrant> quadrantMock = new();

            Assert.That(
                _testLibraryComputer.ExecuteCommandCore(quadrantMock.Object),
                Is.EqualTo(CommandResult.Ok)
            );
        }

        [Test]
        public void Valid_ExecuteCommandCore_Prints_EmptyString()
        {
            _ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(0);

            Mock<IQuadrant> quadrantMock = new();

            _testLibraryComputer.ExecuteCommandCore(quadrantMock.Object);

            _ioMock.Verify(io => io.WriteLine(""), Times.Once);
        }

        [Test]
        public void Valid_ExecuteCommandCore_Executes_ComputerFunction()
        {
            _ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(0);

            Mock<IReadWrite> ioMockForComputerFunction = new();

            // Use this one instead of _computerFunctionMocks for verification purposes
            Mock<ComputerFunction> computerFunctionMock = new(
                "description1",
                ioMockForComputerFunction.Object
            );

            Mock<IQuadrant> quadrantMock = new();

            LibraryComputer testLibraryComputer = new(
                _ioMock.Object,
                computerFunctionMock.Object
            );

            testLibraryComputer.ExecuteCommandCore(quadrantMock.Object);

            computerFunctionMock.Verify(
                computerFunction => computerFunction.Execute(quadrantMock.Object)
            );
        }

        #endregion ExecuteCommandCore

        #region GetFunctionIndex

        [Test]
        public void Valid_GetFunctionIndex_Returns_Correct_Index()
        {
            _ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(2);

            Assert.That(_testLibraryComputer.GetFunctionIndex(), Is.EqualTo(2));
        }

        [Test]
        public void GetFunctionIndex_NegativeIndex_Returns_Correct_Index()
        {
            Queue<float> indices = new([-1f, 2f]);
            _ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(() => indices.Dequeue());

            Assert.That(_testLibraryComputer.GetFunctionIndex(), Is.EqualTo(2));
        }

        [Test]
        public void GetFunctionIndex_NegativeIndex_Displays_ComputerFunctions()
        {
            Queue<float> indices = new([-1f, 2f]);
            _ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(() => indices.Dequeue());

            _testLibraryComputer.GetFunctionIndex();

            for (int i = 0; i < _computerFunctionMocks.Length; i++)
            {
                _ioMock.Verify(
                    io => io.WriteLine($"   {i} = {_computerFunctionMocks[i].Description}"),
                    Times.Once
                );
            }
        }

        [Test]
        public void GetFunctionIndex_TooLargeIndex_Returns_Correct_Index()
        {
            Queue<float> indices = new([1000f, 2f]);
            _ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(() => indices.Dequeue());

            Assert.That(_testLibraryComputer.GetFunctionIndex(), Is.EqualTo(2));
        }

        [Test]
        public void GetFunctionIndex_TooLargeIndex_Displays_ComputerFunctions()
        {
            Queue<float> indices = new([1000f, 2f]);
            _ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(() => indices.Dequeue());

            _testLibraryComputer.GetFunctionIndex();

            for (int i = 0; i < _computerFunctionMocks.Length; i++)
            {
                _ioMock.Verify(
                    io => io.WriteLine($"   {i} = {_computerFunctionMocks[i].Description}"),
                    Times.Once
                );
            }
        }

        #endregion GetFunctionIndex
    }
}
