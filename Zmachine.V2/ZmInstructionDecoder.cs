using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zmachine.V2.InstructionDefinitions;

namespace Zmachine.V2
{
    // Decoding instructions from the source.
    // And the terrible, terrible way it was originally encoded
    internal partial class ZmInstructionDecoder
    {
        private int version;
        public void Decode(byte[] memory, int startAddress, int version)
        {
            this.version = version;
            Console.WriteLine($"Starting at {startAddress} : {startAddress.ToString("X")}");
            // Translate first byte to work out which table we are looking at.

            var firstByte = memory[startAddress];
            // Each instruction falls into one of these groups, This is just making it easier to process them.
            // I think.
            var currentAddress = startAddress;
            while (currentAddress < memory.Length)
            {
                Console.WriteLine(currentAddress.ToString("X"));

                currentAddress = firstByte switch
                {
                    >= 0 and <= 0x1F => long2OP_CC(memory, currentAddress),
                    >= 0x20 and <= 0x3f => long2OP_CV(memory, currentAddress),
                    >= 0x40 and <= 0x5f => long2OP_VC(memory, currentAddress),
                    >= 0x60 and <= 0x7f => long2OP_VV(memory, currentAddress),
                    >= 0x80 and <= 0x8f => short1OP_LC(memory, currentAddress),
                    >= 0x90 and <= 0x9f => short1OP_C(memory, currentAddress),
                    >= 0xa0 and <= 0xaf => short1OP_V(memory, currentAddress),
                    >= 0xb0 and < 0xbe or 0xBf => short0OP(memory, currentAddress),
                    0xBE => be(memory, currentAddress),
                    >= 0xc0 and <= 0xdf => variable2OP(memory, currentAddress),
                    >= 0xe0 and <= 0xff => variableVAR(memory, currentAddress),

                };

                firstByte = memory[currentAddress += 1];

            }
        }

        private int variableVAR(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, operandTypes, currentAddress, store, branch)  = variableInstruction(memory, ref startAddress);
            return currentAddress;
        }

        private int be(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }

