using System.Diagnostics;
using Zmachine.Library.V2.Instructions;
using Zmachine.Library.V2.Objects;
using Zmachine.Library.V2.Utilities;

namespace Zmachine.Library.V2.Implementation
{
    public partial class Machine
    {
        /// <summary>
        /// 10.2 Input streams
        /// 10.1 Keyboard only in V1
        /// </summary>
        private Stream input0;
        private Stream input1;

        private readonly Stream outputScreen;
        private readonly Stream outputTranscript;
        internal byte[] GameData;
        internal int ProgramCounter;
        internal StoryHeader StoryHeader;
        internal HeaderExtensionTable? HeaderExtensions;
        public FeaturesVersion FeaturesVersion;
        internal DictionaryTable DictionaryTable;
        internal AbbreviationsTable AbbreviationTable;
        internal TextProcessor TextDecoder;
        internal ObjectTable ObjectTable;
        internal InstructionDecoder InstructionDecoder;
        internal Stack<ActivationRecord> CallStack = new Stack<ActivationRecord>();
        internal DecodedInstruction currentInstr;
        /// <summary>
        /// complete output from the read methods.
        /// </summary>
        internal string readInputText = "";
        /// <summary>
        /// 10.5 Terminating characters
        /// Which characters can terminate a read command.
        /// </summary>
        internal byte[] terminatingChars;
        internal Dictionary<short, byte[]> ColorMapper = new Dictionary<short, byte[]>();


        ///has aread or sread been set
        public bool IsReadingInstruction { get; private set; }

        public Machine(Stream input0, Stream input1, Stream outputScreen, Stream outputTranscript, byte[] storyData)
        {
            this.input0 = input0;
            this.input1 = input1;
            this.outputScreen = outputScreen;
            this.outputTranscript = outputTranscript;

            // Nom, nom nom game data.            
            GameData = new byte[storyData.Length];
            storyData.CopyTo(GameData, 0);
            this.StoryHeader = StoryHeader.CreateHeader(GameData);

            // ToDo this should be moved into it;s own method for the interim.
            SetTerminatingChars(this.StoryHeader.Version);
            // Oo
            if (this.StoryHeader.Version >= 5 && this.StoryHeader.HeaderExtensionTable != 0)
            {
                this.HeaderExtensions = HeaderExtensionTable.CreateHeaderExtenions(GameData, this.StoryHeader.HeaderExtensionTable);
            }
            this.FeaturesVersion = LibraryUtilities.GetFeatureVersion(StoryHeader.Version);
            this.DictionaryTable = new DictionaryTable(StoryHeader.Version, StoryHeader.DictionaryTable, GameData);
            this.AbbreviationTable = new AbbreviationsTable(StoryHeader.AbbreviationTable, GameData, StoryHeader.Version);
            this.TextDecoder = new TextProcessor(AbbreviationTable, StoryHeader.Version);
            this.ObjectTable = new ObjectTable(this.StoryHeader.ObjectTable, this.StoryHeader.Version, GameData);
            this.InstructionDecoder = new InstructionDecoder(LibraryUtilities.GetVersionInstructions(this.FeaturesVersion), StoryHeader.Version);
        }

        /// <summary>
        /// Atemporay method
        /// This is a simple way of ensuring terminatoing characters
        /// </summary>
        /// <param name="storyVersion"></param>
        private void SetTerminatingChars(int storyVersion)
        {
            if (storyVersion < 5)
            {
                this.terminatingChars = new byte[] { (byte)'\r' };
            }
            else
            {
                if (storyVersion == 0)
                {
                    this.terminatingChars = Enumerable.Empty<Byte>().ToArray();
                    this.terminatingChars = this.terminatingChars.Concat(new byte[] { (byte)'\r' }).ToArray();
                }
                else
                {
                    // add other chars then
                    this.terminatingChars = Enumerable.Empty<Byte>().ToArray();
                    this.terminatingChars = this.terminatingChars.Concat(new byte[] { (byte)'\r' }).ToArray();
                }
            }
        }

        public void Update()
        {
            if (IsReadingInstruction)
            {
                ProcessRead();
                return;
            }

            //  a nonsense!
            if (this.ProgramCounter == -1)
            {
                return;
            }

            if (ProgramCounter < GameData.Length - 1)
            {
                this.currentInstr = InstructionDecoder.Decode(GameData, ref ProgramCounter);
                switch (currentInstr.instruction.Name)
                {
                    case "print":Print(currentInstr);
                        break;
                    case "call_1n":Call_1n(currentInstr);
                        break;
                    case "new_line":NewLine();
                        break;
                    case "rtrue":RTrue();
                        break;
                    case "aread":ARead(currentInstr);
                        break;
                    case "jump":Jump(currentInstr);
                        break;
                    case "mod":Mod(currentInstr);
                        break;
                    case "mul":Mul(currentInstr);
                        break;
                    case "add":Add(currentInstr);
                        break;
                    case "div":Div(currentInstr);
                        break;
                    case "store":Store(currentInstr);
                        break;
                    default:
                        Debug.WriteLine(currentInstr.instruction.Name);
                        break;
                }
                this.ProgramCounter += 1;
            }
        }

        /// <summary>
        /// A read collects a command
        /// and terminates when the return/termination key is found.
        /// </summary>
        private void ProcessRead()
        {
            if (this.input0.Length > 0)
            {
                this.input0.Position = 0;
                var bytes = new byte[input0.Length];
                var span = bytes.AsSpan();
                input0.Read(span);
                var chars = System.Text.Encoding.UTF8
                                                .GetString(span)
                                                .ToLower(System.Globalization.CultureInfo.CurrentCulture)
                                                .GetValidZSCIIChars(this.StoryHeader.Version);
                for (var x = 0; x < chars.Length; x++)
                {
                    var charA = (char)chars[x];
                    if (charA == '\b')
                    { // backspace
                        if (readInputText.Length == 1)
                        {
                            readInputText = "";
                        }
                        else if (readInputText.Length > 1)
                        {
                            readInputText = readInputText.Remove(readInputText.Length - 1, 1);
                        }
                    }
                    else if (charA == 27) // escape
                    {
                        this.readInputText = "";
                    }
                    else // Append character.
                    {
                        this.readInputText = this.readInputText + charA;
                    }

                    if (this.terminatingChars.Any(s => s == charA)) // we have completed our task.
                    {
                        this.IsReadingInstruction = false;
                    }
                }
                // Convert what we have into zChars, and then back again for output.
                var rawZchars = TextDecoder.EncodeUtf8ZChars(readInputText);
                var outputString = TextDecoder.DecodeZChars(rawZchars);
                // we write that into the output stream.
                // Which is then drawn onto the screen.
                // Whatever collects the output stream, must have a concept of acurrent line.
                // Which it will accept blithely. This is that current line. Everytime we update. we are effectively sending an incomplete command.
                // every new change is a new line.
                // word-wrapping etc can all be handedl by the screen for the moment.
                // new-line is the terminator here. The screen don't care.
                if (outputString.Length > 0)
                {
                    using StreamWriter sw = new StreamWriter(this.outputScreen, System.Text.Encoding.UTF8, bufferSize: outputString.Length, leaveOpen: true);
                    sw.Write(outputString);
                    sw.Close();
                }
                this.input0.SetLength(0);
            }

        }
    }
}
