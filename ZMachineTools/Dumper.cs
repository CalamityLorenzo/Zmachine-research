using ZMachine.Library.V1;
using ZMachine.Library.V1.Objects;
using ZMachine.Library.V1.Utilities;

namespace ZMachineTools
{
    public class Tools
    {
        private MemoryStream StoryData;
        private byte[] Memory;
        private StoryHeader StoryHeader;
        private FeaturesVersion FeaturesVersion;
        private DictionaryTable DictionaryTable;

        public AbbreviationsTable AbbreviationTable;

        private TextProcessor TextDecoder;
        private ObjectTable ObjectTable;

        public HeaderExtensionTable? HeaderExtensions;

        public Tools(Stream storyData)
        {
            // Here's an assumption...
            storyData.Position = 0;
            StoryData = new MemoryStream();
            storyData.CopyTo(StoryData);
            StoryData.Position = 0;

            Memory = new byte[StoryData.Length];
            // Nom, nom nom game data.
            StoryData.Read(Memory, 0, Memory.Length);
            this.StoryHeader = StoryHeader.CreateHeader(Memory);
            if (this.StoryHeader.HeaderExtensionTable != 0)
            {
                this.HeaderExtensions = HeaderExtensionTable.CreateHeaderExtenions(Memory, this.StoryHeader.HeaderExtensionTable);
            }
            this.FeaturesVersion = LibraryUtilities.GetFeatureVersion(StoryHeader.Version);
            this.DictionaryTable = new DictionaryTable(StoryHeader.Version, StoryHeader.DictionaryTable, Memory);
            this.AbbreviationTable = new AbbreviationsTable(StoryHeader.AbbreviationTable, Memory, StoryHeader.Version);
            this.TextDecoder = new TextProcessor(Memory, AbbreviationTable, StoryHeader.Version);
            this.ObjectTable = new ObjectTable(this.StoryHeader.ObjectTable, this.StoryHeader.Version, Memory);
        }

        public void Dictionary()
        {
            this.DictionaryTable = new DictionaryTable(StoryHeader.Version, this.StoryHeader.DictionaryTable, Memory);

            for (var x = 0; x < this.DictionaryTable.Length; ++x)
            {
                // All the bytes
                var dictionaryBytes = this.DictionaryTable[x];
                // get the text
                Console.WriteLine(this.TextDecoder.DecodeDictionaryEntry(dictionaryBytes));
            }
        }

        public void Abbreviations()
        {
            for (var x = 0; x < AbbreviationTable.Length; ++x)
            {
                var abbreviationAddress = this.AbbreviationTable[x];
                // Do the thing with the text;
                Console.WriteLine($"{x}\t:\t'{this.TextDecoder.DecodeAbbreviationEntry(abbreviationAddress)}'");
            }

            /*
            for (var y = 1; y < 4; ++y)
            {
                for (var x = 1; x < 33; ++x)
                {
                    //var abbreviationAddress = this.AbbreviationTable[x];
                    var abbreviationAddress = this.AbbreviationTable.GetEntryAddress(y, x);
                    // Do the thing with the text;
                    Console.WriteLine(this.TextDecoder.DecodeAbbreviationEntry(abbreviationAddress));
                }
            }
            */

        }

        public void Objects()
        {
            Console.WriteLine($"Total Objects : {this.ObjectTable.TotalObjects}");
            Console.WriteLine($"Object Size : {this.ObjectTable.ObjectSize}");

            // Lets get all the objects
            for (var x = 1; x <= this.ObjectTable.TotalObjects; ++x)
            {
                var obj = this.ObjectTable.GetObject((ushort)x);
                if (obj.PropertyTable.shortNameBytes.Length > 0)
                {
                    var zChars = this.TextDecoder.GetZChars(obj.PropertyTable.shortNameBytes);
                    Console.WriteLine($"{x} {this.TextDecoder.DecodeZChars(zChars)}");
                }
                else
                    Console.WriteLine($"{x} {{Empty Description}}");
                Console.WriteLine($"Parent:{obj.Parent}");               //for(var y=0;y< obj.PropertyTable.Properties.Length; ++y)
                //{
                //    Console.WriteLine(obj.PropertyTable.Properties[y]);
                //}
            }
        }

