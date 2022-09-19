using System.Diagnostics;
using Zmachine.Library.V2.Instructions;

namespace Zmachine.Library.V2
{
    // Decoding instructions from the source.
    // And the terrible, terrible way it was originally encoded
    public partial class InstructionDecoder
    {
        private int version;
        private Dictionary<string, MachineInstruction> instructions;

        public InstructionDecoder(Dictionary<string, MachineInstruction> instructions, int version)
        {
            this.instructions = instructions;
            this.version = version;
        }

        /// <summary>
        /// Reads the next instruction.
        /// </summary>
        /// <param name="memory">Bag of memory</param>
        /// <param name="startAddress">The startingaddress</param>
        /// <returns>Decoded Instruction with operands.</returns>
        public DecodedInstruction Decode(byte[] memory, ref int startAddress)
        {
            //Console.WriteLine($"Starting at {startAddress} : {startAddress.ToString("X")}");

            // Translate first byte to work out which table we are looking at.
            var firstByte = memory[startAddress];
            var currentAddress = startAddress;
            // Current address in hex.
            var hexAddress = "";
            try
            {
                hexAddress = $"{currentAddress:X4}";
                // 4. Remarks:On disassembly
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
                startAddress = currentAddress;
                return instruction;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception : " + hexAddress);
                throw;
            }

        }

        //long      2OP     small constant, variable
        private DecodedInstruction longInstruction(byte[] memory, ref int address)
        {
            var instructionStartAddress = address;
            var topmostByte = memory[address];

            // The opcode fits in the bottom 5/FIVE
            var decimalInstruction = topmostByte & 31;
            // Opreands are kept in bits 6 & 5
            var operand1Type = (memory[address] >> 6 & 1) == 1 ? OperandType.Variable : OperandType.SmallConstant;
            var operand2Type = (memory[address] >> 5 & 1) == 1 ? OperandType.Variable : OperandType.SmallConstant;

            // Small Constnat Operands (1 byte)
            // Variable Operands (1 byte)
            var operandInfo = new List<Operand>
            {
               new Operand( operand1Type, GetOperandFromType(operand1Type, memory, ref address)),
               new Operand(operand2Type, GetOperandFromType(operand2Type, memory, ref address))
            }.ToArray();

            var instruction = instructions[$"2OP:{decimalInstruction}"];
            byte store = instruction.Store ? memory[address += 1] : (byte)0;
            // remember we are not dealing with the offset at all here
            var branch = GetBranch(memory, ref address, instruction.Branch);

            var allBytes = GetHexAddressRange(instructionStartAddress, address, memory);
            return new DecodedInstruction(instruction, operandInfo, store, branch, instructionStartAddress, address, allBytes);
        }

        private DecodedInstruction shortInstruction(byte[] memory, ref int address)
        {
            var instructionStartAddress = address;
            var topmostByte = memory[address];
            var operand1Type = (OperandType)(memory[address] >> 4 & 3);
            byte[] operand = GetOperandFromType(operand1Type, memory, ref address);
            // The opcode fits in the bottom 4 bits
            var decimalInstruction = InstructionModulo(topmostByte, operand1Type != OperandType.Omitted ? 128 : 176, 16);
            var instruction = operand1Type == OperandType.Omitted ?
                                            instructions[$"0OP:{decimalInstruction}"] :
                                            instructions[$"1OP:{decimalInstruction}"];

            byte store = instruction.Store ? memory[address += 1] : (byte)0;
            // remember we are not dealing with the offset at all here
            var branch = GetBranch(memory, ref address, instruction.Branch);

            if (instruction.Name == "print" || instruction.Name == "print_ret")
            {
                address += 1;
                //address == 0xd5f1
                // we have hit a natural 6 need to 
                var zChars = this.LiteralStringBytes(memory, ref address);
                operand = zChars;
            }

            var allBytes = GetHexAddressRange(instructionStartAddress, address, memory);

            return new DecodedInstruction(instruction, new[] { new Operand(value: operand, operandType: operand1Type) }, store, branch, instructionStartAddress, address, $"{allBytes}");

        }

