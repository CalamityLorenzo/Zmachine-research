using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zmachine.V3.Instructions;

namespace Zmachine.V3.Machines
{

    [Flags]
    /// Instructions, methods, properties etc
    /// can be applied in varous differing ways.
    /// Good news! We know what/where they this is to discriminate.
    /// Note: there is an extension method to convert this to a actual version number too.
    internal enum MachineVersion
    {
        None = 0,
        One = 1,
        Two = 2,
        Three = 4,
        Four = 8,
        Five = 16,
        Six = 32,
        Seven = 64,
        Eight = 128,
        UpToFour = One | Two | Three | Four,
        FiveAndUp = Five | Six | Seven | Eight,
        SixAndUp = Six | Seven | Eight,
        All = One | Two | Three | Four | Five | Six | Seven | Eight
    }


    public partial class Machine
    {

        int ProgramCounter = 1;
        // the data
        public Stream GameData { get; }
        public StoryHeader StoryHeader { get; set; }
        internal MachineVersion FeaturesVersion { get; }

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
            this.FeaturesVersion = MachineExtensions.GetFeatureVersion(StoryHeader.Version);
            // move the program counter to the first instruction.
            ProgramCounter = SetProgramCounterInitialValue(StoryHeader.Version, StoryHeader.ProgramCounterInitalValue);
            // Load the correct set of instructions 
            this.Instructions = MachineExtensions.GetVersionInstructions(FeaturesVersion);
        }

        private static int SetProgramCounterInitialValue(int version, int programCounterInitalValue, int routineOffSet = 0)
        {
            // V1 Program counter initial value is a byte
            // v6 Packed address over two bytes
            if (version < 6)
                return programCounterInitalValue;
            else
            {
                return version switch
                {
                    // 4 or 5 => programCounterInitalValue * 4,
                    6 or 7 => programCounterInitalValue * 4 + routineOffSet,
                    8 => programCounterInitalValue * 8,
                    _ => throw new ArgumentOutOfRangeException("Version is out of range")
                };
            }
        }
    }
}
