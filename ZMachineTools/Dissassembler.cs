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
            var setStartingPoint = LibraryUtilities.SetProgramCounterInitialValue(StoryHeader.Version, this.StoryHeader.ProgramCounterInitalValue, this.StoryHeader.RoutinesOffset);

            var callName = "";
            // Lets get to the starting point of thje app.
            // v6 has a main, earlier has a jump off point
            int mainRountineAddress = 0;

            while (mainRountineAddress == 0)
            {
                var instruction = InstructionDecoder.Decode(this.Memory, setStartingPoint);
                callName = instruction.instruction.Name;
                if (callName.StartsWith("call"))
                {
                    var operand = instruction.operands[0].operand;
                    // Routime start is a packed adddress and of course, version dependen.
                    mainRountineAddress = (operand[0] << 8 | operand[1]).GetPackedAddress(StoryHeader.Version, 0, 0);
                }

            }
            var startAddress = mainRountineAddress.ToString("X4");
            var arguments = Memory[mainRountineAddress];
            if (arguments < 15)
            {
                Console.WriteLine(startAddress);
                var callStack = new List<int>();
                while (true)
                {
                    try
                    {
                        var currentInstr = InstructionDecoder.Decode(this.Memory, mainRountineAddress += 1);
                        mainRountineAddress += currentInstr.endAddress - currentInstr.startAddress;
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
                                    callStack.Add((operands.operand[0] << 8 | operands.operand[1]).GetPackedAddress(this.StoryHeader.Version, 0, 0));
                                    break;
                                }
                            case "print":
                                {
                                    var bytes = TextDecoder.GetZChars(currentInstr.operands[0].operand);
                                    var stringData = $"{TextDecoder.DecodeZChars(bytes)}";
                                    Console.WriteLine(stringData);
                                    break;
                                }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        break;
                    }

                }
            }



            // This is the first method and entry point

        }
    }
}
