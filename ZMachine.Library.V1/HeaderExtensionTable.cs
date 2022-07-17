using System.Collections;

namespace ZMachine.Library.V1
{
    public record HeaderExtensionTable(
            int NumberOfWords,
            int XCoordsPostClick,
            int YCoordsPostClick,
            int UnicodeTranslationTableAddress,
            BitArray Flags3)
    {

        public static HeaderExtensionTable CreateHeaderExtenions(byte[] Memory, int startAddress)
        {
            var wordCount = Memory[startAddress];
            return new HeaderExtensionTable(
                                    NumberOfWords: wordCount,
                                    XCoordsPostClick: wordCount > 0 ? Memory[startAddress + 1] : 0,
                                    YCoordsPostClick: wordCount > 1 ? Memory[startAddress + 2] : 0,
                                    UnicodeTranslationTableAddress: wordCount > 2 ? Memory[startAddress + 3] : 0,
                                    Flags3: wordCount > 2 ? new BitArray(Memory[startAddress + 3]):new BitArray(Enumerable.Empty<byte>().ToArray()) );



        }
    }
}
