using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            _= firstByte switch
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
            throw new NotImplementedException();
        }

        private int be(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }

        private int variable2OP(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }

        private int short0OP(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }

        private int short1OP_V(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }

        private int short1OP_C(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }

        private int short1OP_LC(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }

        private int long2OP_VV(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }

        private int long2OP_VC(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }

        private int long2OP_CV(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
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
            var branch = id.Branch? memory[startAddress += 1] : (byte)0;


            var instruction = decimalInstruction switch {
                1 => je(new[] { op1, op2 }, branch),
                2 => jl(new[] { op1, op2 }, branch),
                3 => jg(new[] { op1, op2 }, branch),
                4=>dec_chk(new[] { op1, op2 }, branch),
                5=>inc_chk(new[] { op1, op2 }, branch),
                _ => "";
            };
            /*
             *                 case 2=>jl(memory, startAddress),
                case 3=>jg(memory, startAddress),
                    break;
                case 4:
                    dec_chk(memory, startAddress);
            break;
                case 5:
                    inc_chk(memory, startAddress);
            break;
            default:
                    throw new ArgumentOutOfRangeException($"Instruction unknown ${decimalInstruction} ${decimalInstruction.ToString("X")} {Convert.ToString(decimalInstruction, 2)}");
            break;
            */
            
            return address;
        }


    }
}
