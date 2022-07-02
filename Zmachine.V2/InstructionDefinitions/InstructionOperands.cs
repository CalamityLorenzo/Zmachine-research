using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2.InstructionDefinitions
{
    internal record InstructionOperands(
    
        OperandType operandType,
        byte[] operand
    );
}
