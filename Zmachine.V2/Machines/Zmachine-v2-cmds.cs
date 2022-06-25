using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2.Machines
{
    public partial class Zmachine_v2
    {
        // Load a byte from memory
        internal void loadb(byte address)
        {
           var data =  this.Memory[address];
        }

        // lord a word from memory{
        internal void loadw(byte address)
        {
            var data = this.Memory.Get2ByteValue(address*2);
        }
    }
}
