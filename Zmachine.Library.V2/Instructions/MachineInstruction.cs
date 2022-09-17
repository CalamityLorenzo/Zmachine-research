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
        public short Offset { get; init; }
        public bool BranchIfTrue { get; }
        public static Branch Empty => new Branch(new byte[] { }, false);

        public Branch(byte[] branchValue, bool branchIfTrue)
        {
            if (branchValue.Length == 0)
                this.Offset = 0;
            else if (branchValue.Length == 1)
                this.Offset = (short)((branchValue[0] & 0b00111111));
            else
            {
                ushort final = (ushort)((branchValue[0] & 0b00111111) << 8 | branchValue[1]);
                // Check the signbit on the 14bit number.
                if ((final & 0x2000) == 0x2000)
                    final |= 0xc000;

                this.Offset = (short)(final);

            }
            BranchIfTrue = branchIfTrue;
        }
    }

}
