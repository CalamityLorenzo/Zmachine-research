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
        Three,
        Four=4,
        Five = 5,
        Six=6,
        Seven=7,
        SixOr,
        ThreeOnly,
        OneOnly,
        FiveOnly,
        FourOnly,
        FourOrSix,
        FiveOrSix,
        SixOnly,
        OneToFour,
    }

    [Flags]
    internal enum ZMachineVersion
    {
        None = 0,
        One =1,
        Two =2,
        Three=4,
        Four=8,
        Five=16,
        Six=32,
        Seven=64,
        Eight=128,
        UpToFour = One | Two | Three | Four,
        FiveAndUp = Five | Six | Seven | Eight,
        SixAndUp =  Six | Seven | Eight,
        All = One | Two | Three | Four | Five | Six | Seven | Eight

    }

    [Flags]
    internal enum OperandType
    {
        LargeConstant = 0,
        SmallConstant = 1,
        Variable =2,
        Omitted = 3
    }

    // Basic layout of an instruction.
    internal record InstructionDefinition(bool Store, bool Branch, string OpCode, int DecCode, ZMachineVersion Version, string Name);
}
