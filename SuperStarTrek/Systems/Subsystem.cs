using Games.Common.IO;
using SuperStarTrek.Commands;
using SuperStarTrek.Space;

namespace SuperStarTrek.Systems;

internal abstract class Subsystem
{
    internal readonly IReadWrite _io;

    protected Subsystem(string name, Command command, IReadWrite io)
    {
        Name = name;
        Command = command;
        Condition = 0;
        _io = io;
    }

    internal string Name { get; }

    internal virtual float Condition { get; private set; }

    internal bool IsDamaged => Condition < 0;

    internal Command Command { get; }

    internal virtual bool CanExecuteCommand() => true;

    protected bool IsOperational(string notOperationalMessage)
    {
        if (IsDamaged)
        {
            _io.WriteLine(notOperationalMessage.Replace("{name}", Name));
            return false;
        }

        return true;
    }

    internal CommandResult ExecuteCommand(IQuadrant quadrant)
        => CanExecuteCommand() ? ExecuteCommandCore(quadrant) : CommandResult.Ok;

    internal abstract CommandResult ExecuteCommandCore(IQuadrant quadrant);

    internal virtual void Repair()
    {
        if (IsDamaged)
        {
            Condition = 0;
        }
    }

    internal virtual bool Repair(float repairWorkDone)
    {
        if (IsDamaged)
        {
            Condition += repairWorkDone;
            if (Condition > -0.1f && Condition < 0)
            {
                Condition = -0.1f;
            }
        }

        return !IsDamaged;
    }

    internal void TakeDamage(float damage) => Condition -= damage;
}
