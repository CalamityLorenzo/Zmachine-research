using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2.InstructionDefinitions
{
    internal enum MinVersion
    {
        NotFound =0,
        All=1,
        Five = 5,
        Six=6,
        Seven=7,
        FiveOrThree,
        SixOr,
        OneOnly,
        FiveOnly,
        FourOnly,
        FourOrSix,
        FiveOrSix,
        SixOnly,
    }

    internal record InstructionDefinition(bool Store, bool Branch, string OpCode, int DecCode, MinVersion Version, string Name);
}
