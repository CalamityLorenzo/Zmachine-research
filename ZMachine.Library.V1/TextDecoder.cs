using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMachine.Library.V1
{


    public class TextDecoder
    {
        private enum ShiftDirection
        {
            Unknown = 0,
            Up,
            Down,
        }

        private readonly int version;
        private readonly byte[] memory;
        private readonly AbbreviationsTable abbreviations;
        private readonly Dictionary<string, Dictionary<byte, char>> VersionDictionaries;
        public TextDecoder(byte[] Memory, AbbreviationsTable Abbreviations, int Version)
        {
            version = Version;
            memory = Memory;
            abbreviations = Abbreviations;

            // TODO: Custom dictionaruies kept in the story file.
            if (version > 1)
                VersionDictionaries = new()
                {
                    {"A0", ZCharDictionaries.A0Decode },
                    {"A1", ZCharDictionaries.A1Decode },
                    {"A2", ZCharDictionaries.A2V3Decode }
                };
            else
            {
                VersionDictionaries = new()
                {
                    {"A0", ZCharDictionaries.A0Decode },
                    {"A1", ZCharDictionaries.A1Decode },
                    {"A2", ZCharDictionaries.A2V1Decode }
                };
            }
        }

        /// <summary>
        /// THis method assumes the singleZChars is completed (including abbreviations)
        /// must be interpolated before calling this method.
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

            var currentDictionary = new KeyValuePair<string, Dictionary<byte, char>>("A0", VersionDictionaries["A0"]);
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
                        ZSCIIChar = (byte)(singleZChars[x] & 31);
                        decodeZSCII = 1;
                    }
                    else if (decodeZSCII == 1)
                    {
                        var result = ((byte)ZSCIIChar << 5) | ((byte)(singleZChars[x] & 31));
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
                        var newDictionaryKey = ShiftDictionary(currentChar % 2 == 0 ? ShiftDirection.Up : ShiftDirection.Down, currentDictionary.Key);
                        oldDictionaryKey = currentDictionary.Key;
                        currentDictionary = new KeyValuePair<string, Dictionary<byte, char>>(newDictionaryKey, VersionDictionaries[newDictionaryKey]);
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
            var currentDictionary = KeyValuePair.Create("A0", this.VersionDictionaries["A0"]);

            //char[] allChars = new char[singleZChars.Length / 2 * 3];
            // The string to be built
            List<Char> allChars = new();

            // this can be set from 1->3
            // on the NEXT loop we fetch the indicated abbreviation from the table 
            // convert that in zChars and shove it into the allChars list
            // (remember to move past the abbreviation byte too)
            int getAbbreviation = 0;
            byte isZChar = 0;
            byte ZSCIIChar = 0;
            for (var x = 0; x < singleZChars.Length; x++)
            {
                var currentChar = singleZChars[x];
                if (isZChar > 0)
                {
                    if (isZChar == 2)
                    {
                        ZSCIIChar = (byte)(singleZChars[x] & 31);
                        isZChar = 1;
                    }
                    else if (isZChar == 1)
                    {
                        var result = ((byte)ZSCIIChar << 5) | ((byte)(singleZChars[x] & 31));
                        allChars.Add((char)result);
                        isZChar = 0;
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
                else if (singleZChars[x] < 4)
                {
                    if (singleZChars[x] == 0)
                        allChars.Add(' ');
                    else // 1-3 == Abbreviation table lookup.
                        getAbbreviation = singleZChars[x];
                }
                else if (singleZChars[x] == 4 || singleZChars[x] == 5)
                {
                    var key = singleZChars[x] == 4 ? "A1" : "A2";
                    currentDictionary = KeyValuePair.Create(key, this.VersionDictionaries[key]);
                }
                else
                { // We are writing the actual content here!
                    if (currentDictionary.Key != "A2")
                        // Normal case.
                        allChars.Add(currentDictionary.Value[singleZChars[x]]);
                    else
                    {   // 10 byte extended characters
                        if (singleZChars[x] == 6)
                        {
                            isZChar = 2;
                        }
                        else
                        {
                            allChars.Add(currentDictionary.Value[singleZChars[x]]);
                        }
                    }

                    if (currentDictionary.Key != "A0")
                    {
                        currentDictionary = KeyValuePair.Create("A0", this.VersionDictionaries["A0"]);
                    }
                }
            }


            return new string(allChars.ToArray());
        }

        /// <summary>
        /// Choosing dictionaries is a little risky in v1 and v2.
        /// </summary>
        /// <param name="shift"></param>
        /// <param name="currentDictionaryKey"></param>
        /// <returns></returns>
        private string ShiftDictionary(ShiftDirection shift, string currentDictionaryKey)
        {
            if (shift == ShiftDirection.Up)
            {
                return currentDictionaryKey switch
                {
                    "A0" => "A1",
                    "A1" => "A2",
                    "A2" => "A1",
                };
            }
            else
            {
                return currentDictionaryKey switch
                {
                    "A0" => "A2",
                    "A1" => "A0",
                    "A2" => "A1",
                };
            }
        }


        /// <summary>
        /// Returns an array of bytes in the ZChar format.
        /// We can pass any arbitrary array into here...so be careful
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public byte[] GetZChars(byte[] rawBytes, ref int startAddress)
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

        public string DecodeAbbreviationEntry(int startAddress)
        {
            var AllBytes = GetZChars(memory, ref startAddress);
            return DecodeZChars(AllBytes);
        }
    }
}
