using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2
{
    public class ZmAbbreviations
    {
        public ZmAbbreviations(int startAddress, byte[] memory)
        {
            this.StartAddress = startAddress;
            this.Memory = memory;
        }

        public int StartAddress { get; }
        public byte[] Memory { get; }
        /// <summary>
        /// Returns the memory address pointed at by the contents of the Abbreviation table entry.
        /// The Memory address is a Word Address and thus must have it's value multipled by 2. 
        /// That's the start of the abbreviation word(s).
        /// </summary>
        /// <param name="abbrevIdx"></param>
        /// <returns></returns>
        public int this[int abbrevIdx]
        {
            get
            {
                if (abbrevIdx > 96) throw new ArgumentOutOfRangeException("Only 96 dictionary entries avilable.");
                var addresssOffset = 2 * abbrevIdx;
                int rawAddress = this.Memory.Get2ByteValue(this.StartAddress + addresssOffset);
                return rawAddress * 2;
            }
        }

        /// <summary>
        ///  Returns the abbreviation address in hex
        /// </summary>
        /// <param name="abbrevIdx"></param>
        /// <returns></returns>
        //public string this[int abbrevIdx]
        //{
        //    get
        //    { 
        //        // Target the correct address for this entry.
        //        var addresssOffset = 2 * abbrevIdx;
        //        return Memory.  Memory.Get2ByteValueHex(this.StartAddress + addresssOffset);
        //    }
        //}
    }
}