        private DecodedInstruction extendedInstruction(byte[] memory, ref int address)
        {
            var instructionStartAddress = address;
            var decimalInstruction = memory[address += 1];
            var instruction = instructions[$"EXT:{decimalInstruction}"];
            var operandTypes = memory[address += 1];

            byte otherOperandTypes = 0;
            // In the special case of the "double variable" VAR opcodes call_vs2 and call_vn2 (opcode numbers 12 and 26),
            // a second byte of types is given, containing the types for the next four operands. 
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
            return new DecodedInstruction(instruction, operands.ToArray(), store, branch, instructionStartAddress, address, allBytes);

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
                                ? instructions[$"2OP:{decimalInstruction}"]
                                : instructions[$"VAR:{decimalInstruction}"];

            var operandTypeByte = memory[address += 1];

            var operands = GetOperandAndType(operandTypeByte, memory, ref address);

            byte store = instruction.Store ? memory[address += 1] : (byte)0;
            // remember we are not dealing with the offset at all here
            var branch = GetBranch(memory, ref address, instruction.Branch);

            var allBytes = GetHexAddressRange(instructionStartAddress, address, memory);
            return new DecodedInstruction(instruction, operands.ToArray(), store, branch, instructionStartAddress, address, allBytes);
        }

        private List<Operand> GetOperandAndType(byte operandTypeByte, byte[] memory, ref int address)
        {
            //loop though the operandtypes, until 
            // a. reach the end
            // b. hit an omitted value (11)

            // types are stored 2 bits and r->l so 6 = op1 4 = op2 etc
            // we also collect the values as we go.

            var instrOperands = new List<Operand>();
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

        private Branch GetBranch(byte[] memory, ref int address, bool hasBranch)
        {

            if (hasBranch)
            {
                byte[] branch;
                // is it one byte only
                var branchByte = memory[address += 1];
                if ((branchByte >> 6 & 1) == 1) branch = new[] { branchByte };
                else branch = new[] { branchByte, memory[address += 1] };

                Branch branchObj = new(branch, ((branchByte >> 7 & 1) == 1));

                return branchObj;
            }
            else
            {
                // So technicallly...techincally this is 'false' return, but we know in each instruction what we are storing/returning 
                // so this is safe.
                return Branch.Empty;
            }

        }

        private string GetHexAddressRange(int startAddress, int endAddress, byte[] memory) => String.Join(" ", memory.Skip(startAddress).Take((1 + endAddress) - startAddress).Select(itm => itm.ToString("X2")));

        // 4.2 http://inform-fiction.org/zmachine/standards/z1point1/sect04.html
        private byte[] GetOperandFromType(OperandType operandType, byte[] memory, ref int address) => operandType switch
        {
            OperandType.LargeConstant => new[] { (memory[address += 1]), (memory[address += 1]) },
            OperandType.SmallConstant => new[] { memory[address += 1] },
            OperandType.Omitted => new byte[] { 0 },
            OperandType.Variable => new[] { memory[address += 1] }
        };

        // the range Size seemsto be 1more than than actual range
        // is this cos of thezero?
        int InstructionModulo(int instructionByte, int lowestRange, int rangeSize)
        {
            if (instructionByte > lowestRange + rangeSize)
            {
                var difference = instructionByte - lowestRange;
                if (difference > rangeSize)
                {
                    difference = difference % rangeSize;
                }
                return lowestRange + difference;
            }
            return instructionByte;
        }

        private byte[] LiteralStringBytes(byte[] rawBytes, ref int startAddress)
        {

            if (rawBytes.Length < 2)
            {
                throw new Exception("Two bytes minimum");
            }
            // Zchars come in 2 byte zwords (16 bit), for their encoding
            // 1bit 5BitChar 5BitChar 5BitChar 
            // The completed word/sentance etc
            List<byte> AllBytes = new List<byte>();
            var isTerminated = false;
            var idx = startAddress;
            // We don't know how large the string is until we hit a terminator value (top bit == 1)
            while (!isTerminated)
            {
                // is the top bit a 1, last section of the string.
                isTerminated = (rawBytes[idx] >> 7 == 1);
                // Decode the characters
                var nxtIdx = idx + 1;
                AllBytes.Add(rawBytes[idx]);
                AllBytes.Add(rawBytes[nxtIdx]);

                if (!isTerminated)
                    idx += 2; // Byte address words
                else
                    idx += 1; // Move to the end of the current byte address
            }
            //startAddress = idx - 1;
            startAddress = idx;
            return AllBytes.ToArray();
        }
    }
}
