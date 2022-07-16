﻿using System.Collections;
using Zmachine.V3;

namespace Zmachine.V3
{
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
                        int RevisionNumber) : this(Version, Flags1, HighMemoryStart, ProgramCounterInitalValue, DictionaryTable, ObjectTable, GlobalVariables, BaseOfStaticMemory, Flags2, RevisionNumber) { }
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
                        int RevisionNumber) : this(Version, Flags1, HighMemoryStart, ProgramCounterInitalValue, DictionaryTable, ObjectTable, GlobalVariables, BaseOfStaticMemory, Flags2, AbbreviationTable, RevisionNumber) { }
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
                int RevisionNumber) : this(Version, Flags1, HighMemoryStart, ProgramCounterInitalValue, DictionaryTable, ObjectTable, GlobalVariables, BaseOfStaticMemory, Flags2, LengthOfFile, ChecksumOfFile, AbbreviationTable, RevisionNumber) { }
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
                int HeaderExtensionTable) : this(Version, Flags1, HighMemoryStart, ProgramCounterInitalValue, DictionaryTable, ObjectTable, GlobalVariables, BaseOfStaticMemory, Flags2, LengthOfFile, ChecksumOfFile, InterpreterNumber, InterpreterVersion, ScreenHeightLines, ScreenWidthCharacters, AbbreviationTable, RevisionNumber) { }
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
        { }

        public static StoryHeader CreateHeader(Span<byte> Memory)
        {
            var version = Memory[0];
            var featuresVersion = MachineExtensions.GetFeatureVersion(version);

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
                _ => throw new ArgumentOutOfRangeException($"Version number not recognised: {version}");
            };
        }
    }
}
