using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMachine.Library.V1;
using ZMachine.Library.V1.Objects;
using ZMachine.Library.V1.Utilities;

namespace ZMachineTools
{
    public class Dissassembler
    {
        private MemoryStream StoryData;
        private Byte[] Memory;
        private StoryHeader StoryHeader;
        private HeaderExtensionTable HeaderExtensions;
        private FeaturesVersion FeaturesVersion;
        private DictionaryTable DictionaryTable;
        private AbbreviationsTable AbbreviationTable;
        private TextDecoder TextDecoder;
        private ObjectTable ObjectTable;

        public InstructionDecoder InstructionDecoder { get; }

        public Dissassembler(Stream storyData)
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
            this.DictionaryTable = new DictionaryTable(StoryHeader.DictionaryTable, Memory);
            this.AbbreviationTable = new AbbreviationsTable(StoryHeader.AbbreviationTable, Memory, StoryHeader.Version);
            this.TextDecoder = new TextDecoder(Memory, AbbreviationTable, StoryHeader.Version);
            this.ObjectTable = new ObjectTable(this.StoryHeader.ObjectTable, this.StoryHeader.Version, Memory);

            this.InstructionDecoder = new InstructionDecoder(LibraryUtilities.GetVersionInstructions(this.FeaturesVersion), StoryHeader.Version);
        }

        public void Disassemble()
        {
            // Start at the entry point
            var setStartingPoint = LibraryUtilities.SetProgramCounterInitialValue(StoryHeader.Version, this.StoryHeader.ProgramCounterInitalValue, this.StoryHeader.RoutinesOffset);

            var callName = "";
            // Lets get to the starting point of thje app.
            // v6 has a main, earlier has a jump off point
            int mainRoutineAddress = 0;
            while (mainRoutineAddress == 0)
            {
                var instruction = InstructionDecoder.Decode(this.Memory, setStartingPoint);
                callName = instruction.instruction.Name;
                if (callName.StartsWith("call"))
                {
                    var operand = instruction.operands[0].operand;
                    // Routime start is a packed adddress and of course, version dependen.
                    mainRoutineAddress = (operand[0] << 8 | operand[1]).GetPackedAddress(StoryHeader.Version, 0, 0);
                }

            }

            Dictionary<int, RoutineInfo> Routines = new Dictionary<int, RoutineInfo> { { mainRoutineAddress, new RoutineInfo(mainRoutineAddress, 0,0, false) } };

            ProcessRoutine(mainRoutineAddress, Routines);
            // This is the first method and entry point

        }

        private void ProcessRoutine(int routineAddress,  Dictionary<int, RoutineInfo> Routines)
        {
            var startAddress = routineAddress;
            var arguments = Memory[routineAddress];
            var lengthR = $"arguments = {arguments}".Length;
            Console.WriteLine("*".PadLeft(lengthR, '*'));
            var startAddressHex = routineAddress.ToString("X4");
            Console.WriteLine($"Address = {startAddressHex}");
            Console.WriteLine($"arguments = {arguments}");
            Console.WriteLine("*".PadLeft(lengthR, '*'));

            if (arguments < 15)
            {
                Console.WriteLine(startAddressHex);
                var callStack = new List<int>();
                while (true)
                {
                    try
                    {
                        var currentInstr = InstructionDecoder.Decode(this.Memory, routineAddress += 1);
                        routineAddress += currentInstr.endAddress - currentInstr.startAddress;
                        switch (currentInstr.instruction.Name)
                        {
                            case "call": // only present in v3
                            case "call_1s":
                            case "call_1n":
                            case "call_2n":
                            case "call_2s":
                                {
                                    // First operand is always the routine.
                                    var operands = currentInstr.operands[0];
                                    var packedAddress = (operands.operand[0] << 8 | operands.operand[1]).GetPackedAddress(this.StoryHeader.Version, 0, 0);
                                    if (!Routines.ContainsKey(packedAddress))
                                        Routines[packedAddress] = new RoutineInfo(packedAddress, 0,0, false);
                                    break;
                                }
                            case "print":
                                {
                                    var bytes = TextDecoder.GetZChars(currentInstr.operands[0].operand);
                                    var stringData = $"{TextDecoder.DecodeZChars(bytes)}";
                                //    Console.WriteLine($"\"{stringData.Replace("\r", "^")}\"");
                                    break;
                                }
                        }

                       // Console.WriteLine($"${currentInstr.startAddress:X} : {currentInstr.instruction.Name} {currentInstr.hexBytes}");

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        var oldRoutine = Routines[startAddress];
                        Routines[startAddress] = oldRoutine with { addressError = routineAddress, arguments= arguments, isParsed=true };

                        var nextRoutine = Routines.FirstOrDefault(a => a.Value.isParsed == false);
                        if (nextRoutine.Equals(default(KeyValuePair<int, RoutineInfo>))==false)
                        {
                            ProcessRoutine(nextRoutine.Value.addressStart, Routines);
                        }
                        break;
                    }

                }
            }


        }
    }
}
