using Moq;
using NUnit.Framework;
using SuperStarTrek.Objects;
using Games.Common.Randomness;
using SuperStarTrek.Space;
using SuperStarTrek.Commands;
using SuperStarTrek.Systems;
using Games.Common.IO;

namespace SuperStarTrek.Test.Systems
{
    public class KlingonTests
    {
        private Mock<IRandom> _mockRandom;
        private Klingon _klingon;

        [SetUp]
        public void Setup()
        {
            _mockRandom = new Mock<IRandom>();


            _mockRandom.Setup(r => r.NextFloat()).Returns(0.5f);


            _klingon = new Klingon(new Coordinates(0, 0), _mockRandom.Object);
        }

        [Test]
        public void Constructor_SetsInitialEnergyBasedOnRandom()
        {

            Assert.That(_klingon.Energy, Is.EqualTo(200).Within(0.01));
        }

        [Test]
        public void MoveTo_UpdatesSector()
        {
            var newSector = new Coordinates(5, 7);
            _klingon.MoveTo(newSector);
            Assert.That(_klingon.Sector, Is.EqualTo(newSector));
        }

        [Test]
        public void TakeHit_HitGreaterThanThreshold_ReturnsTrueAndReducesEnergy()
        {
            float initialEnergy = _klingon.Energy;


            int hitStrength = (int)(0.2 * initialEnergy);

            bool result = _klingon.TakeHit(hitStrength);

            Assert.IsTrue(result);
            Assert.That(_klingon.Energy, Is.LessThan(initialEnergy));
        }

        [Test]
        public void TakeHit_HitLessThanThreshold_ReturnsFalseAndDoesNotReduceEnergy()
        {
            float initialEnergy = _klingon.Energy;


            int hitStrength = (int)(0.1 * initialEnergy);

            bool result = _klingon.TakeHit(hitStrength);

            Assert.IsFalse(result);
            Assert.That(_klingon.Energy, Is.EqualTo(initialEnergy));
        }
        [Test]
        public void Klingon_Fireon() {
            var mockIO = new Mock<IReadWrite>();
            var mockRandom = new Mock<IRandom>();
            var mockEnterprise = new Mock<Enterprise>(1, new Coordinates(0, 0), mockIO.Object, mockRandom.Object);
            var mockShield = new Mock<ShieldControl>(mockEnterprise.Object, mockIO.Object);
            mockEnterprise.Object.Add(mockShield.Object);
            mockEnterprise.Setup(e => e.TakeHit(It.IsAny<Coordinates>(), It.IsAny<int>())).Returns(CommandResult.Ok);
            TestContext.WriteLine(_klingon.FireOn(mockEnterprise.Object));
            Assert.That(_klingon.FireOn(mockEnterprise.Object), Is.EqualTo(CommandResult.Ok));


        }



    }
}