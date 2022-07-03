using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2
{
    public class ZmAbbreviations
    {
        private readonly int version;
        private int totalAbbreviations;
        public int Length { get => totalAbbreviations; }
        public ZmAbbreviations(int startAddress, byte[] memory, int version)
        {
            this.StartAddress = startAddress;
            this.Memory = memory;
            this.version = version;
            if (version < 3)
                totalAbbreviations = 32;
            else
                totalAbbreviations = 96;
        }



        public int StartAddress { get; }
        public byte[] Memory { get; }
        /// <summary>
        /// Returns the memory address pointed at by the contents of the Abbreviation table entry.
        /// The Memory address is a Word Address and thus must have it's value multipled by 2. 
        /// That's the start of the abbreviation word(s).
        /// </summary>
        /// <param name="abbrevIdx"></param>
        /// <returns></returns>1
        public int this[int abbrevIdx]
        {
            get
            {
                if (abbrevIdx > totalAbbreviations) throw new ArgumentOutOfRangeException($"Only {totalAbbreviations} abbreviation entries avilable.");
                var addresssOffset = 2 * abbrevIdx;
                int rawAddress = this.Memory.Get2ByteValue(this.StartAddress + addresssOffset);
                return rawAddress * 2;
            }
        }

        public byte[] GetEntry(int table, int entry)
        {
            var address = ((32 * (table - 1)) + entry)*2;
            var basicAddress = (int)this.Memory.Get2ByteValue(address + StartAddress);
            basicAddress *= 2;
            return ZmTextDecoder.GetZChars(this.Memory, ref basicAddress);
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
