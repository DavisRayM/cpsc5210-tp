using Moq;
using Games.Common.Randomness;
using Games.Common.IO;
using SuperStarTrek.Systems;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Systems.ComputerFunctions;
using SuperStarTrek.Commands;

namespace SuperStarTrek.Test.Integration
{
    public class LibraryComputer_Integration
    {
        [Test]
        public void LibraryComputer_EnterpriseIntegration_Execute()
        {
            var mockIO = new Mock<IReadWrite>();
            var _random = new Mock<IRandom>()
            {
                CallBase = true,
            };

            var sectorCoords = new Coordinates(1, 1);
            var enterprise = new Enterprise(1000, sectorCoords, mockIO.Object, _random.Object);
            var functionMock = new Mock<GalaxyRegionMap>(mockIO.Object, new Mock<Galaxy>(_random.Object).Object);

            var computer = new LibraryComputer(mockIO.Object, functionMock.Object);
            enterprise.Add(computer);

            mockIO.Setup(io => io.ReadNumber("Computer active and waiting command")).Returns(0);
            enterprise.Execute(Command.COM);

            functionMock.Verify(f => f.Execute(enterprise.Quadrant), Times.Once());
        }
    }
}
