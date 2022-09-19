using Zmachine.Library.V2.Utilities;

namespace Zmachine.Library.V2
{
    public class DictionaryTable
    {
        internal byte[][] Entries { get; }
        public int DictionaryEntries { get; }

        // this type is clearly wrong, but thats what it's v1.
        public char[] WordSeparators { get; }
        public byte WordEntryLength { get; }
        public int Version { get; }
        internal ushort StartAddress { get; }
        public int Length;

        /// <summary>
        /// Builds the list of dictionary table bytes on construction
        /// no text at all at this point.
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="Memory"></param>
        public DictionaryTable(int version, ushort startAddress, byte[] Memory)
        {
            this.Version = version;
            var separatorCount = Memory[startAddress];
            this.StartAddress = startAddress;
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
            this.DictionaryEntries = startAddress + separatorCount + 1 + 1 + 2;
            var dictionaryEntryAddress = this.DictionaryEntries;
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
                    if (Version > 3)
                        Entry[i] = Memory[dictionaryEntryAddress + i];
                    else if (i < 4)
                        Entry[i] = Memory[dictionaryEntryAddress + i];
                }

                Entries[x] = Entry;
                // MOve to the next. or even past the end
                dictionaryEntryAddress += (WordEntryLength);
            };

        }

        public byte[] this[int entry] => this.Entries[entry];

        public (ushort wordAddress, ushort wordEntryId)  FindMatch(byte[] wordZchars)
        {
            var wordLength = wordZchars.Length;
            var entryId = 0;
            for (var x = 0; x < this.Entries.Length; ++x)
            {

                var entry = this.Entries[x];
                // compare ALL the bytes!!!
                var ctr = 0;
                while ((entry[ctr] == wordZchars[ctr]) && ctr<WordEntryLength)
                {
                    ctr++;
                    if (ctr == wordLength)
                    {
                        entryId = x;
                        break;
                    }
                }
                
            }
            if (entryId > 0)
                return new ((ushort)(this.DictionaryEntries + this.WordEntryLength * entryId), (ushort)(entryId));
            return new (0,0);
        }
    }
}