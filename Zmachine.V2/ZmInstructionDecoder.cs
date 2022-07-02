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
        private ZMachineVersion instructionVersion;
        private int version;
        private Instructions instructions;

        public ZmInstructionDecoder(Instructions instructions)
        {
            this.instructions = instructions;
        }

        public void Decode(byte[] memory, int startAddress, int version)
        {
            this.instructionVersion = MachineExtensions.GetZMachineVersion(version);

            Console.WriteLine($"Starting at {startAddress} : {startAddress.ToString("X")}");
            // Translate first byte to work out which table we are looking at.

            var firstByte = memory[startAddress];
            // Each instruction falls into one of these groups, This is just making it easier to process them.
            // I think.
            var currentAddress = startAddress;
            while (currentAddress < memory.Length)
            {
                //Console.WriteLine(currentAddress.ToString("X"));

                var instruction = firstByte switch
                {
                    >= 0 and <= 0x1F => longInstruction(memory, ref currentAddress),
                    >= 0x20 and <= 0x3f => longInstruction(memory, ref currentAddress),
                    >= 0x40 and <= 0x5f => longInstruction(memory, ref currentAddress),
                    >= 0x60 and <= 0x7f => longInstruction(memory, ref currentAddress),
                    >= 0x80 and <= 0x8f => shortInstruction(memory, ref currentAddress),
                    >= 0x90 and <= 0x9f => shortInstruction(memory, ref currentAddress),
                    >= 0xa0 and <= 0xaf => shortInstruction(memory, ref currentAddress),
                    >= 0xb0 and < 0xbe or 0xBf => shortInstruction(memory, ref currentAddress),
                    0xBE => this.version == 5 ? extendedInstruction(memory, ref currentAddress) : longInstruction(memory, ref currentAddress),
                    >= 0xc0 and <= 0xdf => variableInstruction(memory, ref currentAddress),
                    >= 0xe0 and <= 0xff => variableInstruction(memory, ref currentAddress),

                };
                Console.WriteLine($"${instruction.startAddress} : {instruction.instruction.Name} {instruction.hexBytes}");
                firstByte = memory[currentAddress += 1];

            }
        }





        //long      2OP     small constant, variable
        private DecodedInstruction longInstruction(byte[] memory, ref int address)
        {
            var instructionStartAddress = address;
            var topmostByte = memory[address];

            // The opcode fits in the bottom 5/FIVE/NOT FOUR bits
            var decimalInstruction = topmostByte & 31;
            // Opreands are kept in bits 6 & 5

            var operand1Type = (memory[address] >> 6 & 1) == 1 ? OperandType.Variable : OperandType.SmallConstant;
            var operand2Type = (memory[address] >> 5 & 1) == 1 ? OperandType.Variable : OperandType.SmallConstant;

            // Small Constnat Operands (1 byte)
            // Variable Operands (1 byte)
            var op1 = GetOperandFromType(operand1Type, memory, ref address);
            var op2 = GetOperandFromType(operand2Type, memory, ref address); ;

            var instruction = instructions.GetInstruction($"2OP:{decimalInstruction}");
            byte store = instruction.Store ? memory[address += 1] : (byte)0;
            // remember we are not dealing with the offset at all here
            var branch = GetBranch(memory, ref address, instruction.Branch);

            var allBytes = GetHexAddressRange(instructionStartAddress, address, memory);
            return new DecodedInstruction(instruction, op1.Concat(op2).ToArray(), new() { operand1Type, operand2Type }, store, branch, instructionStartAddress.ToString("X"), allBytes);


        }

        private DecodedInstruction shortInstruction(byte[] memory, ref int address)
        {
            var instructionStartAddress = address;
            var topmostByte = memory[address];

            // The opcode fits in the bottom 4 bits
            var decimalInstruction = instructionModulo(topmostByte, 128, 16);

            var operand1Type = (OperandType)(memory[address] >> 4 & 3);

            byte[] operand = GetOperandFromType(operand1Type, memory, ref address);


            var instruction = operand1Type == OperandType.Omitted ?
                                            instructions.GetInstruction($"0OP:{decimalInstruction}") :
                                            instructions.GetInstruction($"1OP:{decimalInstruction}");
            byte store = instruction.Store ? memory[address += 1] : (byte)0;
            // remember we are not dealing with the offset at all here
            var branch = GetBranch(memory, ref address, instruction.Branch);

            var allBytes = GetHexAddressRange(instructionStartAddress, address, memory);

            return new DecodedInstruction(instruction, operand, new() { operand1Type }, store, branch, instructionStartAddress.ToString("X"), allBytes);

        }

        int instructionModulo(int instructionByte, int lowestRange, int rangeSize)
        {
            if (instructionByte> lowestRange + rangeSize)
            {
                var difference = instructionByte - lowestRange;
                if (difference > rangeSize)
                {
                    difference =  difference% rangeSize;

                }
                return lowestRange + difference;
            }
            return instructionByte;
        }

        private DecodedInstruction extendedInstruction(byte[] memory, ref int address)
        {
            var instructionStartAddress = address;
            var decimalInstruction = memory[address += 1];



            var instruction = instructions.GetInstruction($"EXT:{decimalInstruction}");
            var operandTypes = memory[address += 1];

            byte otherOperandTypes = 0;
            //In the special case of the "double variable" VAR opcodes call_vs2 and call_vn2 (opcode numbers 12 and 26), a second byte of types is given, containing the types for the next four operands. 
            if (instruction.Name == "call_vn2" || instruction.Name == "call_vs2")
            {
                otherOperandTypes = memory[address += 1];
            }

            var operands = GetOperandAndType(operandTypes, memory, ref address);
            if (otherOperandTypes != 0 && otherOperandTypes != 3) // omitted type{
            {
                var otherOperands = GetOperandAndType(otherOperandTypes, memory, ref address);
                operands = operands.Concat(otherOperands).ToList();
            }


            byte store = instruction.Store ? memory[address += 1] : (byte)0;
            // remember we are not dealing with the offset at all here
            var branch = GetBranch(memory, ref address, instruction.Branch);
            var allBytes = GetHexAddressRange(instructionStartAddress, address, memory);
            return new DecodedInstruction(instruction, operands.SelectMany(a=>a.operand).ToArray(), operands.Select(a => a.operandType).ToList(), store, branch, instructionStartAddress.ToString("X"), allBytes);

        }

        private DecodedInstruction variableInstruction(byte[] memory, ref int address)
        {

            var instructionStartAddress = address;
            var topmostByte = memory[address];
            // if bit 5 is 0 then the count is 2OP;
            // else count is VAR.
            bool is2OP = (topmostByte >> 5 & 1) == 0;
            // The opcode number is given in the bottom 5 bits. 
            var decimalInstruction = is2OP ? topmostByte & 31 : topmostByte;
            //var form = is2OP? "2OP": "VAR";
            var instruction = is2OP 
                                ? instructions.GetInstruction($"2OP:{decimalInstruction}")
                                : instructions.GetInstruction($"VAR:{decimalInstruction}");

            var operandTypeByte = memory[address += 1];

            var operands = GetOperandAndType(operandTypeByte, memory, ref address);

            byte store = instruction.Store ? memory[address += 1] : (byte)0;
            // remember we are not dealing with the offset at all here
            var branch = GetBranch(memory, ref address, instruction.Branch);

            var allBytes = GetHexAddressRange(instructionStartAddress, address, memory);


            return new DecodedInstruction(instruction, operands.SelectMany(a => a.operand).ToArray(), operands.Select(a => a.operandType).ToList(), store, branch, instructionStartAddress.ToString("X"), allBytes);

        }

        private List<InstructionOperands> GetOperandAndType(byte operandTypeByte, byte[] memory, ref int address)
        {
            //loop though the operandtypes, until 
            // a. reach the end
            // b. hit an omitted value (11)

            // types are stored 2 bits and r->l so 6 = op1 4 = op2 etc
            // we also collect the values as we go.

            var instrOperands = new List<InstructionOperands>();
            var shift = 6;
            while (shift >= 0)
            {
                List<byte> operandBytes = new List<byte>();
                var current = (OperandType)(operandTypeByte >> shift & 3);
                if (current == OperandType.Omitted)
                {
                    break;
                }
                if (OperandType.LargeConstant == current)
                {
                    operandBytes.Add(memory[address += 1]);
                    operandBytes.Add(memory[address += 1]);
                }
                else
                {
                    operandBytes.Add(memory[address += 1]);
                }

                instrOperands.Add(new(current, operandBytes.ToArray()));
                shift -= 2;

            }

            return instrOperands;
        }

        private byte[] GetBranch(byte[] memory, ref int address, bool hasBranch)
        {

            if (hasBranch)
            {
                byte[] branch;
                // is it one byte only
                var branchByte = memory[address += 1];
                if ((branchByte >> 6 &1) == 1) branch = new[] { branchByte };
                else branch = new[] { branchByte, memory[address += 1] };
                return branch;
            }
            else
            {
                return new byte[0];
            }

        }

        private string GetHexAddressRange(int startAddress, int endAddress, byte[] memory) => String.Join(" ", memory.Skip(startAddress).Take(endAddress - startAddress).Select(itm => itm.ToString("X")));

        // 4.2 http://inform-fiction.org/zmachine/standards/z1point1/sect04.html
        private byte[] GetOperandFromType(OperandType operandType, byte[] memory, ref int address) => operandType switch
        {
            OperandType.LargeConstant => new[] { (memory[address += 1]), (memory[address += 1]) },
            OperandType.SmallConstant => new[] { memory[address += 1] },
            OperandType.Omitted => new byte[] { 0 },
            OperandType.Variable => new[] { memory[address += 1] }
        };

    }
}
