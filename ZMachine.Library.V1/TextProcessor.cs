namespace ZMachine.Library.V1
{
    public class TextProcessor
    {
        // Which way we are moving betwen dictionaries
        private enum ShiftDirection
        {
            Unknown = 0,
            Up,
            Down,
        }

        private readonly int version;
        private readonly byte[] memory;
        private readonly AbbreviationsTable abbreviations;
        private readonly Dictionary<string, Dictionary<byte, char>> DecodeDictionaries;
        private readonly Dictionary<string, Dictionary<char, byte>> EncodeDictionaries;

        public TextProcessor(byte[] Memory, AbbreviationsTable Abbreviations, int Version)
        {
            version = Version;
            memory = Memory;
            abbreviations = Abbreviations;

            // TODO: Custom dictionaruies kept in the story file.
            if (version > 1)
            {
                DecodeDictionaries = new()
                {
                    {"A0", ZCharDictionaries.A0Decode },
                    {"A1", ZCharDictionaries.A1Decode },
                    {"A2", ZCharDictionaries.A2V3Decode }
                };
                EncodeDictionaries = new()
                {
                    {"A0", ZCharDictionaries.A0Encode },
                    {"A1", ZCharDictionaries.A1Encode},
                    {"A2", ZCharDictionaries.A2V3Encode},
                };
            }
            else
            {
                DecodeDictionaries = new()
                {
                    {"A0", ZCharDictionaries.A0Decode },
                    {"A1", ZCharDictionaries.A1Decode },
                    {"A2", ZCharDictionaries.A2V1Decode }
                };
                EncodeDictionaries = new()
                {
                    {"A0", ZCharDictionaries.A0Encode },
                    {"A1", ZCharDictionaries.A1Encode},
                    {"A2", ZCharDictionaries.A2V1Encode},
                };
            }
        }

        /// <summary>
        /// Discrimator for what ever version we are using.
        /// </summary>
        /// <param name="singleZChars"></param>
        /// <returns></returns>
        public string DecodeZChars(byte[] singleZChars)
        {
            if (version > 3)
                return DecodeZCharsV3Upwards(singleZChars);
            else
                return DecodeZCharsV2Downwards(singleZChars);
        }

        private string DecodeZCharsV2Downwards(byte[] singleZChars)
        {
            List<Char> allChars = new();
            var currentDictionary = new KeyValuePair<string, Dictionary<byte, char>>("A0", DecodeDictionaries["A0"]);
            var oldDictionaryKey = "A0";
            // Simple Shift means '2' or '3' eg next char only is going to be SHIFTed
            var shiftLock = false;
            bool getAbbreviation = true;
            int decodeZSCII = 0;
            byte ZSCIIChar = 0;
            for (var x = 0; x < singleZChars.Length; ++x)
            {
                var currentChar = singleZChars[x];
                // Sneaking a little loop into athe current loop.
                if (decodeZSCII > 0)
                {
                    if (decodeZSCII == 2)
                    {
                        ZSCIIChar = (byte)(currentChar & 31);
                        decodeZSCII = 1;
                    }
                    else if (decodeZSCII == 1)
                    {
                        var result = ((byte)ZSCIIChar << 5) | ((byte)(currentChar & 31));
                        // exceptions for the zchars in v1.
                        if (result == 0)
                        {
                            allChars.Add(' ');
                        }
                        else if (result == 1 && version == 1)
                        {
                            allChars.Add('\n');
                        }
                        else
                        {
                            allChars.Add((char)result);
                        }

                        decodeZSCII = 0;
                    }
                }
                else if (getAbbreviation)
                {
                    var entryAddress = this.abbreviations.GetEntryAddress(1, currentChar);
                    allChars.AddRange(this.DecodeAbbreviationEntry(entryAddress));
                    getAbbreviation = false;
                }
                else // actual character code work
                {
                    if (currentChar == 1)
                    {
                        getAbbreviation = true;
                    }
                    else if (currentChar == 2 || currentChar == 3 || currentChar == 4 || currentChar == 5)
                    {
                        var newDictionaryKey = ShiftCharDictionary(currentChar % 2 == 0 ? ShiftDirection.Up : ShiftDirection.Down, currentDictionary.Key);
                        oldDictionaryKey = currentDictionary.Key;
                        currentDictionary = new KeyValuePair<string, Dictionary<byte, char>>(newDictionaryKey, DecodeDictionaries[newDictionaryKey]);
                        if (currentChar > 3) shiftLock = true;
                    }
                    else if (currentChar == 6) // ZChar
                    {
                        decodeZSCII = 2;
                    }

                }
            }

            return new string(allChars.ToArray());
        }

        private string DecodeZCharsV3Upwards(byte[] singleZChars)
        {
            var currentDictionary = KeyValuePair.Create("A0", this.DecodeDictionaries["A0"]);

            // The string to be built
            List<Char> allChars = new();

            // this can be set from 1->3
            // on the NEXT loop we fetch the indicated abbreviation from the table 
            // convert that in zChars and shove it into the allChars list
            // (remember to move past the abbreviation byte too)
            int getAbbreviation = 0;
            // Zhcars are the 10 byte characters (2x5).
            // if we select  '6' this is set to 2.
            // and the loop progresses counting down isZchar and decoded the characters
            byte isZsciiChar = 0;
            byte ZSCIIChar = 0;
            // TO DO : Change this to a while loop.
            // it will make it esaier to read :
            // eg abreviations are handled in the same loop iteration
            // isZChar counter can be removed.
            for (var x = 0; x < singleZChars.Length; x++)
            {
                var currentChar = singleZChars[x];
                if (isZsciiChar > 0)
                {
                    if (isZsciiChar == 2)
                    {
                        ZSCIIChar = (byte)(currentChar & 31);
                        isZsciiChar = 1;
                    }
                    else if (isZsciiChar == 1)
                    {
                        var result = ((byte)ZSCIIChar << 5) | ((byte)(currentChar & 31));
                        allChars.Add((char)result);
                        isZsciiChar = 0;
                        // Don't forget to reset the the fucking dictionary
                        currentDictionary = KeyValuePair.Create("A0", this.DecodeDictionaries["A0"]);
                    }

                }
                else if (getAbbreviation > 0)
                {
                    var entryAddress = abbreviations.GetEntryAddress(getAbbreviation, currentChar);
                    var abbreviation = this.DecodeAbbreviationEntry(entryAddress);
                    allChars.AddRange(abbreviation);
                    // lets just get that sorted    
                    getAbbreviation = 0;

                } // if we had an abbreviation dontt forget to step through, or you will get spurious characters
                else
                {
                    if (currentChar == 0)
                        allChars.Add(' ');
                    else if (currentChar < 4)
                        getAbbreviation = currentChar;
                    else if (currentChar == 4 || currentChar == 5)
                    {
                        var key = currentChar == 4 ? "A1" : "A2";
                        currentDictionary = KeyValuePair.Create(key, this.DecodeDictionaries[key]);
                    }
                    else if (currentChar == 6 && currentDictionary.Key == "A2")
                    {
                        isZsciiChar = 2;
                    }
                    else
                    { // We are writing the actual content here!
                        if (currentDictionary.Key != "A2")
                            // Normal case.
                            allChars.Add(currentDictionary.Value[currentChar]);
                        else
                        {   // 10 byte extended characters
                            allChars.Add(currentDictionary.Value[currentChar]);
                        }

                        if (currentDictionary.Key != "A0")
                        {
                            currentDictionary = KeyValuePair.Create("A0", this.DecodeDictionaries["A0"]);
                        }
                    }
                }
            }

            return new string(allChars.ToArray());
        }

        /// <summary>
        /// Choosing dictionaries is a little risky in v1 and v2.
        /// </summary>
        /// <param name="shift"></param>
        /// <param name="currentKey"></param>
        /// <returns></returns>
        private string ShiftCharDictionary(ShiftDirection shift, string currentKey)
        {
            if (shift == ShiftDirection.Up)
            {
                return currentKey switch
                {
                    "A0" => "A1",
                    "A1" => "A2",
                    "A2" => "A1",
                    _ => throw new ArgumentOutOfRangeException("Up dictionary not found."),
                };
            }
            else
            {
                return currentKey switch
                {
                    "A0" => "A2",
                    "A1" => "A0",
                    "A2" => "A1",
                    _ => throw new ArgumentOutOfRangeException("Down dictionary not found."),
                };
            }
        }


        /// <summary>
        /// This method assumes you have passed in reasonable zchars.
        /// eg ends with correct padding, and has upper bit set, is divisible by 3.
        /// </summary>
        /// <param name="zChars"></param>
        /// <returns></returns>
        public byte[] EncodeZcharsToWords(byte[] zChars)
        {

            List<byte> zWords = new();
            // now take that stack of bytes and turn 3 chars into 2 bytes....
            for (var x = 0; x < zChars.Length; x += 3)
            {
                var top = zChars[x];
                var middle = zChars[x + 1];
                var bottom = zChars[x + 2];
                // Create the word here
                var zWord = (ushort)((top & 0x1f) << 10 | (middle & 0x1f) << 5 | (bottom & 0x1f));
                // if we are at the end tag it.
                if (x + 3 >= zChars.Length)
                    zWord = (ushort)(zWord | 0x8000);
                zWords.Add((byte)(zWord >> 8));
                zWords.Add((byte)(zWord & 255));
            }

            return zWords.ToArray();
        }

        // Helper so you dont have to always pass a ref address
        public byte[] GetZChars(byte[] rawBytes)
        {
            var start = 0;
            return GetZChars(rawBytes, ref start);
        }

        /// <summary>
        /// Returns an array of bytes in the ZChar format.
        /// We can pass any arbitrary array into here...so be careful
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static byte[] GetZChars(byte[] rawBytes, ref int startAddress)
        {
            if (rawBytes.Length < 2)
            {
                throw new Exception("Two bytes minimum");
            }
            // Zchars come in 2 byte zwords (16 bit), for their encoding
            // 1bit 5BitChar 5BitChar 5BitChar 
            // The completed word/sentance etc
            List<byte> AllBytes = new List<byte>();
            var isTerminated = false;
            var idx = startAddress;
            // We don't know how large the string is until we hit a terminator value (top bit == 1)
            while (!isTerminated)
            {
                // is the top bit a 1, last section of the string.
                isTerminated = (rawBytes[idx] >> 7 == 1);
                // So three chars are in here.
                // Get the individual characters out
                // Convert two bytes into an Int16 (16 bits 4 bytes) move the top 8 bits to the left 1 byte and 
                ushort zWord = (ushort)((rawBytes[idx] << 8) | rawBytes[idx + 1]);
                byte[] currentZchars = GetZCharBytesFromWord(zWord);
                // Decode the characters
                AllBytes.AddRange(currentZchars);
                if (!isTerminated)
                    idx += 2; // Byte address words
                else
                    idx += 1; // Move to the end of the current byte address
            }
            startAddress = idx;
            return AllBytes.ToArray();
        }

        /// <summary>
        /// extracts the 5 bit chasrs from the words
        /// </summary>
        /// <param name="zWord"></param>
        /// <returns></returns>
        private static byte[] GetZCharBytesFromWord(ushort zWord)
        {
            // bottom word (mask the first 5 bits by setting it's value 11111 (31))
            return new byte[] {
            (byte)((zWord >> 10) & 0x1f),
            // shift by 5 bits and repeat
            (byte)((zWord >> 5) & 0x1f),
            // once more with felling
            (byte)(zWord & 0x1f),
            };
        }

        /// <summary>
        /// Dictionaries are all lowercase, and cannot have abbreviations.
        /// Multi-block characters are something somwthinf
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string DecodeDictionaryEntry(IEnumerable<byte> bytes)
        {
            var charData = bytes.ToArray();
            var memoryAddress = 0;
            var AllBytes = GetZChars(charData, ref memoryAddress);
            return DecodeZChars(AllBytes);
        }
        /// <summary>
        /// Returns a complete abbreviation.
        /// </summary>
        /// <param name="startAddress"></param>
        /// <returns></returns>
        public string DecodeAbbreviationEntry(int startAddress)
        {
            var AllBytes = GetZChars(memory, ref startAddress);
            return DecodeZChars(AllBytes);
        }

        /// <summary>
        /// Encode a native string into a zmachine series of zchars
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public byte[] EncodeUtf8ZChars(string text)
        {
            List<byte> zChars = new(2); // This represents the 2 byte word of 1 control bit and three chars.
            var allChars = text.GetEnumerator();
            // Encode into the system dictionaries.
            // This is missing a lot nuance regarding unicode.
            while (allChars.MoveNext())
            {
                // Run through our list of chars and map them to the entries in the dictionaries (A0->A2)
                // we also insert the contol characters.
                var currentChar = allChars.Current;
                if (EncodeDictionaries["A0"].ContainsKey(currentChar))
                {
                    zChars.Add(EncodeDictionaries["A0"][currentChar]);
                }
                else if (EncodeDictionaries["A1"].ContainsKey(currentChar))
                {
                    zChars.Add(4);// the next char is in A1
                    zChars.Add(EncodeDictionaries["A1"][currentChar]);
                }
                else if (EncodeDictionaries["A2"].ContainsKey(currentChar))
                {
                    zChars.Add(5);// the next char is in A2
                    zChars.Add(EncodeDictionaries["A2"][currentChar]);
                }
                // If we don't have a match then we hopefully have a direct ZSCII match instead.
                else if ((byte)currentChar == 32)
                {
                    zChars.Add(0);
                }
                else
                {
                    // ZSCII incoming ten bit drama!
                    zChars.Add(5);
                    zChars.Add(6);
                    var bottom = currentChar & 0x1f;
                    var top = currentChar >> 5 & 0x1f;
                    zChars.Add((byte)top);
                    zChars.Add((byte)bottom);
                }
            }

            // Ensure we have any requisite padding.
            // each 2 byte word contains 3 charactres.
            // if the amount of chars is not divisible by3 
            // add the remainder as padding (5's)
            var mod3 = zChars.Count % 3;
            if (mod3 == 1)
                zChars.AddRange(new byte[] { 5, 5 }); //.ToArray();
            if (mod3 == 2)
                zChars.Add(5);

            // Now ensure the 2nd to last byte has the 7th (8th ashualkly) but set.

            return zChars.ToArray();
        }


        /// <summary>
        /// process the input string, and return only the valid ZSCII (input)chars.
        /// 3.8 Table 2
        /// </summary>
        /// <param name="data"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static byte[] ValidZsciiChars(string data, int version)
        {
            int dL = data.Length;
            List<byte> validChars = new();
            for(var x = 0; x < data.Length; ++x)
            {
                var thisChar = (byte)data[x];
                // Backspace
                if (thisChar == 8) validChars.Add(thisChar);
                // new line
                if (thisChar == 13) validChars.Add(thisChar);
                // Sentence space
                if (version ==6 && thisChar == 11) validChars.Add(thisChar);
                // escape
                if (thisChar == 27) validChars.Add(thisChar);
                // standard ascii
                if (thisChar >=32 && thisChar<=126) validChars.Add(thisChar);
                // cursor u/d/l/r
                if (thisChar >= 129 && thisChar <= 132) validChars.Add(thisChar);
                // FKeys
                if (thisChar >= 133 && thisChar <= 144) validChars.Add(thisChar);
                // Key pad
                if (thisChar >= 145 && thisChar <= 154) validChars.Add(thisChar);
                // extra chars
                if (thisChar >= 155 && thisChar <= 251) validChars.Add(thisChar);
                // menu click (v6)
                if (version == 6 && thisChar == 252) validChars.Add(thisChar);
                // double click (v6)
                if (version == 6 && thisChar == 253) validChars.Add(thisChar);
                // single click 
                if (thisChar == 254) validChars.Add(thisChar);
            }
            return validChars.ToArray();
        }

    }
}
