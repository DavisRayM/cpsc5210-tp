using Games.Common.IO;
using SuperStarTrek.Commands;
using SuperStarTrek.Space;
using SuperStarTrek.Systems.ComputerFunctions;

namespace SuperStarTrek.Systems;

internal class LibraryComputer : Subsystem
{
    internal readonly IReadWrite _io;
    internal readonly ComputerFunction[] _functions;

    internal LibraryComputer(IReadWrite io, params ComputerFunction[] functions)
        : base("Library-Computer", Command.COM, io)
    {
        _io = io;
        _functions = functions;
    }

    internal override bool CanExecuteCommand() => IsOperational("Computer disabled");

    internal override CommandResult ExecuteCommandCore(IQuadrant quadrant)
    {
        var index = GetFunctionIndex();
        _io.WriteLine();

        _functions[index].Execute(quadrant);

        return CommandResult.Ok;
    }

    internal int GetFunctionIndex()
    {
        while (true)
        {
            var index = (int)_io.ReadNumber("Computer active and waiting command");
            if (index >= 0 && index <= 5) { return index; }

            for (int i = 0; i < _functions.Length; i++)
            {
                _io.WriteLine($"   {i} = {_functions[i].Description}");
            }
        }
    }
}
