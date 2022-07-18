namespace ZMachine.Library.V1.Instructions
{
    // This gives us most of the context requried to decode any instruction.
    internal record  DecodedInstruction(MachineInstruction instruction
                                      , InstructionOperands[] operands
                                      , byte store
                                      , byte[] branch,
                                        string startAddress,
                                        string hexBytes);
    
}
