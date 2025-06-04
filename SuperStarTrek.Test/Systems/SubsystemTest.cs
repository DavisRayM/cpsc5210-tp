using Games.Common.IO;
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
            internal override CommandResult ExecuteCommandCore(IQuadrant quadrant)
            {
                return CommandResult.Ok;
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
    }
}
