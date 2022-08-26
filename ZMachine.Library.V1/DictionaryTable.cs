using ZMachine.Library.V1.Utilities;

namespace ZMachine.Library.V1
{
    public class DictionaryTable
    {
        internal byte[][] Entries { get; }
        // this type is clearly wrong, but thats what it's v1.
        public char[] WordSeparators { get; }
        public byte WordEntryLength { get; }
        public int Version { get; }

        public int Length;

        /// <summary>
        /// Builds the list of dictionary table bytes
        /// no text at all at this point.
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="Memory"></param>
        public DictionaryTable(int version, int startAddress, byte[] Memory)
        {
            this.Version = version;
            var separatorCount = Memory[startAddress];

            this.WordSeparators = new char[separatorCount];
            for (var x = 0; x < separatorCount; x++)
            {
                // this is a diryt hack and wrng
                // but gets me started
                WordSeparators[x] = (char)Memory[startAddress + 1 + x];
            }

            
            this.WordEntryLength = Memory[startAddress + 1 + separatorCount];

            this.Length = Memory.Get2ByteValue(startAddress + separatorCount + 1 + 1);

            this.Entries = new byte[Length][];
            // now to populate the dictionary!
            // each entry
            int dictionaryEntryAddress = startAddress + separatorCount + 1 + 1 + 2;

            for (var x = 0; x < Length; x++)
            {
                // Move the memory counter along 1 dictionary word length
                // each entry is a certain length of bytes (WordEntryLength)
                // get those bytes baby!
                byte[] Entry;
                if (Version > 3)
                    Entry = new byte[WordEntryLength];
                else
                    Entry = new byte[4];

                for (var i = 0; i < WordEntryLength; i++)
                {
                    if(Version >3)
                        Entry[i] = Memory[dictionaryEntryAddress + i];
                    else if(i<4)
                        Entry[i] = Memory[dictionaryEntryAddress + i];
                }

                Entries[x] = Entry;
                //// MOve to the next. or even past the end
                dictionaryEntryAddress += (WordEntryLength);
            };

        }

        public byte[] this[int entry] => this.Entries[entry];
    }
}