using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2.Machines
{
    public partial class Zmachine_v2
    {

        int ProgramCounter = 1;
        // the data
        public Stream GameData { get; }
        public ZmHeaderDetails HeaderDetails { get; set; }

        // the complete machine memory divided into 3
        // dynamic, static high. They each have various purposes.
        // Dynamic = $0-> (The value of $0e minus 1) this includes the header
        // Static = (Value of $0e)-> (Endind not defined but must finish before $0ffff / 655353
        // High = (value Of $0d)->Ending of file (static and high may overlap)
        // Each entry is byte. Depending on the version depends on the size of the array. eg v3 = 128K v5=256k v6+ = MOAR!!!!
        // header is (by tradition) the first 64 bytes $0100 0000 0x40
        public byte[] Memory;

        Stack<byte> Stack = new Stack<byte>();

        public Zmachine_v2(Stream storyData)
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
            InitDetailsObject();
            // move the program counter to the first instruction.
            ProgramCounter = SetProgramCounterInitialValue(HeaderDetails.Version, HeaderDetails.ProgramCounterInitalValue);
            }

        private static int SetProgramCounterInitialValue(int version, int programCounterInitalValue, int routineOffSet = 0)
        {
            // V1 Program counter initial value is a byte
            // v6 Packed address over two bytes
            if (version < 6)
                return programCounterInitalValue >> 8;
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

        private void InitDetailsObject()
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
