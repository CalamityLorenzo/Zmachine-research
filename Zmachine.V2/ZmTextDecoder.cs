﻿namespace Zmachine.V2
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
            //char[] allChars = new char[singleZChars.Length / 2 * 3];

            List<Char> allChars = new();
            string DictType = null;
            Dictionary<byte, char>? oldDictionary = null;
            for (var x = 0; x < singleZChars.Length; x++)
            {
                if (singleZChars[x] < 4)
                {
                    if (singleZChars[x] == 0)
                        allChars.Add(' ');
                }
                else if (singleZChars[x] != 4 && singleZChars[x] != 5)
                {
                    allChars.Add(decodeDictionary[singleZChars[x]]);
                    if (oldDictionary != null)
                    {
                        decodeDictionary = oldDictionary;
                        oldDictionary = null;
                    }
                    DictType = null;
                }
                else
                {
                    if (singleZChars[x] == 4)
                    {
                        if (x == 0 || singleZChars[x - 1] != 4)
                        {
                            oldDictionary = decodeDictionary;
                            decodeDictionary = ZCharDictionaries.A1Decode;
                        }
                    }
                    else if (singleZChars[x] == 5)
                    {
                        if (x == 0 || singleZChars[x - 1] != 5)
                        {
                            oldDictionary = decodeDictionary;
                            decodeDictionary = ZCharDictionaries.A2V3Decode;
                        }
                    }
                    else
                        allChars[x] = ' ';
                    //else
                    //    allChars[x] = ' ';
                }
            }

            return new string(allChars.ToArray());
        }

        public static string DecodeZCharsWithAbbreviations(byte[] singleZChars, ZmAbbreviations abbreviations)
        {
            Dictionary<byte, char> decodeDictionary = ZCharDictionaries.A0Decode;
            //char[] allChars = new char[singleZChars.Length / 2 * 3];
            List<Char> allChars = new();

            // this can be set from 1->3
            // on the NEXT loop we fetch the indicated abbreviation from the table 
            // convert that in zChars and shove it into the allChars list
            // (remember to move past the abbreviation byte too)
            int getAbbreviation = 0;
            Dictionary<byte, char>? oldDictionary = null;
            bool isA2Dictionary = false;
            byte isZChar = 0;
            byte ZSCIIChar = 0;
            for (var x = 0; x < singleZChars.Length; x++)
            {
                if (isZChar>0)
                {
                    if (isZChar == 2)
                    {
                        ZSCIIChar = (byte)(singleZChars[x] & 31);
                        isZChar = 1;
                    }else if (isZChar == 1)
                    {
                        var result = ((byte)ZSCIIChar << 5) | ((byte)(singleZChars[x] & 31));
                        allChars.Add((char)result);
                        isZChar = 0;
                    }

                }else if (getAbbreviation > 0)
                {
                    var entry = abbreviations.GetEntry(getAbbreviation, singleZChars[x]);
                    allChars.AddRange(ZmTextDecoder.DecodeZChars(entry));
                    // lets just get that sorted    
                    getAbbreviation = 0;

                } // if we had an abbreviation dontt forget to step through, or you will get spurious characters
                else if (singleZChars[x] < 4)
                {
                    if (singleZChars[x] == 0)
                        allChars.Add(' ');
                    else
                        getAbbreviation = singleZChars[x];

                }
                else if (singleZChars[x] != 4 && singleZChars[x] != 5)
                {
                    if (!isA2Dictionary)
                        // Normal case.
                        allChars.Add(decodeDictionary[singleZChars[x]]);
                    else
                    {
                        if (singleZChars[x] == 6)
                        {
                            isZChar = 2;
                        }
                        else
                        {
                            allChars.Add(decodeDictionary[singleZChars[x]]);
                        }
                        isA2Dictionary = false;
                    }
                    if (oldDictionary != null)
                    {
                        decodeDictionary = oldDictionary;
                        oldDictionary = null;
                    }
                }
                else
                {
                    if (singleZChars[x] == 4)
                    {
                        if (x == 0 || singleZChars[x - 1] != 4)
                        {
                            oldDictionary = decodeDictionary;
                            decodeDictionary = ZCharDictionaries.A1Decode;
                        }
                    }
                    else if (singleZChars[x] == 5)
                    {
                        if (x == 0 || singleZChars[x - 1] != 5)
                        {
                            oldDictionary = decodeDictionary;
                            decodeDictionary = ZCharDictionaries.A2V3Decode;
                            isA2Dictionary = true;
                        }
                    }
                    else
                        allChars[x] = ' ';
                }
            }

            return new string(allChars.ToArray());
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
                // COnvert two bytes into an Int16 (16 bits 4 bytes) move the top 8 bits to the left 1 byte and 
                ushort zWord = (ushort)((rawBytes[idx] << 8) | rawBytes[idx + 1]);
                byte[] currentZchars = GetZCharBytesFromWord(zWord);
                // Decode the characters
                AllBytes.AddRange(currentZchars);
                idx += 2; // Byte address words
            }
            startAddress = idx - 1;
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
            var memoryAddress = 0;
            var AllBytes = GetZChars(charData, ref memoryAddress);
            return DecodeZChars(AllBytes);

        }
    }
}
