namespace Zmachine.V2
{
    public static class ZmTextDecoder
    {
        /// <summary>
        /// THis method assumes the singleZChars is completed (including abbreviations)
        /// must be interpolated before calling this method.
        /// </summary>
        /// <param name="singleZChars"></param>
        /// <returns></returns>
        public static string DecodeZChars(byte[] singleZChars)
        {
            Dictionary<byte, char> decodeDictionary = ZCharDictionaries.A0Decode;
            char[] allChars = new char[singleZChars.Length / 2 * 3];
            for (var x = 0; x < singleZChars.Length; x++)
            {
                if (singleZChars[x] < 4)
                {
                    if(singleZChars[x] == 0)
                        allChars[x] = ' ';
                }
                else if (singleZChars[x] != 4 && singleZChars[x] != 5)
                {
                    allChars[x] = decodeDictionary[singleZChars[x]];
                }
                else
                {
                    if (singleZChars[x] == 4 && singleZChars[x - 1] != 4)
                        decodeDictionary = ZCharDictionaries.A1Decode;
                    else if (singleZChars[x] == 5 && singleZChars[x - 1] != 5)
                        decodeDictionary = ZCharDictionaries.A2V3Decode;
                    else
                        allChars[x] = ' ';
                }
            }

            return new string(allChars);
        }



        /// <summary>
        /// Returns an array of bytes in the ZChar format.
        /// We can pass any arbitrary array into here...so be careful
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static byte[] GetZChars(byte[] rawBytes, int startAddress=0)
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
            while(!isTerminated)
            {
                // is the top bit a 1, last section of the string.
                isTerminated  = (rawBytes[idx] >> 7 == 1);
                // So three chars are in here.
                // Get the individual characters out
                // COnvert two bytes into an Int16 (16 bits 4 bytes) move the top 8 bits to the left 1 byte and 
                ushort zWord = (ushort)((rawBytes[idx] << 8) | rawBytes[idx + 1]);
                byte[] currentZchars = GetZCharBytesFromWord(zWord);
                // Decode the characters
                AllBytes.AddRange(currentZchars);
                idx += 2; // Byte address words
            }
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
        public static string DecodeDictionaryEntry(IEnumerable<byte> bytes)
        {
            var charData = bytes.ToArray();
            var AllBytes = GetZChars(charData);
            return DecodeZChars(AllBytes);

        }
    }
}
