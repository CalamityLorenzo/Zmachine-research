﻿namespace Zmachine.V2.InstructionDefinitions
{
    internal record InstructionOperands(
        OperandType operandType,
        byte[] operand
    );
}
