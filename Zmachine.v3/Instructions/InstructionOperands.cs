namespace Zmachine.V3.Instructions
{
    internal record InstructionOperands(
        OperandType operandType,
        byte[] operand
    );
}
