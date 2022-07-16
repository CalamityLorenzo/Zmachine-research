using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2.Machines
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
        public MachineHeader HeaderDetails { get; set; }

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
            GameData.Read(Memory, 0, Memory.Length);

            // This is for convienice.
            // cos I cannot be bothered to learn all the hex for the position of the header.
            InitialiseHeader();
            // move the program counter to the first instruction.
            ProgramCounter = SetProgramCounterInitialValue(HeaderDetails.Version, HeaderDetails.ProgramCounterInitalValue);
            }

        public void Disassemble()
        {
            var abbreviations = new ZmAbbreviations(HeaderDetails.AbbreviationsTable, this.Memory, this.HeaderDetails.Version);
            var instructions = new Instructions(HeaderDetails.Version);
            var sx = new ZmInstructionDecoder(instructions, abbreviations, HeaderDetails.Version);
            sx.Decode(this.Memory, 0xD338+1, this.HeaderDetails.Version);
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

        private void InitialiseHeader()
        {

            var version = Memory[0];
            this.HeaderDetails = new()
            {
                Version = version,
                ProgramCounterInitalValue = Memory.Get2ByteValue(6),
                Flags = Memory[1], // need to break this out.
                Flags2 = Memory[16],
                ReleaseNumber = $"{Memory[2]}:{Memory[3]}",

                HighMemStart = Memory.Get2ByteValue(4),
                StaticStart = Memory.Get2ByteValue(0xE),

                Dictionary = Memory.Get2ByteValue(8),
                ObjectTable = Memory.Get2ByteValue(10),
                GlobalObjects = Memory.Get2ByteValue(12),
                AbbreviationsTable = Memory.Get2ByteValue(24),

                LengthOfFile = Memory.Get2ByteValue(26),
                InterpreterNumber = Memory[30],
                InterpreterVersion = Memory[31],

                RoutinesOffset = version > 5 ? Memory.Get2ByteValue(40) : ushort.MinValue,
                StaticStringsOffSet = version > 5 ? Memory.Get2ByteValue(42) : ushort.MinValue,

                DateCompiled = new string(new char[]
                {
                    (char)Memory[0x12],
                    (char)Memory[0x13],
                    (char)Memory[0x14],
                    (char)Memory[0x15],
                    (char)Memory[0x16],
                    (char)Memory[0x17]
                })
            };
        }
    }
}
