namespace Zmachine.Library.V2.Instructions
{
    // This gives us most of the context requried to decode any instruction.
    public record DecodedInstruction(MachineInstruction instruction
                                      , Operand[] operands
                                      , byte store
                                      , Branch branch,
                                        int startAddress,
                                        int endAddress,
                                        string hexBytes)
    {
        public string startAddressHex => this.startAddress.ToString("X4");
        public string endAddressHex => this.startAddress.ToString("X4");
    }

}
