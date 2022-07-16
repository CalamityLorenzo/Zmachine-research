using ZMachine.Library.V1;
using ZMachine.Library.V1.Instructions;
using ZMachine.Library.V1.Utilities;

namespace Zmachine.V4.Machines
{

    public partial class Machine
    {

        int ProgramCounter = 1;
        // the data
        public Stream GameData { get; }
        public StoryHeader StoryHeader { get; set; }
        internal FeaturesVersion FeaturesVersion { get; }

        internal Dictionary<string, MachineInstruction> Instructions;

        // the complete machine memory divided into 3
        // dynamic, static high. They each have various purposes.
        // Dynamic = $0-> (The value of $0e minus 1) this includes the header
        // Static = (Value of $0e)-> (Endind not defined but must finish before $0ffff / 655353
        // High = (value Of $0d)->Ending of file (static and high may overlap)
        // Each entry is byte. Depending on the version depends on the size of the array. eg v3 = 128K v5=256k v6+ = MOAR!!!!
        // header is (by tradition) the first 64 bytes $0100 0000 0x40
        public byte[] Memory;

        Stack<byte> Stack = new Stack<byte>();

        public Machine(Stream storyData)
        {
            // Here's an assumption...
            storyData.Position = 0;
            GameData = new MemoryStream();
            storyData.CopyTo(GameData);
            GameData.Position = 0;

            Memory = new byte[GameData.Length];
            // Nom, nom nom game data.
            GameData.Read(Memory, 0, Memory.Length);
            this.StoryHeader = StoryHeader.CreateHeader(Memory);
            this.FeaturesVersion = LibraryUtilities.GetFeatureVersion(StoryHeader.Version);
            // move the program counter to the first instruction.
            ProgramCounter = LibraryUtilities.SetProgramCounterInitialValue(StoryHeader.Version, StoryHeader.ProgramCounterInitalValue);
            // Load the correct set of instructions 
            this.Instructions = LibraryUtilities.GetVersionInstructions(FeaturesVersion);
        }

    }
}
