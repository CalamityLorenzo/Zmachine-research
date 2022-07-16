
namespace ZMachine.Library.V1
{
    using System.Collections;
    using ZMachine.Library.V1.Utilities;
    // Default v1 header
    public record StoryHeader(int Version,
                           BitArray Flags1,
                           int HighMemoryStart,
                           int ProgramCounterInitalValue,
                           int DictionaryTable,
                           int ObjectTable,
                           int GlobalVariables,
                           int BaseOfStaticMemory,
                           BitArray Flags2,
                           int RevisionNumber)
    {
        public int AbbreviationTable { get; }
        public int LengthOfFile { get; }
        public int ChecksumOfFile { get; }
        public int InterpreterNumber { get; }
        public int InterpreterVersion { get; }
        public int ScreenHeightLines { get; }
        public int ScreenWidthCharacters { get; }
        public int ScreenWidthUnits { get; }
        public int ScreenHeightUnits { get; }
        public int FontWidthUnits { get; }
        public int FontHeightUnits { get; }
        public int DefaultBackground { get; }
        public int DefaultForeground { get; }
        public int TerminatingCharsTable { get; }
        public int AlphabetTable { get; }
        public int HeaderExtensionTable { get; }
        public int RoutinesOffset { get; }
        public int StaticStringsOffset { get; }
        public int Stream3PixelsWidth { get; }

        // v2 Header
        public StoryHeader(int Version,
                        BitArray Flags1,
                        int HighMemoryStart,
                        int ProgramCounterInitalValue,
                        int DictionaryTable,
                        int ObjectTable,
                        int GlobalVariables,
                        int BaseOfStaticMemory,
                        BitArray Flags2,
                        int AbbreviationTable,
                        int RevisionNumber) : this(Version, Flags1, HighMemoryStart, ProgramCounterInitalValue, DictionaryTable, ObjectTable, GlobalVariables, BaseOfStaticMemory, Flags2, RevisionNumber)
        {
            this.AbbreviationTable = AbbreviationTable;
        }
        // v3 Header
        public StoryHeader(int Version,
                        BitArray Flags1,
                        int HighMemoryStart,
                        int ProgramCounterInitalValue,
                        int DictionaryTable,
                        int ObjectTable,
                        int GlobalVariables,
                        int BaseOfStaticMemory,
                        BitArray Flags2,
                        int LengthOfFile,
                        int ChecksumOfFile,
                        int AbbreviationTable,
                        int RevisionNumber) : this(Version, Flags1, HighMemoryStart, ProgramCounterInitalValue, DictionaryTable, ObjectTable, GlobalVariables, BaseOfStaticMemory, Flags2, AbbreviationTable, RevisionNumber)
        {
            this.LengthOfFile = LengthOfFile;
            this.ChecksumOfFile = ChecksumOfFile;
        }
        // v4
        public StoryHeader(int Version,
                BitArray Flags1,
                int HighMemoryStart,
                int ProgramCounterInitalValue,
                int DictionaryTable,
                int ObjectTable,
                int GlobalVariables,
                int BaseOfStaticMemory,
                BitArray Flags2,
                int LengthOfFile,
                int ChecksumOfFile,
                int InterpreterNumber,
                int InterpreterVersion,
                int ScreenHeightLines,
                int ScreenWidthCharacters,
                int AbbreviationTable,
                int RevisionNumber) : this(Version, Flags1, HighMemoryStart, ProgramCounterInitalValue, DictionaryTable, ObjectTable, GlobalVariables, BaseOfStaticMemory, Flags2, LengthOfFile, ChecksumOfFile, AbbreviationTable, RevisionNumber)
        {
            this.InterpreterNumber = InterpreterNumber;
            this.InterpreterVersion = InterpreterVersion;
            this.ScreenHeightLines = ScreenHeightLines;
            this.ScreenWidthCharacters = ScreenWidthCharacters;
        }
        // v5
        public StoryHeader(int Version,
                BitArray Flags1,
                int HighMemoryStart,
                int ProgramCounterInitalValue,
                int DictionaryTable,
                int ObjectTable,
                int GlobalVariables,
                int BaseOfStaticMemory,
                BitArray Flags2,
                int LengthOfFile,
                int ChecksumOfFile,
                int InterpreterNumber,
                int InterpreterVersion,
                int ScreenHeightLines,
                int ScreenWidthCharacters,
                int ScreenWidthUnits,
                int ScreenHeightUnits,
                int FontWidthUnits, // width of '0'
                int FontHeightUnits,
                int DefaultBackground,
                int DefaultForeground,
                int TerminatingCharsTable, // address
                int AbbreviationTable,
                int RevisionNumber,
                int AlphabetTable,
                int HeaderExtensionTable) : this(Version, Flags1, HighMemoryStart, ProgramCounterInitalValue, DictionaryTable, ObjectTable, GlobalVariables, BaseOfStaticMemory, Flags2, LengthOfFile, ChecksumOfFile, InterpreterNumber, InterpreterVersion, ScreenHeightLines, ScreenWidthCharacters, AbbreviationTable, RevisionNumber)
        {
            this.ScreenWidthUnits = ScreenWidthUnits;
            this.ScreenHeightUnits = ScreenHeightUnits;
            this.FontWidthUnits = FontWidthUnits;
            this.FontHeightUnits = FontHeightUnits;
            this.DefaultBackground = DefaultBackground;
            this.DefaultForeground = DefaultForeground;
            this.TerminatingCharsTable = TerminatingCharsTable;
            this.AlphabetTable = AlphabetTable;
            this.HeaderExtensionTable = HeaderExtensionTable;
        }
        //v6 
        public StoryHeader(int Version,
                BitArray Flags1,
                int HighMemoryStart,
                int ProgramCounterInitalValue,
                int DictionaryTable,
                int ObjectTable,
                int GlobalVariables,
                int BaseOfStaticMemory,
                BitArray Flags2,
                int LengthOfFile,
                int ChecksumOfFile,
                int InterpreterNumber,
                int InterpreterVersion,
                int ScreenHeightLines,
                int ScreenWidthCharacters,
                int ScreenWidthUnits,
                int ScreenHeightUnits,
                int FontWidthUnits, /// v6 = v5 fontHeight memory location
                int FontHeightUnits, /// v6 =v5 fontWidth memory location
                int RoutinesOffset, /// divided by 8
                int StaticStringsOffset, /// Divided by 8
                int DefaultBackground,
                int DefaultForeground,
                int TerminatingCharsTable,
                int Stream3PixelsWidth,
                int AbbreviationTable,
                int RevisionNumber,
                int AlphabetTable,
                int HeaderExtensionTable) : this(Version, Flags1, HighMemoryStart, ProgramCounterInitalValue, DictionaryTable, ObjectTable, GlobalVariables, BaseOfStaticMemory, Flags2, LengthOfFile, ChecksumOfFile, InterpreterNumber, InterpreterVersion, ScreenHeightLines, ScreenWidthCharacters, ScreenWidthUnits, ScreenHeightUnits, FontWidthUnits, FontHeightUnits, DefaultBackground, DefaultForeground, TerminatingCharsTable, AbbreviationTable, RevisionNumber, AlphabetTable, HeaderExtensionTable)
        {
            this.RoutinesOffset = RoutinesOffset;
            this.StaticStringsOffset = StaticStringsOffset;
            this.Stream3PixelsWidth = Stream3PixelsWidth;
        }

        public static StoryHeader CreateHeader(Span<byte> Memory)
        {
            var version = Memory[0];
            var featuresVersion = LibraryUtilities.GetFeatureVersion(version);

            return version switch
            {
                1 => new StoryHeader(
                    Version: version,
                    Flags1: new BitArray(Memory[1]),
                    HighMemoryStart: Memory.Get2ByteValue(4),
                    ProgramCounterInitalValue: Memory.Get2ByteValue(6),
                    DictionaryTable: Memory.Get2ByteValue(8),
                    ObjectTable: Memory.Get2ByteValue(10),
                    GlobalVariables: Memory.Get2ByteValue(12),
                    BaseOfStaticMemory: Memory.Get2ByteValue(14),
                    Flags2: new BitArray(Memory[16]),
                    RevisionNumber: Memory[54]),
                2 => new StoryHeader(
                     Version: version,
                     Flags1: new BitArray(Memory[1]),
                     HighMemoryStart: Memory.Get2ByteValue(4),
                     ProgramCounterInitalValue: Memory.Get2ByteValue(6),
                     DictionaryTable: Memory.Get2ByteValue(8),
                     ObjectTable: Memory.Get2ByteValue(10),
                     GlobalVariables: Memory.Get2ByteValue(12),
                     BaseOfStaticMemory: Memory.Get2ByteValue(14),
                     Flags2: new BitArray(Memory[16]),
                     AbbreviationTable: Memory.Get2ByteValue(24),
                     RevisionNumber: Memory[54]),
                3 => new StoryHeader(
                        Version: version,
                        Flags1: new BitArray(Memory[1]),
                        HighMemoryStart: Memory.Get2ByteValue(4),
                        ProgramCounterInitalValue: Memory.Get2ByteValue(6),
                        DictionaryTable: Memory.Get2ByteValue(8),
                        ObjectTable: Memory.Get2ByteValue(10),
                        GlobalVariables: Memory.Get2ByteValue(12),
                        BaseOfStaticMemory: Memory.Get2ByteValue(14),
                        Flags2: new BitArray(Memory[16]),
                        AbbreviationTable: Memory.Get2ByteValue(24),
                        LengthOfFile: Memory.Get2ByteValue(26),
                        ChecksumOfFile: Memory.Get2ByteValue(28),
                        RevisionNumber: Memory[54]),
                4 => new StoryHeader(
                        Version: version,
                        Flags1: new BitArray(Memory[1]),
                        HighMemoryStart: Memory.Get2ByteValue(4),
                        ProgramCounterInitalValue: Memory.Get2ByteValue(6),
                        DictionaryTable: Memory.Get2ByteValue(8),
                        ObjectTable: Memory.Get2ByteValue(10),
                        GlobalVariables: Memory.Get2ByteValue(12),
                        BaseOfStaticMemory: Memory.Get2ByteValue(14),
                        Flags2: new BitArray(Memory[16]),
                        AbbreviationTable: Memory.Get2ByteValue(24),
                        LengthOfFile: Memory.Get2ByteValue(26),
                        ChecksumOfFile: Memory.Get2ByteValue(28),
                        InterpreterNumber: Memory[30],
                        InterpreterVersion: Memory[31],
                        ScreenHeightLines: Memory[32],
                        ScreenWidthCharacters: Memory[33],
                        RevisionNumber: Memory[54]),
                5 => new StoryHeader(
                        Version: version,
                        Flags1: new BitArray(Memory[1]),
                        HighMemoryStart: Memory.Get2ByteValue(4),
                        ProgramCounterInitalValue: Memory.Get2ByteValue(6),
                        DictionaryTable: Memory.Get2ByteValue(8),
                        ObjectTable: Memory.Get2ByteValue(10),
                        GlobalVariables: Memory.Get2ByteValue(12),
                        BaseOfStaticMemory: Memory.Get2ByteValue(14),
                        Flags2: new BitArray(Memory[16]),
                        AbbreviationTable: Memory.Get2ByteValue(24),
                        LengthOfFile: Memory.Get2ByteValue(26),
                        ChecksumOfFile: Memory.Get2ByteValue(28),
                        InterpreterNumber: Memory[30],
                        InterpreterVersion: Memory[31],
                        ScreenHeightLines: Memory[32],
                        ScreenWidthCharacters: Memory[33],
                        ScreenWidthUnits: Memory.Get2ByteValue(34),
                        ScreenHeightUnits: Memory.Get2ByteValue(36),
                        FontWidthUnits: Memory[38],
                        FontHeightUnits: Memory[39],
                        DefaultBackground: Memory[44],
                        DefaultForeground: Memory[45],
                        TerminatingCharsTable: Memory.Get2ByteValue(46),
                        RevisionNumber: Memory[54],
                        AlphabetTable: Memory.Get2ByteValue(52),
                        HeaderExtensionTable: Memory.Get2ByteValue(54)),
                6 => new StoryHeader(
                        Version: version,
                        Flags1: new BitArray(Memory[1]),
                        HighMemoryStart: Memory.Get2ByteValue(4),
                        ProgramCounterInitalValue: Memory.Get2ByteValue(6),
                        DictionaryTable: Memory.Get2ByteValue(8),
                        ObjectTable: Memory.Get2ByteValue(10),
                        GlobalVariables: Memory.Get2ByteValue(12),
                        BaseOfStaticMemory: Memory.Get2ByteValue(14),
                        Flags2: new BitArray(Memory[16]),
                        AbbreviationTable: Memory.Get2ByteValue(24),
                        LengthOfFile: Memory.Get2ByteValue(26),
                        ChecksumOfFile: Memory.Get2ByteValue(28),
                        InterpreterNumber: Memory[30],
                        InterpreterVersion: Memory[31],
                        ScreenHeightLines: Memory[32],
                        ScreenWidthCharacters: Memory[33],
                        ScreenWidthUnits: Memory.Get2ByteValue(34),
                        ScreenHeightUnits: Memory.Get2ByteValue(36),
                        FontWidthUnits: Memory[39], // Swapped between v5 and 6
                        FontHeightUnits: Memory[38], // Swapped between v5 and 6
                        RoutinesOffset: Memory.Get2ByteValue(40), // Divided by 8
                        StaticStringsOffset: Memory.Get2ByteValue(42),
                        DefaultBackground: Memory[44],
                        DefaultForeground: Memory[45],
                        TerminatingCharsTable: Memory.Get2ByteValue(46),
                        Stream3PixelsWidth: Memory[48],
                        RevisionNumber: Memory[54],
                        AlphabetTable: Memory.Get2ByteValue(52),
                        HeaderExtensionTable: Memory.Get2ByteValue(54)),
                _ => throw new ArgumentOutOfRangeException($"Version number not recognised: {version}"),
            };
        }
    }
}