        private int variable2OP(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, operandTypes, currentAddress, store, branch) = variableInstruction(memory, ref startAddress);
            return currentAddress;
        }


        private int short0OP(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress, store, branch) = shortInstruction(memory, ref startAddress);
            return currentAddress;
        }

        private int short1OP_V(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress, store, branch)  = shortInstruction(memory, ref startAddress);
            return currentAddress;
        }

        private int short1OP_C(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress, store, branch)  = shortInstruction(memory, ref startAddress);
            return currentAddress;
        }

        private int short1OP_LC(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress, store, branch)  = shortInstruction(memory, ref startAddress);
            return currentAddress;
        }

        private int long2OP_VV(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress, store, branch) = longInstruction(memory, ref startAddress);
            return currentAddress;
        }

        private int long2OP_VC(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress, store, branch) = longInstruction(memory, ref startAddress);
            return currentAddress;

        }


        private int long2OP_CV(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress, store, branch) = longInstruction(memory, ref startAddress);
            return currentAddress;

            //return startAddress;

        }
        // long 2OP small constant, small Constant
        private int long2OP_CC(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress, store, branch) = longInstruction(memory, ref startAddress);

            //var store = instruction.Store ? memory[currentAddress += 1] : 0;
            //var branch = instruction.Branch ? memory[currentAddress += 2] : 0;

            return currentAddress;

        }

        //long      2OP     small constant, variable
        private (InstructionDefinition instr, byte[] ops, int finalAddress, byte store, byte[] branch) longInstruction(byte[] memory, ref int address)
        {
            var topmostByte = memory[address];

            // The opcode fits in the bottom 5/FIVE/NOT FOUR bits
            var decimalInstruction = topmostByte & 31;
            // Opreands are kept in bits 6 & 5

            var operand1Type = (memory[address] >> 6 & 1) == 1 ? zType.Variable : zType.SmallConstant;
            var operand2Type = (memory[address] >> 5 & 1) == 1 ? zType.Variable : zType.SmallConstant;

            // Small Constnat Operands (1 byte)
            // Variable Operands (1 byte)
            var op1 = memory[address += 1];
            var op2 = memory[address += 1];

            var instruction = InstructionDefinitions.Instructions_2OP.Instructions.First(operand => operand.OpCode == $"2OP:{decimalInstruction}");
            byte store = instruction.Store ? memory[address += 1] : (byte)0;
            // remember we are not dealing with the offset at all here
            var branch = GetBranch(memory, ref address, instruction.Branch);
            return (instruction, new[] { op1, op2 }, address, store, branch);

        }

        private (InstructionDefinition instr, byte[] operand, int finalAddress, byte store, byte[] branch) shortInstruction(byte[] memory, ref int address)
        {
            var topmostByte = memory[address];

            // The opcode fits in the bottom 4 bits
            var decimalInstruction = topmostByte;

            var operand1Type = (zType)(memory[address] >> 4 & 3);

            byte[] operand = operand1Type switch
            {
                zType.LargeConstant => new[] { (memory[address += 1]), (memory[address += 1]) },
                zType.SmallConstant => new[] { memory[address += 1] },
                zType.Omitted => new byte[] { 0 },
                zType.Variable => new[] { memory[address +=  1] }
            };


            var instruction = operand1Type == zType.Omitted ?
                                            InstructionDefinitions.Instructions_0OP.Instructions.First(operand => operand.OpCode == $"0OP:{decimalInstruction}") :
                                            InstructionDefinitions.Instructions_1OP.Instructions.First(operand => operand.OpCode == $"1OP:{decimalInstruction}");
            byte store = instruction.Store ? memory[address += 1] : (byte)0;
            // remember we are not dealing with the offset at all here
            var branch = GetBranch(memory, ref address, instruction.Branch);

            return (instruction, operand, address, store, branch);
        }

        private (InstructionDefinition instr, byte[] operands, List<zType> operandTypes, int finalAddress, byte store, byte[] branch) variableInstruction(byte[] memory, ref int address)
        {
            var topmostByte = memory[address];
            // if bit 5 is 0 then the count is 2OP;
            // else count is VAR.
            bool is2OP = (topmostByte >> 5 & 1) == 0;
            // The opcode number is given in the bottom 5 bits. 
            var decimalInstruction = is2OP ? topmostByte & 31 : topmostByte;
            //var form = is2OP? "2OP": "VAR";
            var instruction = is2OP
                                ? InstructionDefinitions.Instructions_2OP.Instructions.First(operand => operand.OpCode == $"2OP:{decimalInstruction}")
                                : InstructionDefinitions.Instructions_Var.Instructions.First(operand => operand.OpCode == $"VAR:{decimalInstruction}");

            var operandTypeByte = memory[address += 1];

            //loop though the operandtypes, until 
            // a. reach the end
            // b. hit an omitted value (11)

            // types are stored 2 bits and r->l so 6 = op1 4 = op2 etc
            // we also collect the values as we go.
            var shift = 6;
            List<zType> operandTypes = new();
            List<byte> operandBytes = new List<byte>();
            while (shift >= 0)
            {
                var current = (zType)(operandTypeByte >> shift & 3);
                if (current == zType.Omitted)
                {
                    break;
                }
                operandTypes.Add(current);
                if (zType.LargeConstant == current)
                {
                    operandBytes.Add(memory[address += 1]);
                    operandBytes.Add(memory[address += 1]);
                }
                else
                {
                    operandBytes.Add(memory[address += 1]);
                }

                shift -= 2;

            }

            byte store = instruction.Store ? memory[address += 1] : (byte)0;
            // remember we are not dealing with the offset at all here
            var branch = GetBranch(memory, ref address, instruction.Branch);


            return (instruction, operandBytes.ToArray(), operandTypes, address, store, branch);

        }


        private byte[] GetBranch(byte[] memory, ref int address, bool hasBranch)
        {

            if (hasBranch)
            {
                byte[] branch;
                // is it one byte only
                var branchByte = memory[address += 1];
                if (branchByte >> 6 == 1) branch = new[] { branchByte };
                else branch = new[] { branchByte, memory[address += 1] };
                return branch;
            }
            else
            {
                return new byte[0];
            }

        }
    }
}
