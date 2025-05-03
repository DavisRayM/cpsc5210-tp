using Games.Common.IO;
using Moq;
using SuperStarTrek.Systems;
using SuperStarTrek.Systems.ComputerFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStarTrek.Test.Systems
{
    public class LibraryComputerTests
    {
        #region GetFunctionIndex

        [Test]
        public void Valid_GetFunctionIndex_Behaves_Correctly()
        {
            Mock<IReadWrite> ioMock = new();
            ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(2);

            Mock<IReadWrite> ioMockForComputerFunctions = new();

            ComputerFunction[] computerFunctionMocks = {
                new Mock<ComputerFunction>("description1", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description2", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description3", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description4", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description5", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description6", ioMockForComputerFunctions.Object).Object
            };

            LibraryComputer testLibraryComputer = new(
                ioMock.Object,
                computerFunctionMocks
            );

            Assert.That(testLibraryComputer.GetFunctionIndex(), Is.EqualTo(2));
        }

        [Test]
        public void GetFunctionIndex_NegativeIndex_Displays_ComputerFunctions()
        {
            Mock<IReadWrite> ioMock = new();
            Queue<float> indices = new([-1f, 2f]);
            ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(() => indices.Dequeue());

            Mock<IReadWrite> ioMockForComputerFunctions = new();

            ComputerFunction[] computerFunctionMocks = {
                new Mock<ComputerFunction>("description1", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description2", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description3", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description4", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description5", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description6", ioMockForComputerFunctions.Object).Object
            };

            LibraryComputer testLibraryComputer = new(
                ioMock.Object,
                computerFunctionMocks
            );

            Assert.That(testLibraryComputer.GetFunctionIndex(), Is.EqualTo(2));

            for (int i = 0; i < computerFunctionMocks.Length; i++)
            {
                ioMock.Verify(io => io.WriteLine($"   {i} = {computerFunctionMocks[i].Description}"));
            }
        }

        [Test]
        public void GetFunctionIndex_TooLargeIndex_Displays_ComputerFunctions()
        {
            Mock<IReadWrite> ioMock = new();
            Queue<float> indices = new([1000f, 2f]);
            ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(() => indices.Dequeue());

            Mock<IReadWrite> ioMockForComputerFunctions = new();

            ComputerFunction[] computerFunctionMocks = {
                new Mock<ComputerFunction>("description1", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description2", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description3", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description4", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description5", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description6", ioMockForComputerFunctions.Object).Object
            };

            LibraryComputer testLibraryComputer = new(
                ioMock.Object,
                computerFunctionMocks
            );

            Assert.That(testLibraryComputer.GetFunctionIndex(), Is.EqualTo(2));

            for (int i = 0; i < computerFunctionMocks.Length; i++)
            {
                ioMock.Verify(io => io.WriteLine($"   {i} = {computerFunctionMocks[i].Description}"));
            }
        }

        // Fails because GetFunctionIndex() incorrectly assumes LibraryComputer always has 6 functions
        // Fix: change line 37 in LibraryComputer.cs:
            // compare index to functions length instead of checking if <= 5
        [Test]
        public void GetFunctionIndex_WrongIndex_HigherThanFunctionsLength_Displays_ComputerFunctions()
        {
            Mock<IReadWrite> ioMock = new();
            Queue<float> indices = new([2f, 0f]);
            ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(() => indices.Dequeue());

            Mock<IReadWrite> ioMockForComputerFunctions = new();

            ComputerFunction[] computerFunctionMocks = {
                new Mock<ComputerFunction>("description1", ioMockForComputerFunctions.Object).Object,
            };

            LibraryComputer testLibraryComputer = new(
                ioMock.Object,
                computerFunctionMocks
            );

            Assert.That(testLibraryComputer.GetFunctionIndex(), Is.EqualTo(0));

            for (int i = 0; i < computerFunctionMocks.Length; i++)
            {
                ioMock.Verify(io => io.WriteLine($"   {i} = {computerFunctionMocks[i].Description}"));
            }
        }

        // more than 6 functions, index should be valid but is > 5 -- will fail
        [Test]
        public void GetFunctionIndex_ValidIndex_GreaterThan5_Should_Return()
        {
            Mock<IReadWrite> ioMock = new();
            Queue<float> indices = new([6f, 2f]);
            ioMock.Setup(io => io.ReadNumber(It.IsAny<string>())).Returns(() => indices.Dequeue());

            Mock<IReadWrite> ioMockForComputerFunctions = new();

            ComputerFunction[] computerFunctionMocks = {
                new Mock<ComputerFunction>("description1", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description2", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description3", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description4", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description5", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description6", ioMockForComputerFunctions.Object).Object,
                new Mock<ComputerFunction>("description7", ioMockForComputerFunctions.Object).Object
            };

            LibraryComputer testLibraryComputer = new(
                ioMock.Object,
                computerFunctionMocks
            );

            Assert.That(testLibraryComputer.GetFunctionIndex(), Is.EqualTo(6));
        }

        #endregion GetFunctionIndex
    }
}
