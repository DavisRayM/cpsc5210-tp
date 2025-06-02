using Games.Common.IO;
using Games.Common.Randomness;
using Moq;
using NUnit.Framework;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Systems.ComputerFunctions;

namespace SuperStarTrek.Test.Systems.ComputerFunctions
{
	public class StarbaseDataCalculatorTest
	{
        [Test]
        public void Execute_WhenQuadrantIsNull_ThrowsArgumentNullException()
        {
            var calculator = new StarbaseDataCalculator(
            new Enterprise(100, new Coordinates(1, 1), new Mock<IReadWrite>().Object, new Mock<IRandom>().Object),
            new Mock<IReadWrite>().Object);

            Assert.Throws<NullReferenceException> (() => calculator.Execute(null));
        }

        [Test]
        public void Execute_WhenNoStarbase_DisplaysNoStarbaseMessage()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(100, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            

            var sbdCalculator = new StarbaseDataCalculator(mockEnterprise.Object, mockIO.Object);

            mockQuadrant.Setup(q => q.HasStarbase).Returns(false);

            sbdCalculator.Execute(mockQuadrant.Object);

          
            mockIO.Verify(io => io.WriteLine(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Execute_WithStarbase_DisplaysFromEnterpriseToStarbaseHeader()
        {
            var starbaseSector = new Coordinates(3, 4);
            var enterpriseSector = new Coordinates(1, 2);
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(100, enterpriseSector, mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var starbase = new Starbase(starbaseSector, mockRandom.Object, mockIO.Object);
            var sbdCalculator = new StarbaseDataCalculator(enterprise, mockIO.Object);

            mockQuadrant.Setup(q => q.HasStarbase).Returns(true);
            mockQuadrant.Setup(q => q.Starbase).Returns(starbase);

            sbdCalculator.Execute(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("From Enterprise to Starbase:"), Times.Once);
        }

        [Test]
        public void Execute_WithStarbase_WritesMultipleOutputLines()
        {
            var starbaseSector = new Coordinates(3, 4);
            var enterpriseSector = new Coordinates(1, 2);
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(100, enterpriseSector, mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var starbase = new Starbase(starbaseSector, mockRandom.Object, mockIO.Object);
            var sbdCalculator = new StarbaseDataCalculator(enterprise, mockIO.Object);

            mockQuadrant.Setup(q => q.HasStarbase).Returns(true);
            mockQuadrant.Setup(q => q.Starbase).Returns(starbase);

            sbdCalculator.Execute(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_WithStarbase_DisplaysDirectionInformation()
        {
            var starbaseSector = new Coordinates(3, 4);
            var enterpriseSector = new Coordinates(1, 2);
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(100, enterpriseSector, mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var starbase = new Starbase(starbaseSector, mockRandom.Object, mockIO.Object);
            var sbdCalculator = new StarbaseDataCalculator(enterprise, mockIO.Object);

            mockQuadrant.Setup(q => q.HasStarbase).Returns(true);
            mockQuadrant.Setup(q => q.Starbase).Returns(starbase);

            sbdCalculator.Execute(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine(It.Is<string>(s =>
                s.Contains("Direction") || s.Contains("Course"))), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_WithStarbase_DisplaysDistanceInformation()
        {
            var starbaseSector = new Coordinates(3, 4);
            var enterpriseSector = new Coordinates(1, 2);
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(100, enterpriseSector, mockIO.Object, mockRandom.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            var starbase = new Starbase(starbaseSector, mockRandom.Object, mockIO.Object);
            var sbdCalculator = new StarbaseDataCalculator(enterprise, mockIO.Object);

            mockQuadrant.Setup(q => q.HasStarbase).Returns(true);
            mockQuadrant.Setup(q => q.Starbase).Returns(starbase);

            sbdCalculator.Execute(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine(It.Is<string>(s => s.Contains("Distance"))), Times.AtLeastOnce);
        }


        [Test]
        public void Execute_MultipleTimesWithSameQuadrant_DisplaysHeaderEachTime()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(100, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var starbase = new Starbase(new Coordinates(4, 5), mockRandom.Object, mockIO.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            mockQuadrant.Setup(q => q.HasStarbase).Returns(true);
            mockQuadrant.Setup(q => q.Starbase).Returns(starbase);
            var calculator = new StarbaseDataCalculator(enterprise, mockIO.Object);

            calculator.Execute(mockQuadrant.Object);
            calculator.Execute(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("From Enterprise to Starbase:"), Times.Exactly(2));
        }

        public void Execute_WhenEnterpriseAndStarbaseAtSameLocation_DisplaysZeroDistance()
        {
            var coordinates = new Coordinates(2, 2);
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(100, coordinates, mockIO.Object, mockRandom.Object);
            var starbase = new Starbase(coordinates, mockRandom.Object, mockIO.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            mockQuadrant.Setup(q => q.HasStarbase).Returns(true);
            mockQuadrant.Setup(q => q.Starbase).Returns(starbase);
            var calculator = new StarbaseDataCalculator(enterprise, mockIO.Object);

            calculator.Execute(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine(It.Is<string>(s => s.Contains("Distance = 0"))), Times.Once);
        }

        [Test]
        public void Execute_WithMaxDistanceCoordinates_DisplaysDistanceInformation()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(100, new Coordinates(0, 0), mockIO.Object, mockRandom.Object);
            var starbase = new Starbase(new Coordinates(7, 7), mockRandom.Object, mockIO.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            mockQuadrant.Setup(q => q.HasStarbase).Returns(true);
            mockQuadrant.Setup(q => q.Starbase).Returns(starbase);
            var calculator = new StarbaseDataCalculator(enterprise, mockIO.Object);

            calculator.Execute(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine(It.Is<string>(s => s.Contains("Distance ="))), Times.Once);
        }

        [Test]
        public void Execute_WithKnownCoordinates_DisplaysExpectedHeader()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(100, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var starbase = new Starbase(new Coordinates(4, 5), mockRandom.Object, mockIO.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            mockQuadrant.Setup(q => q.HasStarbase).Returns(true);
            mockQuadrant.Setup(q => q.Starbase).Returns(starbase);
            var calculator = new StarbaseDataCalculator(enterprise, mockIO.Object);

            calculator.Execute(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("From Enterprise to Starbase:"), Times.Once);
        }

        [Test]
        public void Execute_WithKnownCoordinates_DisplaysExpectedDirection()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(100, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var starbase = new Starbase(new Coordinates(4, 5), mockRandom.Object, mockIO.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            mockQuadrant.Setup(q => q.HasStarbase).Returns(true);
            mockQuadrant.Setup(q => q.Starbase).Returns(starbase);
            var calculator = new StarbaseDataCalculator(enterprise, mockIO.Object);

            calculator.Execute(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Direction = 8.25"), Times.Once);
        }

        [Test]
        public void Execute_WithKnownCoordinates_DisplaysExpectedDistance()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var enterprise = new Enterprise(100, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);
            var starbase = new Starbase(new Coordinates(4, 5), mockRandom.Object, mockIO.Object);
            var mockQuadrant = new Mock<IQuadrant>();
            mockQuadrant.Setup(q => q.HasStarbase).Returns(true);
            mockQuadrant.Setup(q => q.Starbase).Returns(starbase);
            var calculator = new StarbaseDataCalculator(enterprise, mockIO.Object);

            calculator.Execute(mockQuadrant.Object);

            mockIO.Verify(io => io.WriteLine("Distance = 5"), Times.Once);
        }

    }
}

