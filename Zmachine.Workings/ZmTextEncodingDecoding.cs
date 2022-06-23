using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.Workings
{
    public class ZmTextEncodingDecoding
    {
        public ZmAbbreviations Abbreviations { get; }

        public ZmTextEncodingDecoding(ZmAbbreviations abbreviations)
        {
            Abbreviations = abbreviations;
        }

        /// <summary>
        /// Turn a bunch of chars into a ZMachine friendly representation
        /// </summary>
        /// <param name="inputChars"></param>
        /// <returns></returns>
        public List<EncodedChar> EncodeChars(char[] inputChars)
        {
            List<EncodedChar> encodedChars = new List<EncodedChar>();

            //Encoding each character with 
            foreach (var @char in inputChars)
            {
                AlphabetType alphaType = AlphabetType.Lower;
                if (Char.IsUpper(@char))
                    alphaType = AlphabetType.Upper;
                else
                {
                    if (Char.IsDigit(@char) || char.IsPunctuation(@char) || (byte)@char == 13)
                        alphaType = AlphabetType.Punctuation;
                }

                if (alphaType != AlphabetType.Punctuation)
                {
                    // This coverts into Ascii lowercase.
                    // Lowercase. and uppercase alphabets have the same index.
                    // lowercase happens more often
                    var zCharCode = 0;
                    // Spaces are 0 in zmachine
                    if (@char != 32)
                    {
                        var charCode = alphaType == AlphabetType.Lower ? (byte)@char : (byte)@char + 32;
                        // Ascii 'a' = 91
                        zCharCode = charCode - 91;
                    }
                    encodedChars.Add(new EncodedChar
                    {
                        alphabet = alphaType,
                        CharCode = (byte)zCharCode,
                        Char = @char
                    });
                }
                else
                {
                    // We are assuming a post v2  with dictionary.
                    var zCharCode = ZCharDictionaries.A2V3.ContainsKey(@char) ?
                                        ZCharDictionaries.A2V3[@char] : ZCharDictionaries.A2V3['?'];
                    encodedChars.Add(new EncodedChar
                    {
                        alphabet = alphaType,
                        CharCode = (byte)zCharCode
                    });
                }
            }
            return encodedChars;
        }

        /// <summary>
        /// Turn a zmahcine friendly version into the zmahcine version
        /// </summary>
        /// <param name="allChars"></param>
        /// <returns></returns>
        public List<ushort> ZmEncodedChars(List<EncodedChar> allChars)
        {
            // first create a complete flat byte array, with the correct alphabet
            var allBytes = new List<byte>();
            foreach (var @char in allChars)
            {
                if (@char.alphabet != AlphabetType.Lower)
                    allBytes.Add((byte)@char.alphabet);
                allBytes.Add(@char.CharCode);
            }

            // turn that flat array of bytes into the ushort array of characters
            List<ushort> allZChars = new List<ushort>();
            for (var x = 0; x < allBytes.Count; x += 3)
            {

                BitArray bitArray = new BitArray(16);
                // Are we at the end of the sequence
                bitArray.Set(15, (allBytes.Count <= x + 3) ? true : false);
                //top 
                SetZChar(bitArray, allBytes[x], 10);
                // middle (padding bit)
                SetZChar(bitArray, (x + 1 < allBytes.Count) ? allBytes[x + 1] : (byte)'5', 5);
                //bottom (padding bit)
                SetZChar(bitArray, (x + 2 < allBytes.Count) ? allBytes[x + 2] : (byte)'5', 0);

                int[] result = new int[1];
                bitArray.CopyTo(result, 0);
                allZChars.Add((ushort)result[0]);
            }

            return allZChars;
        }

        public string ZmDecodeChars2(List<byte> singleChars)
        {
            Dictionary<byte, char> decodeDictionary = ZCharDictionaries.A0Decode;
            char[] allChars = new char[singleChars.Count / 2 * 3];
            bool insertAbbreviation = false;
            for (var x = 0; x < singleChars.Count; x++)
            {

                if (insertAbbreviation)
                {
                    insertAbbreviation = false;
                }
                else  if (singleChars[x] < 4)
                {
                    // 2 == Abbreviation
                    if (singleChars[x] == 2)
                    {
                        insertAbbreviation = true;
                    }
                }
                else if (singleChars[x] != 4 && singleChars[x] != 5)
                {
                    allChars[x] = decodeDictionary[singleChars[x]];
                }
                else
                {
                    if (singleChars[x] == 4 && singleChars[x - 1] != 4)
                        decodeDictionary = ZCharDictionaries.A1Decode;
                    else if (singleChars[x] == 5 && singleChars[x - 1] != 5)
                        decodeDictionary = ZCharDictionaries.A2V3Decode;
                    else
                        allChars[x] = ' ';
                }
            }

            return new string(allChars);
        }

        public string ZmDecodeChars(List<byte> singleChars)
        {
            Dictionary<byte, char> decodeDictionary = ZCharDictionaries.A0Decode;
            bool useDict = false;
            bool isUpper = false;
            List<char> allChars = new();
            for (var x = 0; x < singleChars.Count; x++)
            {
                if (useDict)
                {
                    if (singleChars[x] != 4 && singleChars[x] != 5)
                    {

                        allChars.Add(decodeDictionary[singleChars[x]]);
                        useDict = false;
                    }
                    else
                    {
                        allChars.Add(' ');
                        useDict = true;
                    }
                }
                else
                {
                    if (singleChars[x] != 4 && singleChars[x] != 5)
                    {
                        if (singleChars[x] == 0)
                        {
                            allChars.Add(' ');
                        }
                        else
                        {
                            var charToAdd = !isUpper ? (char)(singleChars[x] + 91) : (char)(singleChars[x] + 59);
                            allChars.Add(charToAdd);
                        }
                        useDict = false;
                        isUpper = false;
                    }
                    else
                    {
                        if (singleChars[x] == 5)
                        {
                            useDict = true;
                            decodeDictionary = ZCharDictionaries.A2V3Decode;
                        }
                        else
                        {
                            isUpper = true;
                        }
                    }
                }
            }

            return new string(allChars.ToArray());
        }

        public byte[] GetZChars(byte[] rawBytes)
        {

            if (rawBytes.Length < 2)
            {
                throw new Exception("Two bytes minimum");
            }
            // Zchars come in 2 byte zwords (16 bit), for their encoding
            // 1bit 5BitChar 5BitChar 5BitChar 
            // The completed word/sentance etc
            List<byte> AllBytes = new List<byte>();
            for (var i = 0; i < rawBytes.Length; i += 2)
            {
                // is the top bit a 1, last section of the string.
                bool isTerminator = (rawBytes[i] >> 7 == 1);
                // So three chars are in here.
                // Get the individual characters out
                // COnvert two bytes into an Int16 (16 bits 4 bytes) move the top 8 bits to the left 1 byte and 
                ushort zWord = (ushort)((rawBytes[i] << 8) | rawBytes[i + 1]);
                byte[] currentZchars = GetZCharBytesFromWord(zWord);
                // Decode the characters
                AllBytes.AddRange(currentZchars);
                if (isTerminator)
                    break;
            }
            return AllBytes.ToArray();
        }

        //Right now we asssume all chars are lower.
        public string ZmDecodeBytes(IEnumerable<byte> bytes)
        {
            var charData = bytes.ToArray();
            var AllBytes = this.GetZChars(charData);
            return ZmDecodeChars2(AllBytes.ToList());

        }

        private byte[] GetZCharBytesFromWord(ushort zWord)
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
        /// Test to see if what goes in compes back out again.
        /// </summary>
        /// <param name="allChars"></param>
        public void ZmDisplay(List<ushort> allChars)
        {

        }

        void SetZChar(BitArray bitArray, byte @byte, int startIndex)
        {
            // 5 bits in a zchar
            for (int x = 0; x < 5; ++x)
            {
                var bit = ((@byte >> x) & 1) == 1;
                bitArray.Set(startIndex + x, bit);
            }
        }
    }
}
