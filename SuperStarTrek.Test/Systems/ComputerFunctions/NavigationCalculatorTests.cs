using System;
using Moq;
using NUnit.Framework;
using SuperStarTrek.Space;
using SuperStarTrek.Systems.ComputerFunctions;
using Games.Common.IO;
using SuperStarTrek.Utils;

namespace SuperStarTrek.Test.Systems.ComputerFunctions
{
    internal class TestNavigationCalculator : NavigationCalculator
    {
        public TestNavigationCalculator(IReadWrite io)
            : base("Test NavCalc", io) { }

        public void CallWriteDirectionAndDistance(Coordinates from, Coordinates to) =>
            WriteDirectionAndDistance(from, to);

        public void CallWriteDirectionAndDistance((float X, float Y) from, (float X, float Y) to) =>
            WriteDirectionAndDistance(from, to);

        internal override void Execute(IQuadrant quadrant) { 
            // Stub
        }

    }

    [TestFixture]
    public class NavigationCalculatorTests
    {
        private Mock<IReadWrite> _mockIo;
        private TestNavigationCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _mockIo = new Mock<IReadWrite>();
            _calculator = new TestNavigationCalculator(_mockIo.Object);
        }

        [Test]
        public void WriteDirectionAndDistance_WithCoordinates_PrintsExpectedValues()
        {
            var from = new Coordinates(1, 1); // prints as 2 , 2
            var to = new Coordinates(4, 3);   // prints as 5 , 4

            var (direction, distance) = from.GetDirectionAndDistanceTo(to);

            _calculator.CallWriteDirectionAndDistance(from, to);

            _mockIo.Verify(io => io.WriteLine($"Direction = {direction}"), Times.Once);
            _mockIo.Verify(io => io.WriteLine($"Distance = {distance}"), Times.Once);
        }

        [Test]
        public void WriteDirectionAndDistance_WithTuples_PrintsExpectedValues()
        {
            (float X, float Y) from = (2f, 3f);
            (float X, float Y) to = (6f, 7f);

            var (direction, distance) = DirectionAndDistance.From(from.X, from.Y).To(to.X, to.Y);

            _calculator.CallWriteDirectionAndDistance(from, to);

            _mockIo.Verify(io => io.WriteLine($"Direction = {direction}"), Times.Once);
            _mockIo.Verify(io => io.WriteLine($"Distance = {distance}"), Times.Once);
        }
    }
}
