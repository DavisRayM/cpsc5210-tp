using Games.Common.IO;
using SuperStarTrek.Space;

namespace SuperStarTrek.Systems.ComputerFunctions;

internal abstract class ComputerFunction
{
    protected ComputerFunction(string description, IReadWrite io)
    {
        Description = description;
        IO = io;
    }

    internal string Description { get; }

    internal IReadWrite IO { get; }

    internal abstract void Execute(IQuadrant quadrant);
}
