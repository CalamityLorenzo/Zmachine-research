using Zmachine.Library.V2.Instructions;
using Zmachine.Library.V2.Objects;
using Zmachine.Library.V2.Utilities;

namespace Zmachine.Library.V2
{
    public  class Machine
    {
        /// <summary>
        /// 10.2 Input streams
        /// 10.1 Keyboard only in V1
        /// </summary>
        private Stream input0;
        private Stream input1;

        private readonly Stream outputScreen;
        private readonly Stream outputTranscript;
        internal byte[] MachineMemory;
        internal int ProgramCounter;
        internal StoryHeader StoryHeader;
        internal HeaderExtensionTable HeaderExtensions;
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
        internal string readInputText="";
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
            MachineMemory = new byte[storyData.Length];
            storyData.CopyTo(MachineMemory, 0);
            this.StoryHeader = StoryHeader.CreateHeader(MachineMemory);

            // ToDo this should be moved into it;s own method for the interim.
            SetTerminatingChars(this.StoryHeader.Version);
            // Oo
            if (this.StoryHeader.Version>=5 && this.StoryHeader.HeaderExtensionTable != 0)
            {
                this.HeaderExtensions = HeaderExtensionTable.CreateHeaderExtenions(MachineMemory, this.StoryHeader.HeaderExtensionTable);
            }
            this.FeaturesVersion = LibraryUtilities.GetFeatureVersion(StoryHeader.Version);
            this.DictionaryTable = new DictionaryTable(StoryHeader.Version, StoryHeader.DictionaryTable, MachineMemory);
            this.AbbreviationTable = new AbbreviationsTable(StoryHeader.AbbreviationTable, MachineMemory, StoryHeader.Version);
            this.TextDecoder = new TextProcessor(MachineMemory, AbbreviationTable, StoryHeader.Version);
            this.ObjectTable = new ObjectTable(this.StoryHeader.ObjectTable, this.StoryHeader.Version, MachineMemory);
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
    }
}
