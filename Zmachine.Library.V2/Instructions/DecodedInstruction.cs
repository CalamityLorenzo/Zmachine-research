namespace Zmachine.Library.V2.Instructions
{
    // This gives us most of the context requried to decode any instruction.
    public record DecodedInstruction(MachineInstruction instruction
                                      , InstructionOperands[] operands
                                      , byte store
                                      , byte[] branch,
                                        int startAddress,
                                        int endAddress,
                                        string hexBytes);

}
