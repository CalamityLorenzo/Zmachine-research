namespace Zmachine.Library.V2.Instructions
{
    [Flags]
    public enum OperandType
    {
        LargeConstant = 0,  // 0->65535
        SmallConstant = 1,  // 0->255
        Variable = 2,       // A Variable eg local/global/stack
        Omitted = 3         // NOTHING
    }

    public record InstructionOperands(
        OperandType operandType,
        byte[] operand
    );

    // Basic layout of an instruction.
    public record MachineInstruction(bool Store, bool Branch, string OpCode, int DecCode, FeaturesVersion Version, string Name) { }

}
