using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2
{

    // the methods for diassembly of commands
    internal partial class ZmInstructionDecoder
    {

        // Increment variable, and branch if now greater than value.

        private void inc_chk(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }

        //  Decrement variable, and branch if it is now less than the given value.

        private void dec_chk(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }

        // Jump if a > b(using a signed 16-bit comparison).

        private void jg(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }

        // Jump if a < b (using a signed 16-bit comparison).

        private void jl(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }

        //Jump if a is equal to any of the subsequent operands. (Thus @je a never jumps and @je a b jumps if a = b.)
        private void je(byte[] memory, int startAddress)
        {
            throw new NotImplementedException();
        }
    }
}
