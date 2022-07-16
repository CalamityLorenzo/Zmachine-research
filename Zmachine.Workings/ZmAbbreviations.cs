using Zmachine.Workings.Utilities;

namespace Zmachine.Workings
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

        public int this[int abbrevIdx]
        {
            get
            {
                var addresssOffset = 2 * abbrevIdx;
                ushort rawAddress = this.Memory.Get2ByteValue(this.StartAddress+ addresssOffset);
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
