using System;
using Games.Common.IO;
using Games.Common.Randomness;
using Moq;
using NUnit.Framework;
using SuperStarTrek.Objects;
using SuperStarTrek.Space;
using SuperStarTrek.Systems;

namespace SuperStarTrek.Test.Systems
{
	public class ShieldControlTest
	{
        [Test]
        public void CanExecuteCommand_WhenOperational_ReturnsTrue()
        {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(10, new Coordinates(1, 1), mockIO.Object, mockRandom.Object);

            var shieldControl = new ShieldControl(mockEnterprise.Object, mockIO.Object);
            Assert.IsTrue(shieldControl.CanExecuteCommand());
        }


    }
}

