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


            var currentAddress = firstByte switch
            {
                >= 0 and <= 0x1F => long2OP_CC(memory, startAddress),
                >= 0x20 and <= 0x3f => long2OP_CV(memory, startAddress),
                >= 0x40 and <= 0x5f => long2OP_VC(memory, startAddress),
                >= 0x60 and <= 0x7f => long2OP_VV(memory, startAddress),
                >= 0x80 and <= 0x8f => short1OP_LC(memory, startAddress),
                >= 0x90 and <= 0x9f => short1OP_C(memory, startAddress),
                >= 0xa0 and <= 0xaf => short1OP_V(memory, startAddress),
                >= 0xb0 and < 0xbe or 0xBf => short0OP(memory, startAddress),
                0xBE => be(memory, startAddress),
                >= 0xc0 and <= 0xdf => variable2OP(memory, startAddress),
                >= 0xe0 and <= 0xff => variableVAR(memory, startAddress),

            };
        }

        private int variableVAR(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, operandTypes, currentAddress) = variableInstruction(memory, startAddress);
            return currentAddress;
        }

        private int be(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }

        private int variable2OP(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, operandTypes,  currentAddress) = variableInstruction(memory, startAddress);
            return currentAddress;
        }


        private int short0OP(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress) = shortInstruction(memory, startAddress);
            return startAddress;
        }

        private int short1OP_V(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress) = shortInstruction(memory, startAddress);
            return startAddress;
        }

        private int short1OP_C(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress) = shortInstruction(memory, startAddress);
            return startAddress;
        }

        private int short1OP_LC(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress) = shortInstruction(memory, startAddress);
            return startAddress;
        }

        private int long2OP_VV(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress) = longInstruction(memory, startAddress);
            return startAddress;
        }

        private int long2OP_VC(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress) = longInstruction(memory, startAddress);
            return startAddress;

        }


        private int long2OP_CV(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");
            var (instruction, operands, currentAddress) = longInstruction(memory, startAddress);
            return startAddress;

            //return startAddress;

        }
        // long 2OP small constant, small Constant
        private int long2OP_CC(byte[] memory, int startAddress)
        {
            var startAddressInHex = startAddress.ToString("X");

            // The opcode fits in the bottom 5/FIVE/NOT FOUR bits
            var decimalInstruction = memory[startAddress] & 31;
            var address = startAddress + 1;
            //// Two Small Constant Operands (1 byte)
            var op1 = memory[startAddress += 1];
            var op2 = memory[startAddress += 1];


            var id = InstructionDefinitions.Instructions_2OP.Instructions.First(operand => operand.Name == $"2OP:${decimalInstruction}");

            var store = id.Store ? memory[startAddress += 1] : (byte)0;
            var branch = id.Branch ? memory[startAddress += 1] : (byte)0;


            var instruction = decimalInstruction switch
            {
                1 => je(new[] { op1, op2 }, branch),
                2 => jl(new[] { op1, op2 }, branch),
                3 => jg(new[] { op1, op2 }, branch),
                4 => dec_chk(new[] { op1, op2 }, branch),
                5 => inc_chk(new[] { op1, op2 }, branch),
                _ => ""
            };
            return address;
        }

        //long      2OP     small constant, variable
        private (InstructionDefinition instr, byte[] ops, int finalAddress) longInstruction(byte[] memory, int address)
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

            var id = InstructionDefinitions.Instructions_2OP.Instructions.First(operand => operand.Name == $"2OP:${decimalInstruction}");

            return (id, new[] { op1, op2 }, address);

        }

        private (InstructionDefinition instr, byte operand, int finalAddress) shortInstruction(byte[] memory, int address)
        {
            var topmostByte = memory[address];

            // The opcode fits in the bottom 4 bits
            var decimalInstruction = topmostByte & 15;

            var operand1Type = (memory[address] >> 4 & 3) == 1 ? zType.Omitted : zType.SmallConstant;

            var operand = operand1Type == zType.Omitted ? (byte)0 : memory[address += 1];


            var id = operand1Type == zType.Omitted ?
                                            InstructionDefinitions.Instructions_0OP.Instructions.First(operand => operand.Name == $"0OP:${decimalInstruction}") :
                                            InstructionDefinitions.Instructions_1OP.Instructions.First(operand => operand.Name == $"1OP:${decimalInstruction}");
            return (id, operand, address);
        }

        private (InstructionDefinition instr, byte[] operands, List<zType> operandTypes, int finalAddress) variableInstruction(byte[] memory, int address)
        {
            var topmostByte = memory[address];
            // if bit 5 is 0 then the count is 2OP;
            // else count is VAR.
            bool is2OP = (topmostByte >> 5 & 1) == 1;
            // The opcode number is given in the bottom 5 bits. 
            var decimalInstruction = topmostByte & 15;
            var id = InstructionDefinitions.Instructions_Var.Instructions.First(operand => operand.Name == $"VAR:${decimalInstruction}");

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

            }

            return (id, operandBytes.ToArray(), operandTypes, address);
            
        }

    }
}
