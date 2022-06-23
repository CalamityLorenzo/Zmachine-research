using Zmachine.Workings.Utilities;

namespace Zmachine.Workings
{
    internal class ZMachineDictionary
    {
        public ZMachineDictionary(int startAddress, byte[] Memory, ZmTextEncodingDecoding textDecoder)
        {
            var separatorCount = Memory[startAddress];

            this.WordSeparators = new char[separatorCount];
            for (var x = 0; x < separatorCount; x++)
            {
                // this is a diryt hack and wrng
                // but gets me started
                WordSeparators[x] = (char)Memory[startAddress + 1 + x];
            }

            this.WordEntryLength = Memory[startAddress + 1 + separatorCount];
            this.TotalNumberOfEntries = Memory.Get2ByteValue(startAddress + separatorCount + 1 + 1);

            this.Entries = new string[TotalNumberOfEntries];
            // now to populate the dictionary!
            // each entry
            int dictionaryEntryAddress = startAddress + separatorCount + 1 + 1 + 2;
            
            for (var x = 0; x < TotalNumberOfEntries; x++)
            {
                // Move the memory counter along 1 dictionary word length
                // each entry is a certain length of bytes (WordEntryLength)
                // get those bytes baby!
                byte[] Entry = new byte[WordEntryLength];
                for (var i = 0; i < WordEntryLength; i++)
                {
                    Entry[i] = Memory[dictionaryEntryAddress + i];
                }
                Entries[x] = $"{(dictionaryEntryAddress):X} : " + " " + String.Join(' ', Entry) + " " + textDecoder.ZmDecodeBytes(Entry);
                // MOve to the next. or even past the end
                dictionaryEntryAddress += (WordEntryLength);
            };
             
        }
        public string[] Entries { get; }
        // this type is clearly wrong, but thats what it's v1.
        public char[] WordSeparators { get; }
        public byte WordEntryLength { get; }
        public ushort TotalNumberOfEntries { get; }
        public ZmTextEncodingDecoding TextDecoder { get; }
    }

}
