using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2
{
    // Default v1 header
    public record Headerez(int Version,
                           BitArray Flags1,
                           int HighMemoryStart,
                           int ProgramCounterInitalValue,
                           int Dictionary,
                           int ObjectTable,
                           int GlobalVariables,
                           int BaseOfStaticMemory,
                           BitArray Flags2,
                           int RevisionNumber)
    {
        // v2 Header
        public Headerez(int Version,
                        BitArray Flags1,
                        int HighMemoryStart,
                        int ProgramCounterInitalValue,
                        int Dictionary,
                        int ObjectTable,
                        int GlobalVariables,
                        int BaseOfStaticMemory,
                        BitArray Flags2,
                        int AbbreviationTable,
                        int RevisionNumber) : this(Version, Flags1, HighMemoryStart, ProgramCounterInitalValue, Dictionary, ObjectTable, GlobalVariables, BaseOfStaticMemory, Flags2, RevisionNumber) { }
        // v3 Header
        public Headerez(int Version,
                        BitArray Flags1,
                        int HighMemoryStart,
                        int ProgramCounterInitalValue,
                        int Dictionary,
                        int ObjectTable,
                        int GlobalVariables,
                        int BaseOfStaticMemory,
                        BitArray Flags2,
                        int LengthOfFile,
                        int ChecksumOfFile,
                        int AbbreviationTable,
                        int RevisionNumber) : this(Version, Flags1, HighMemoryStart, ProgramCounterInitalValue, Dictionary, ObjectTable, GlobalVariables, BaseOfStaticMemory, Flags2, AbbreviationTable, RevisionNumber) { }
        // v4
        public Headerez(int Version,
                BitArray Flags1,
                int HighMemoryStart,
                int ProgramCounterInitalValue,
                int Dictionary,
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
                int RevisionNumber) : this(Version, Flags1, HighMemoryStart, ProgramCounterInitalValue, Dictionary, ObjectTable, GlobalVariables, BaseOfStaticMemory, Flags2, LengthOfFile, ChecksumOfFile, AbbreviationTable, RevisionNumber) { }
        // v5
        public Headerez(int Version,
                BitArray Flags1,
                int HighMemoryStart,
                int ProgramCounterInitalValue,
                int Dictionary,
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
                int HeaderExtensionTable) : this(Version, Flags1, HighMemoryStart, ProgramCounterInitalValue, Dictionary, ObjectTable, GlobalVariables, BaseOfStaticMemory, Flags2, LengthOfFile, ChecksumOfFile, InterpreterNumber, InterpreterVersion, ScreenHeightLines, ScreenWidthCharacters, AbbreviationTable, RevisionNumber) { }
        //v6 
        public Headerez(int Version,
                BitArray Flags1,
                int HighMemoryStart,
                int ProgramCounterInitalValue,
                int Dictionary,
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
                int HeaderExtensionTable) : this(Version, Flags1, HighMemoryStart, ProgramCounterInitalValue, Dictionary, ObjectTable, GlobalVariables, BaseOfStaticMemory, Flags2, LengthOfFile, ChecksumOfFile, InterpreterNumber, InterpreterVersion, ScreenHeightLines, ScreenWidthCharacters,
                        ScreenWidthUnits,
                ScreenHeightUnits,
                FontWidthUnits,
                FontHeightUnits,
                DefaultBackground,
                DefaultForeground,
                TerminatingCharsTable,
                    AbbreviationTable, RevisionNumber, AlphabetTable, HeaderExtensionTable)
        { }



    }

    public struct MachineHeader
    {
        public string ReleaseNumber { get; internal set; }
        public ushort HighMemStart { get; internal set; }
        public byte InterpreterVersion { get; internal set; }
        public byte InterpreterNumber { get; internal set; }
        public int Version { get; set; }
        public int ProgramCounterInitalValue { get; set; }
        public int Dictionary { get; set; }
        public int ObjectTable { get; set; }
        public int GlobalObjects { get; set; }
        public int StaticStart { get; set; }
        public BitArray Flags { get; set; }
        public BitArray Flags2 { get; set; }
        public int AbbreviationsTable { get; set; }
        public int LengthOfFile { get; set; }
        public string DateCompiled { get; set; }
        public ushort StaticStringsOffSet { get; set; }
        public ushort RoutinesOffset { get; set; }

    }
}
