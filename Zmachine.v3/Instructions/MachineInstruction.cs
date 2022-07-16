using Zmachine.V3.Machines;

namespace Zmachine.V3.Instructions
{
    [Flags]
    internal enum OperandType
    {
        LargeConstant = 0,
        SmallConstant = 1,
        Variable =2,
        Omitted = 3
    }

    // Basic layout of an instruction.
    internal record MachineInstruction(bool Store, bool Branch, string OpCode, int DecCode, MachineVersion Version, string Name) { }
  
}
