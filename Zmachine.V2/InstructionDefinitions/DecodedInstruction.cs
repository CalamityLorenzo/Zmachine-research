namespace Zmachine.V2.InstructionDefinitions
{
    // This gives us most of the context requried to decode any instruction.
    internal record  DecodedInstruction(InstructionDefinition instruction
                                      , byte[] operands
                                      , List<OperandType> operandTypes
                                      , byte store
                                      , byte[] branch,
                                        string startAddress,
                                        string hexBytes);
    
}
