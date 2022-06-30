using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zmachine.V2.InstructionDefinitions;

namespace Zmachine.V2
{

    // the methods for diassembly of commands
    internal partial class ZmInstructionDecoder
    {

        // Increment variable, and branch if now greater than value.

        private string inc_chk(byte[] values, int branch)
        {
            var variable = values[0];
            return $"inc_check ${(variable < 15 ? $"local" : "global")}{variable} {values[1]}->${branch.GetPackedAddress(version, rOffset,sOffset)}";
        }


        //  Decrement variable, and branch if it is now less than the given value.

        private string dec_chk(byte[] values, int branch)
        { 
            var variable = values[0];
            return $"dec_check ${(variable<15?$"local": "global")}{variable} {values[1]}->${branch.GetPackedAddress(version, rOffset,sOffset)}";
        }

        // Jump if a > b(using a signed 16-bit comparison).

        private string jg(byte[] values, int branch, int rOffset = 0, int sOffset = 0)
        {
            return $"jg ${String.Join(" ", values)}->${branch.GetPackedAddress(version, rOffset,sOffset)}";
        }

        // Jump if a < b (using a signed 16-bit comparison).
        private string jl(byte[] values, int branch, int rOffset = 0, int sOffset = 0)
        {
            return $"jl ${String.Join(" ", values)}->${branch.GetPackedAddress(version, rOffset,sOffset)}";
        }
        //Jump if a is equal to any of the subsequent operands. (Thus @je a never jumps and @je a b jumps if a = b.)
        private string je(byte[] values, int branch, int rOffset = 0, int sOffset = 0)
        {
            return $"je ${String.Join(" ", values)}->${branch.GetPackedAddress(version, rOffset,sOffset)}";
        }

        //Stores array-->word-index (i.e., the word at address array+2*word-index, which must lie in static or dynamic memory).
        private string loadw(byte[] values, int store, int rOffset=0, int sOffset=0)
        {
            return $"loadw ${values[0]}[{values[1]}]->${store.GetPackedAddress(version, rOffset,sOffset)}";
        }

        //Set the VARiable referenced by the operand to value.
        private string store(byte[] values)
        {
            var variable = values[0];
            return $"store {(variable < 15 ? $"local" : "global")}{variable} {values[1]}";
        }

        //Signed 16-bit subtraction.
        private string sub(byte[] values, int store)
        {
            var variable = values[1];
            return $"sub {values[0]} {(variable < 15 ? $"local" : "global")}{variable}";
        }


        private string put_prop(zType[] types, byte[] memory, int address)
        {

        }
    }
}
