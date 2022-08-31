using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zmachine.Library.V2.Implementation;
using Zmachine.Library.V2.Objects;

namespace Zmachine.Library.V2
{
    public class ZmachineTools
    {
        private readonly Machine machine;

        public ZmachineTools(Machine machine)
        {
            // This machine must already be prepped.
            this.machine = machine;
        }


        public void DumpDictionary()
        {
            for (var x = 0; x < this.machine.DictionaryTable.Length; ++x)
            {
                var dictionaryBytes = this.machine.DictionaryTable[x];
                // get the text
                Console.WriteLine(machine.TextDecoder.DecodeDictionaryEntry(dictionaryBytes));
            }
        }

        public void DumpAbbreviations()
        {
            for (var x = 0; x < machine.AbbreviationTable.Length; ++x)
            {
                var abbreviationAddress = machine.AbbreviationTable[x];
                // Do the thing with the text;
                Console.WriteLine($"{x}\t:\t'{machine.TextDecoder.DecodeAbbreviationEntry(abbreviationAddress, machine.GameData)}'");
            }
        }

        public void DumpObjects()
        {
            Console.WriteLine($"Total Objects : {machine.ObjectTable.TotalObjects}");
            Console.WriteLine($"Object Size : {machine.ObjectTable.ObjectSize}");

            // Lets get all the objects
            for (var x = 1; x <= machine.ObjectTable.TotalObjects; ++x)
            {
                var obj = machine.ObjectTable.GetObject((ushort)x);
                if (obj.PropertyTable.shortNameBytes.Length > 0)
                {
                    var zChars = machine.TextDecoder.GetZChars(obj.PropertyTable.shortNameBytes);
                    Console.WriteLine($"{x} {machine.TextDecoder.DecodeZChars(zChars)}");
                }
                else
                    Console.WriteLine($"{x} {{Empty Description}}");
                Console.WriteLine($"Parent:{obj.Parent}");
            }
        }

