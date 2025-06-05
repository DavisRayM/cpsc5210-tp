using SuperStarTrek.Commands;
using SuperStarTrek.Objects;
using System.Collections.Generic;

namespace SuperStarTrek.Space
{
    internal interface IQuadrant
    {
        Coordinates Coordinates { get; }
        bool HasKlingons { get; }
        int KlingonCount { get; }
        bool HasStarbase { get; }
        Starbase Starbase { get; }
        IEnumerable<Klingon> Klingons { get; }

        void Display(string textFormat);
        bool TorpedoCollisionAt(Coordinates coordinates, out string message, out bool gameOver);
        string Remove(Klingon klingon);
        CommandResult KlingonsMoveAndFire();
        CommandResult KlingonsFireOnEnterprise();
        IEnumerable<string> GetDisplayLines();
    }
}
