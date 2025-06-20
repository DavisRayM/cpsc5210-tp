﻿using NUnit.Framework;
using Moq;
using SuperStarTrek.Systems.ComputerFunctions;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Utils;
using Games.Common.IO;
using Games.Common.Randomness;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Diagnostics.Metrics;
using System.Numerics;

namespace SuperStarTrek.Test.Systems.ComputerFunctions
{
    [TestFixture]
    public class DirectionDistanceCalculatorTests
    {
        private Mock<IReadWrite> _io;
        private Mock<IRandom> _random;
        private IOSpy _ioSpy;
        private Mock<Galaxy> _galaxy;
        private Mock<Enterprise> _enterprise;
        private Mock<IQuadrant> _quadrant;
        private DirectionDistanceCalculator _calculator;
        private static IEnumerable<TestCaseData> ValidCoordinates => new[]
        {
            new TestCaseData((0, 0),(0, 5)),
            new TestCaseData((4, 4),(3, 2)),
            new TestCaseData((7, 7),(1, 5)),
            new TestCaseData((0, 7),(2, 7)),
            new TestCaseData((7, 0),(1, 1)),
            new TestCaseData((3, 2),(1, 2))
        };

        [SetUp]
        public void Setup()
        {
            //_io = new Mock<IReadWrite>();
            _random = new Mock<IRandom>();

            // Setup Galaxy, Enterprise, and Quadrant
            _ioSpy = new();
            _galaxy = new Mock<Galaxy>(_random.Object);
            _quadrant = new Mock<IQuadrant>();
            _enterprise = new Mock<Enterprise>(
                1000,
                new Coordinates(1, 1),
                _ioSpy,
                _random.Object
            );
        }

        [Test, TestCaseSource(nameof(ValidCoordinates))]
        public void Execute_PrintsExpectedLocationAndPromptsForCoordinates(
            (int, int) qC, (int, int) sC
        )
        {
            Coordinates quadrantCoords = new(qC.Item1, qC.Item2);
            Coordinates sectorCoords = new(sC.Item1, sC.Item2);
            Coordinates from = new(qC.Item1, qC.Item2);
            Coordinates to = new(sC.Item1, sC.Item2);

            _enterprise.Setup(q => q.QuadrantCoordinates).Returns(quadrantCoords);
            //_enterprise.Setup(q => q.SectorCoordinates).Returns(sectorCoords);
            _enterprise.Object.SectorCoordinates = sectorCoords;

            _ioSpy.EnqueueRead2Numbers(qC);
            _ioSpy.EnqueueRead2Numbers(sC);

            var (direction, distance) = DirectionAndDistance.From(from.X, from.Y).To(to.X, to.Y);

            // Act
            _calculator = new DirectionDistanceCalculator(_enterprise.Object, _ioSpy);
            _calculator.Execute(_quadrant.Object);

            // Assert
            var output = _ioSpy.GetOutput();
            String[] expectedOutput = [
                "Direction/distance calculator:",
                $"You are at quadrant {quadrantCoords}",
                $" sector {sectorCoords}",
                "Please enter",
                $"Direction = {direction}",
                $"Distance = {distance}",
                ""
            ];

            Assert.That(output, Is.EqualTo(string.Join(Environment.NewLine, expectedOutput)));
        }
    }
}
