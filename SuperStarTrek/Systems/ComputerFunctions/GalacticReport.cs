using System.Collections.Generic;
using System.Linq;
using Games.Common.IO;
using SuperStarTrek.Space;

namespace SuperStarTrek.Systems.ComputerFunctions;

internal abstract class GalacticReport : ComputerFunction
{
    internal GalacticReport(string description, IReadWrite io, Galaxy galaxy)
        : base(description, io)
    {
        Galaxy = galaxy;
    }

    internal Galaxy Galaxy { get; }

    protected abstract void WriteHeader(IQuadrant quadrant);

    protected abstract IEnumerable<string> GetRowData();

    internal override void Execute(IQuadrant quadrant)
    {
        WriteHeader(quadrant);
        IO.WriteLine("       1     2     3     4     5     6     7     8");
        IO.WriteLine("     ----- ----- ----- ----- ----- ----- ----- -----");

        foreach (var (row, index) in GetRowData().Select((r, i) => (r, i)))
        {
            IO.WriteLine($" {index + 1}   {row}");
            IO.WriteLine("     ----- ----- ----- ----- ----- ----- ----- -----");
        }
    }
}
