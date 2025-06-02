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

        #region Constructor

        [Test]
        public void LibraryComputer_Constructor_CorrectlySets_Name()
        {
            Assert.That(_testLibraryComputer.Name, Is.EqualTo("Library-Computer"));
        }
        
        [Test]
        public void LibraryComputer_Constructor_CorrectlySets_Command()
        {
            Assert.That(_testLibraryComputer.Command, Is.EqualTo(Command.COM));
        }

        [Test]
        public void LibraryComputer_Constructor_CorrectlySets_Condition()
        {
            Assert.That(_testLibraryComputer.Condition, Is.EqualTo(0));
        }

        [Test]
        public void LibraryComputer_Constructor_CorrectlySets_IO()
        {
            Assert.That(_testLibraryComputer._io, Is.EqualTo(_ioMock.Object));
        }

        [Test]
        public void LibraryComputer_Constructor_CorrectlySets_Functions()
        {
            Assert.That(_testLibraryComputer._functions, Is.EqualTo(_computerFunctionMocks));
        }

        #endregion Constructor

        #region CanExecuteCommand

        [Test]
        public void CanExecuteCommand_Damaged_ShouldReturnFalse()
        {
            _ioMock.Setup(io => io.ReadNumber("Computer active and waiting command")).Returns(2);

            _testLibraryComputer.TakeDamage(1);

            Assert.That(_testLibraryComputer.CanExecuteCommand(), Is.EqualTo(false));
        }

        [Test]
        public void CanExecuteCommand_Damaged_Should_Print_Disabled()
        {
            _ioMock.Setup(io => io.ReadNumber("Computer active and waiting command")).Returns(2);

            _testLibraryComputer.TakeDamage(1);

            _testLibraryComputer.CanExecuteCommand();

            _ioMock.Verify(io => io.WriteLine("Computer disabled"), Times.Once);
        }

        // is not damaged
        [Test]
        public void CanExecuteCommand_NotDamaged_ShouldReturnTrue()
        {
            _ioMock.Setup(io => io.ReadNumber("Computer active and waiting command")).Returns(2);

            Assert.That(_testLibraryComputer.CanExecuteCommand(), Is.EqualTo(true));
        }

        [Test]
        public void CanExecuteCommand_NotDamaged_Should_Print_Disabled()
        {
            _ioMock.Setup(io => io.ReadNumber("Computer active and waiting command")).Returns(2);

            _ioMock.Verify(io => io.WriteLine("Computer disabled"), Times.Never);
        }

        #endregion CanExecuteCommand

        #region ExecuteCommandCore

        [Test]
        public void Valid_ExecuteCommandCore_Returns_Ok()
        {
            _ioMock.Setup(io => io.ReadNumber("Computer active and waiting command")).Returns(0);

            Mock<IQuadrant> quadrantMock = new();

            Assert.That(
                _testLibraryComputer.ExecuteCommandCore(quadrantMock.Object),
                Is.EqualTo(CommandResult.Ok)
            );
        }

        [Test]
        public void Valid_ExecuteCommandCore_Prints_EmptyString()
        {
            _ioMock.Setup(io => io.ReadNumber("Computer active and waiting command")).Returns(0);

            Mock<IQuadrant> quadrantMock = new();

            _testLibraryComputer.ExecuteCommandCore(quadrantMock.Object);

            _ioMock.Verify(io => io.WriteLine(""), Times.Once);
        }

        [Test]
        public void Valid_ExecuteCommandCore_Executes_ComputerFunction()
        {
            _ioMock.Setup(io => io.ReadNumber("Computer active and waiting command")).Returns(0);

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
            _ioMock.Setup(io => io.ReadNumber("Computer active and waiting command")).Returns(5);

            Assert.That(_testLibraryComputer.GetFunctionIndex(), Is.EqualTo(5));
        }

        [Test]
        public void GetFunctionIndex_NegativeIndex_Returns_Correct_Index()
        {
            Queue<float> indices = new([-1f, 2f]);
            _ioMock.Setup(io => io.ReadNumber("Computer active and waiting command")).Returns(() => indices.Dequeue());

            Assert.That(_testLibraryComputer.GetFunctionIndex(), Is.EqualTo(2));
        }

        [Test]
        public void GetFunctionIndex_NegativeIndex_Displays_ComputerFunctions()
        {
            Queue<float> indices = new([-1f, 2f]);
            _ioMock.Setup(io => io.ReadNumber("Computer active and waiting command")).Returns(() => indices.Dequeue());

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
            _ioMock.Setup(io => io.ReadNumber("Computer active and waiting command")).Returns(() => indices.Dequeue());

            Assert.That(_testLibraryComputer.GetFunctionIndex(), Is.EqualTo(2));
        }

        [Test]
        public void GetFunctionIndex_TooLargeIndex_Displays_ComputerFunctions()
        {
            Queue<float> indices = new([1000f, 2f]);
            _ioMock.Setup(io => io.ReadNumber("Computer active and waiting command")).Returns(() => indices.Dequeue());

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