        public void DumpHeader()
        {
            {
                var version = machine.StoryHeader.Version;
                Console.WriteLine($"==== HEADER (Version: {version}) ====");
                Console.WriteLine($"Version:\t\t\t\t\t $00\t:\t{version}");
                Console.WriteLine($"Flags1:\t\t\t\t\t\t $01\t:\t{machine.StoryHeader.Flags1}");
                Console.WriteLine($"Flags2:\t\t\t\t\t\t $10\t:\t{machine.StoryHeader.Flags2}");
                Console.WriteLine($"Release Number:\t\t\t\t $02\t:\t{machine.StoryHeader.ReleaseNumber}");
                Console.WriteLine($"Revision Number:\t\t\t $32\t:\t{machine.StoryHeader.RevisionNumber}");
                Console.WriteLine($"High Memory Start:\t\t\t $04\t:\t{machine.StoryHeader.HighMemoryStart}\t:\t{machine.StoryHeader.HighMemoryStart:X4}");
                var pc = version != 6 ? "Program Counter (word)" : "'main' routine (packed)";
                Console.WriteLine($"{pc}:\t\t $06\t:\t{$"{machine.StoryHeader.ProgramCounterInitalValue}\t:\t{machine.StoryHeader.ProgramCounterInitalValue:X4}"}");
                Console.WriteLine($"Dictionary table:\t\t\t $08\t:\t{machine.StoryHeader.DictionaryTable}\t:\t{machine.StoryHeader.DictionaryTable:X4}");
                Console.WriteLine($"Object table:\t\t\t\t $0A\t:\t{machine.StoryHeader.ObjectTable}\t\t:\t{machine.StoryHeader.ObjectTable:X4}");
                Console.WriteLine($"Global variables:\t\t\t $0C\t:\t{machine.StoryHeader.GlobalVariables}\t:\t{machine.StoryHeader.GlobalVariables:X4}");
                Console.WriteLine($"Static Memory start:\t\t $0E\t:\t{machine.StoryHeader.BaseOfStaticMemory}\t:\t{machine.StoryHeader.BaseOfStaticMemory:X4}");
                if (version > 1)
                    Console.WriteLine($"Abbreviations table:\t\t $18\t:\t{machine.StoryHeader.AbbreviationTable}\t\t:\t{machine.StoryHeader.AbbreviationTable:X4}");
                if (version > 2)
                {
                    Console.WriteLine($"File length (divide by 4):\t $1A\t:\t{machine.StoryHeader.LengthOfFile}\t:\t{machine.StoryHeader.LengthOfFile:X4}");
                    Console.WriteLine($"Checksum:\t\t\t\t\t $1C\t:\t{machine.StoryHeader.ChecksumOfFile}\t:\t{machine.StoryHeader.ChecksumOfFile:X4}");
                }
                if (version > 3)
                {
                    Console.WriteLine($"Interpreter number:\t\t\t $1E\t:\t{machine.StoryHeader.InterpreterNumber}");
                    Console.WriteLine($"Interpreter version:\t\t $1F\t:\t{machine.StoryHeader.InterpreterVersion}");
                    Console.WriteLine($"Screen Height(Lines):\t\t $20\t:\t{machine.StoryHeader.ScreenHeightLines}");
                    Console.WriteLine($"Screen Width(Chars):\t\t $21\t:\t{machine.StoryHeader.ScreenWidthCharacters}");
                }
                if (version > 4)
                {
                    Console.WriteLine($"Screen Width(Width):\t\t $22\t:\t{machine.StoryHeader.ScreenWidthUnits}");
                    Console.WriteLine($"Screen Height(Units):\t\t $24\t:\t{machine.StoryHeader.ScreenHeightUnits}");
                    Console.WriteLine($"Font Width('0'):\t\t\t {(machine.StoryHeader.Version == 5 ? "$26" : "$27")}\t:\t{machine.StoryHeader.FontWidthUnits}");
                    Console.WriteLine($"Font Height(Units):\t\t\t {(machine.StoryHeader.Version == 5 ? "$27" : "$26")}\t:\t{machine.StoryHeader.FontHeightUnits}");
                }

                if (version > 5)
                {
                    Console.WriteLine($"Routines offset (div by 8):\t $28\t:\t{machine.StoryHeader.RoutinesOffset}\t:\t{machine.StoryHeader.RoutinesOffset:X4}");
                    Console.WriteLine($"Strings offset (div by 8):\t $2A\t:\t{machine.StoryHeader.StaticStringsOffset}\t:\t{machine.StoryHeader.StaticStringsOffset:X4}");
                }
                if (version > 4)
                {
                    Console.WriteLine($"Default background colour:\t $2C\t:\t{machine.StoryHeader.DefaultBackground}\t:\t{machine.StoryHeader.DefaultBackground:X4}");
                    Console.WriteLine($"Default foreground colour:\t $2D\t:\t{machine.StoryHeader.DefaultForeground}\t:\t{machine.StoryHeader.DefaultForeground:X4}");
                    Console.WriteLine($"Terminating chars table:\t $2E\t:\t{machine.StoryHeader.TerminatingCharsTable}\t:\t{machine.StoryHeader.TerminatingCharsTable:X4}");
                }

                if (version > 5)
                    Console.WriteLine($"Pixels sent to Stream(Width):\t $34\t:\t{machine.StoryHeader.Stream3PixelsWidth}\t:\t{machine.StoryHeader.Stream3PixelsWidth:X4}");

                if (version > 4)
                {
                    Console.WriteLine($"Alphabet table:\t\t\t\t $34\t:\t{machine.StoryHeader.AlphabetTable}\t:\t{machine.StoryHeader.AlphabetTable:X4}");
                    Console.WriteLine($"Header extension table:\t\t $36\t:\t{machine.StoryHeader.HeaderExtensionTable}\t:\t{machine.StoryHeader.HeaderExtensionTable:X4}");
                }

                if (version == 2)
                    Console.WriteLine($"Serial Code:\t\t\t\t $12\t:\t{machine.StoryHeader.SerialCode}");
                else if (version > 2)
                    Console.WriteLine($"Date Compiled:\t\t\t\t $12\t:\t{machine.StoryHeader.SerialCode}");

                if (version > 5)
                {
                    Console.WriteLine($"Username:\t\t\t\t $12\t:\t{machine.StoryHeader.Username}");
                    Console.WriteLine($"Invform version:\t\t\t $12\t:\t{machine.StoryHeader.InformVersion}");
                }
            }

        }

        /// <summary>
        /// Takes game encoded bytes finds the zchars and retursn the text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string DecodeEncodedText(byte[] text)
        {
            var startAddress = 0;
            var chars = TextProcessor.GetZChars(text, ref startAddress);
            return machine.TextDecoder.DecodeZChars(chars);
        }

        /// <summary>
        /// Creates the string from zchars encdoded bytes
        /// </summary>
        /// <param name="zchars"></param>
        /// <returns></returns>
        public string DecodeZChars(byte[] zchars)
        {
            return machine.TextDecoder.DecodeZChars(zchars);
        }

        /// <summary>
        /// encodes to Zhcars 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public byte[] EncodeToZChars(string text)
        {
            var bytes = machine.TextDecoder.EncodeUtf8ZChars(text);
            return bytes;
        }

        /// <summary>
        /// Encodes Zchars to story format
        /// </summary>
        /// <param name="zchars"></param>
        /// <returns></returns>
        public byte[] EncodeWords(byte[] zchars)
        {
            var bytes = machine.TextDecoder.EncodeZcharsToWords(zchars);
            return bytes;
        }

        /// <summary>
        /// Get Get an object.
        /// </summary>
        /// <param name="objectIdx"></param>
        /// <returns></returns>
        public ZmObject GetObject(ushort objectIdx) => machine.ObjectTable[objectIdx];

        public ushort GetGlobalVariable(int globalVariableIdx) => machine.GlobalVariables[globalVariableIdx];
    }
}
