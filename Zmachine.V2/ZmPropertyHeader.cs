namespace Zmachine.V2
{
    internal class ZmPropertyHeader
    {
        public ushort HeaderLength { get; }
        public string Name { get; }
        public ZmPropertyHeader(ushort PropertyAddress, byte[] Memory)
        {
            HeaderLength = Memory[PropertyAddress]; // This value is 2 bytes words ie 3 = 6.
            var start = PropertyAddress + 1;
            var end = PropertyAddress + 1 + HeaderLength * 2;
            // the full range of bytes for the name
            byte[] NameBytes = Memory[start..end];
            int addr = 0;
            byte[] zChars = ZmTextDecoder.GetZChars(NameBytes,ref addr);
            this.Name = ZmTextDecoder.DecodeZChars(zChars);
        }




    }
}
