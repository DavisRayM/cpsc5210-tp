using Games.Common.IO;
using Moq;
using SuperStarTrek.Commands;
using SuperStarTrek.Space;
using SuperStarTrek.Systems;

namespace SuperStarTrek.Test.Systems
{
    public class SubsystemTest
    {
        // implementation of Subsystem to allow testing of it
        internal class SubsystemTestable(string name, Command command, IReadWrite io)
            : Subsystem(name, command, io)
        {
            public bool _returnValueForCanExecuteCommand;
            public bool _ExecuteCommandCoreCalled = false;
            public IQuadrant? _quadrantCaptured;

            internal override bool CanExecuteCommand()
            {
                return _returnValueForCanExecuteCommand;
            }

            internal override CommandResult ExecuteCommandCore(IQuadrant quadrant)
            {
                _ExecuteCommandCoreCalled = true;
                _quadrantCaptured = quadrant;
                
                return CommandResult.GameOver;
            }
        }

        IOSpy _ioSpy;
        SubsystemTestable testSubsystem;

        [SetUp]
        public void Setup()
        {
            _ioSpy = new();
            testSubsystem = new("", Command.XXX, _ioSpy);
        }

        #region ExecuteCommand

        [Test]
        public void Subsystem_ExecuteCommand_When_CanExecuteCommand_IsFalse_Should_Return_OK()
        {
            testSubsystem._returnValueForCanExecuteCommand = false;
            Mock<IQuadrant> quadrantMock = new();
            Assert.That(testSubsystem.ExecuteCommand(quadrantMock.Object),
                        Is.EqualTo(CommandResult.Ok)); 
        }

        [Test]
        public void Subsystem_ExecuteCommand_When_CanExecuteCommand_IsFalse_Should_Not_Call_ExecuteCommmandCore()
        {
            testSubsystem._returnValueForCanExecuteCommand = false;
            Mock<IQuadrant> quadrantMock = new();
            testSubsystem.ExecuteCommand(quadrantMock.Object);
            Assert.That(testSubsystem._ExecuteCommandCoreCalled, Is.False); 
        }

        [Test]
        public void Subsystem_ExecuteCommand_When_CanExecuteCommand_IsTrue_Should_Call_ExecuteCommmandCore()
        {
            testSubsystem._returnValueForCanExecuteCommand = true;
            Mock<IQuadrant> quadrantMock = new();
            testSubsystem.ExecuteCommand(quadrantMock.Object);
            Assert.That(testSubsystem._ExecuteCommandCoreCalled); 
        }

        [Test]
        public void Subsystem_ExecuteCommand_When_CanExecuteCommand_IsTrue_Should_Send_Quadrant_To_ExecuteCommmandCore()
        {
            testSubsystem._returnValueForCanExecuteCommand = true;
            Mock<IQuadrant> quadrantMock = new();
            testSubsystem.ExecuteCommand(quadrantMock.Object);
            Assert.That(testSubsystem._quadrantCaptured, Is.EqualTo(quadrantMock.Object)); 
        }

        #endregion ExecuteCommand

        #region Repair

        [Test]
        [TestCase(0.1f, 0.05f, ExpectedResult = -0.1f)]
        [TestCase(1f, 0.5f, ExpectedResult = -0.5f)]
        [TestCase(1f, 1.5f, ExpectedResult = 0.5f)]
        [TestCase(0f, 0.5f, ExpectedResult = 0f)]
        public float Subsystem_Repair_WithWorkDone_Should_Correctly_Set_Condition(
            float damage,
            float repairWorkDone
        )
        {
            testSubsystem.TakeDamage(damage);
            testSubsystem.Repair(repairWorkDone);
            return testSubsystem.Condition;
        }

        [Test]
        [TestCase(0.1f, 0.05f, ExpectedResult = false)]
        [TestCase(1f, 0.5f, ExpectedResult = false)]
        [TestCase(1f, 1.5f, ExpectedResult = true)]
        [TestCase(0f, 0.5f, ExpectedResult = true)]
        public bool Subsystem_Repair_WithWorkDone_Should_Return_Corect_Value(
            float damage,
            float repairWorkDone
        )
        {
            testSubsystem.TakeDamage(damage);
            return testSubsystem.Repair(repairWorkDone);
        }

        #endregion Repair
    }
}
