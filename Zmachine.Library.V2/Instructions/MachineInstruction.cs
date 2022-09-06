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

    public record Operand(
        OperandType operandType,
        byte[] value
    );

    // Basic layout of an instruction.
    public record MachineInstruction(bool Store, bool Branch, string OpCode, int DecCode, FeaturesVersion Version, string Name) { }

    public record Branch
    {
        public ushort Offset { get; init; }
        public bool BranchIfTrue { get; }
        public static Branch Empty => new Branch(new byte[] { }, false);

        public Branch(byte[] branchValue, bool branchIfTrue)
        {
            if (branchValue.Length == 0)
                this.Offset = 0;
            else if (branchValue.Length == 1)
                this.Offset = (ushort) (branchValue[0] & 0b00111111); 
            else
                this.Offset = (ushort)( (branchValue[0] & 0b11111) << 8 | branchValue[1]);
            BranchIfTrue = branchIfTrue;
        }
    }

}