        /// <summary>
        /// Dump out only the relevant info per verion
        /// </summary>
        public void Header()
        {
            var version = this.StoryHeader.Version;
            Console.WriteLine($"==== HEADER (Version: {version}) ====");
            Console.WriteLine($"Version:\t\t\t\t\t $00\t:\t{version}");
            Console.WriteLine($"Flags1:\t\t\t\t\t\t $01\t:\t{this.StoryHeader.Flags1}");
            Console.WriteLine($"Flags2:\t\t\t\t\t\t $10\t:\t{this.StoryHeader.Flags2}");
            Console.WriteLine($"Release Number:\t\t\t\t $02\t:\t{this.StoryHeader.ReleaseNumber}");
            Console.WriteLine($"Revision Number:\t\t\t $32\t:\t{this.StoryHeader.RevisionNumber}");
            Console.WriteLine($"High Memory Start:\t\t\t $04\t:\t{this.StoryHeader.HighMemoryStart}\t:\t{this.StoryHeader.HighMemoryStart:X4}");
            var pc = version != 6 ? "Program Counter (word)" : "'main' routine (packed)";
            Console.WriteLine($"{pc}:\t\t $06\t:\t{$"{this.StoryHeader.ProgramCounterInitalValue}\t:\t{this.StoryHeader.ProgramCounterInitalValue:X4}"}");
            Console.WriteLine($"Dictionary table:\t\t\t $08\t:\t{this.StoryHeader.DictionaryTable}\t:\t{this.StoryHeader.DictionaryTable:X4}");
            Console.WriteLine($"Object table:\t\t\t\t $0A\t:\t{this.StoryHeader.ObjectTable}\t\t:\t{this.StoryHeader.ObjectTable:X4}");
            Console.WriteLine($"Global variables:\t\t\t $0C\t:\t{this.StoryHeader.GlobalVariables}\t:\t{this.StoryHeader.GlobalVariables:X4}");
            Console.WriteLine($"Static Memory start:\t\t $0E\t:\t{this.StoryHeader.BaseOfStaticMemory}\t:\t{this.StoryHeader.BaseOfStaticMemory:X4}");
            if (version > 1)
                Console.WriteLine($"Abbreviations table:\t\t $18\t:\t{this.StoryHeader.AbbreviationTable}\t\t:\t{this.StoryHeader.AbbreviationTable:X4}");
            if (version > 2)
            {
                Console.WriteLine($"File length (divide by 4):\t $1A\t:\t{this.StoryHeader.LengthOfFile}\t:\t{this.StoryHeader.LengthOfFile:X4}");
                Console.WriteLine($"Checksum:\t\t\t\t\t $1C\t:\t{this.StoryHeader.ChecksumOfFile}\t:\t{this.StoryHeader.ChecksumOfFile:X4}");
            }
            if (version > 3)
            {
                Console.WriteLine($"Interpreter number:\t\t\t $1E\t:\t{this.StoryHeader.InterpreterNumber}");
                Console.WriteLine($"Interpreter version:\t\t $1F\t:\t{this.StoryHeader.InterpreterVersion}");
                Console.WriteLine($"Screen Height(Lines):\t\t $20\t:\t{this.StoryHeader.ScreenHeightLines}");
                Console.WriteLine($"Screen Width(Chars):\t\t $21\t:\t{this.StoryHeader.ScreenWidthCharacters}");
            }
            if (version > 4)
            {
                Console.WriteLine($"Screen Width(Width):\t\t $22\t:\t{this.StoryHeader.ScreenWidthUnits}");
                Console.WriteLine($"Screen Height(Units):\t\t $24\t:\t{this.StoryHeader.ScreenHeightUnits}");
                Console.WriteLine($"Font Width('0'):\t\t\t {(StoryHeader.Version == 5 ? "$26" : "$27")}\t:\t{this.StoryHeader.FontWidthUnits}");
                Console.WriteLine($"Font Height(Units):\t\t\t {(StoryHeader.Version == 5 ? "$27" : "$26")}\t:\t{this.StoryHeader.FontHeightUnits}");
            }

            if (version > 5)
            {
                Console.WriteLine($"Routines offset (div by 8):\t $28\t:\t{this.StoryHeader.RoutinesOffset}\t:\t{this.StoryHeader.RoutinesOffset:X4}");
                Console.WriteLine($"Strings offset (div by 8):\t $2A\t:\t{this.StoryHeader.StaticStringsOffset}\t:\t{this.StoryHeader.StaticStringsOffset:X4}");
            }
            if (version > 4)
            {
                Console.WriteLine($"Default background colour:\t $2C\t:\t{this.StoryHeader.DefaultBackground}\t:\t{this.StoryHeader.DefaultBackground:X4}");
                Console.WriteLine($"Default foreground colour:\t $2D\t:\t{this.StoryHeader.DefaultForeground}\t:\t{this.StoryHeader.DefaultForeground:X4}");
                Console.WriteLine($"Terminating chars table:\t $2E\t:\t{this.StoryHeader.TerminatingCharsTable}\t:\t{this.StoryHeader.TerminatingCharsTable:X4}");
            }

            if (version > 5)
                Console.WriteLine($"Pixels sent to Stream(Width):\t $34\t:\t{this.StoryHeader.Stream3PixelsWidth}\t:\t{this.StoryHeader.Stream3PixelsWidth:X4}");

            if (version > 4)
            {
                Console.WriteLine($"Alphabet table:\t\t\t\t $34\t:\t{this.StoryHeader.AlphabetTable}\t:\t{this.StoryHeader.AlphabetTable:X4}");
                Console.WriteLine($"Header extension table:\t\t $36\t:\t{this.StoryHeader.HeaderExtensionTable}\t:\t{this.StoryHeader.HeaderExtensionTable:X4}");
            }

            if (version == 2)
                Console.WriteLine($"Serial Code:\t\t\t\t $12\t:\t{this.StoryHeader.SerialCode}");
            else if (version > 2)
                Console.WriteLine($"Date Compiled:\t\t\t\t $12\t:\t{this.StoryHeader.SerialCode}");

            if (version > 5)
            {
                Console.WriteLine($"Username:\t\t\t\t $12\t:\t{this.StoryHeader.Username}");
                Console.WriteLine($"Invform version:\t\t\t $12\t:\t{this.StoryHeader.InformVersion}");
            }
        }

        public string DecodeText(byte[] text)
        {
            var startAddress = 0;
            var chars = TextProcessor.GetZChars(text, ref startAddress);
            return this.TextDecoder.DecodeZChars(chars);
        }

        public string DecodeZChars(byte[] zchars)
        {
            return this.TextDecoder.DecodeZChars(zchars);
        }

        public byte[] EncodeText(string text)
        {
            var bytes = this.TextDecoder.EncodeUtf8ZChars(text);
            return bytes;
        }

        public byte[] EncodeWords(byte[] zchars)
        {
            var bytes = this.TextDecoder.EncodeZcharsToWords(zchars);
            return bytes;
        }

        public ZmObject GetObject(ushort objectIdx)=>this.ObjectTable.GetObject(objectIdx);
    }
}