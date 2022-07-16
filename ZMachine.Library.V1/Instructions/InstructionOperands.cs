namespace ZMachine.Library.V1.Instructions
{
    internal record InstructionOperands(
        OperandType operandType,
        byte[] operand
    );
}
