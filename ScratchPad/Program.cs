using Zmachine.V2;
using Zmachine.V2.Machines;

namespace ScratchPath
{
    public static class Program
    {
        public static void Main(string[] args)
        {


            var newString = ZmTextDecoder.DecodeDictionaryEntry(new byte[]{
                //0x34, 0xCE, 0xB0,0x00,0x00,0x00
                0x25,
0xE3,
0x57,
0x0f,
0x1b,
0x05,
0x88,
0xA8,
0x5E,
0x02,
0x2D,
            });

            var newString2 = ZmTextDecoder.DecodeDictionaryEntry(new byte[]{
                0xFa, 0x9a
            });

            Console.WriteLine(newString);
            Console.WriteLine(newString2);


            var filename = "curses.z5";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            Zmachine_v2 v2 = new(fileStream);
            //var word =  textEncoded.ZmDecodeBytes(abbV1);
            //Console.WriteLine(word);
            while (true)
            {
                var itm = Console.ReadKey(true);
                switch (itm.Key)
                {
                    case ConsoleKey.Escape: Console.WriteLine("Exiting"); return;
                    case ConsoleKey.D1: v2.DumpHeader(); break;
                    case ConsoleKey.D2: v2.DumpDictionary(); break;
                    //case ConsoleKey.D2: v1.DumpDynamic(); break;
                    case ConsoleKey.D3: v2.DumpObjects(); break;
                    case ConsoleKey.D4:  break;
                    case ConsoleKey.D5: v2.Tick(); break;
                    case ConsoleKey.D6: v2.Disassemble(); break;
                }

            }
        }





        //var allChars = EncodeChars();

        //// we need to turn that list of chars into z=machine, 'words'.
        //// 2 byte objcets (16 bit)
        //// high  bit set at end string
        //// 3x5 bit words
        //List<ushort> allZChars = new List<ushort>();
        //for (var x = 0; x < allChars.Count; x += 3)
        //{

        //    BitArray bitArray = new BitArray(16);
        //    // Are we at the end of the sequence
        //    bitArray.Set(15, (allChars.Count <= x + 3) ? true : false);
        //    //top 
        //    SetBitArrayRange(bitArray, allChars[x].CharCode, 10);
        //    // middle
        //    SetBitArrayRange(bitArray, (x + 1 < allChars.Count) ? allChars[x + 1].CharCode : (byte)'5', 5);
        //    //bottom
        //    SetBitArrayRange(bitArray, (x + 2 < allChars.Count ) ? allChars[x + 2].CharCode : (byte)'5', 0);

        //    int[] result = new int[1];
        //    bitArray.CopyTo(result, 0);
        //    allZChars.Add((ushort)result[0]);
        //}


        //allZChars.ToList();

        //void SetBitArrayRange(BitArray bitArray, byte @byte, int startIndex)
        //{
        //    for (int x = 0; x < 4; ++x)
        //    {
        //        var bit = ((@byte >> x) & 1) == 1;
        //        bitArray.Set(startIndex + x, bit);
        //    }
        //}


        //List<EncodedChar> EncodeChars()
        //{
        //    List<EncodedChar> encodedChars = new List<EncodedChar>();

        //    //Encoding each character with 
        //    foreach (var @char in inputChars)
        //    {
        //        AlphabetType alphaType = AlphabetType.Lower;
        //        if (Char.IsUpper(@char))
        //            alphaType = AlphabetType.Upper;
        //        else
        //        {
        //            if (Char.IsDigit(@char) || char.IsPunctuation(@char) || (byte)@char == 13)
        //                alphaType = AlphabetType.Punctuation;
        //        }

        //        if (alphaType != AlphabetType.Punctuation)
        //        {
        //            // This coverts into Ascii lowercase.
        //            // Lowercase. and uppercase alphabets have the same index.
        //            // lowercase happens more often
        //            var charCode = alphaType == AlphabetType.Lower ? (byte)@char : (byte)@char + 26;
        //            // Ascii 'a' = 
        //            var zCharCode = charCode - 91;

        //            encodedChars.Add(new EncodedChar
        //            {
        //                alphabet = alphaType,
        //                CharCode = (byte)zCharCode
        //            });
        //        }
        //        else
        //        {
        //            var zCharCode = ZCharDictionaries.A2V3.ContainsKey(@char) ? ZCharDictionaries.A2V3[@char] : ZCharDictionaries.A2V3['?'];
        //            encodedChars.Add(new EncodedChar
        //            {
        //                alphabet = alphaType,
        //                CharCode = (byte)zCharCode
        //            });
        //        }
        //    }
        //    return encodedChars;
        //}
    }
}
