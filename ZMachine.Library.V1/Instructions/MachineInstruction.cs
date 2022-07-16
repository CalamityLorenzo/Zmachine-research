namespace ZMachine.Library.V1.Instructions
{
    [Flags]
    public enum OperandType
    {
        LargeConstant = 0,
        SmallConstant = 1,
        Variable =2,
        Omitted = 3
    }

    // Basic layout of an instruction.
    public record MachineInstruction(bool Store, bool Branch, string OpCode, int DecCode, FeaturesVersion Version, string Name) { }
  
}
