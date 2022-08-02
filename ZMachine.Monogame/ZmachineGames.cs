﻿using System;
using System.IO;
using ZMachine.Library.V1;
using ZMachine.Library.V1.Objects;
using ZMachine.Library.V1.Utilities;

namespace ZMachine.Monogame
{
    public class ZMachineGamee
    {
        private Stream input0;
        private Stream input1;
        private readonly Stream outputScreen;
        private readonly Stream outputTranscript;
        private MemoryStream StoryData;
        private byte[] Memory;
        private int ProgramCounter;
        private StoryHeader StoryHeader;
        private HeaderExtensionTable HeaderExtensions;
        private FeaturesVersion FeaturesVersion;
        private DictionaryTable DictionaryTable;
        private AbbreviationsTable AbbreviationTable;
        private TextProcessor TextDecoder;
        private ObjectTable ObjectTable;
        private InstructionDecoder InstructionDecoder;

        public ZMachineGamee(Stream input0, Stream input1, Stream outputScreen, Stream outputTranscript, Stream storyData)
        {
            this.input0 = input0;
            this.input1 = input1;
            this.outputScreen = outputScreen;
            this.outputTranscript = outputTranscript;


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
            this.TextDecoder = new TextProcessor(Memory, AbbreviationTable, StoryHeader.Version);
            this.ObjectTable = new ObjectTable(this.StoryHeader.ObjectTable, this.StoryHeader.Version, Memory);
            this.InstructionDecoder = new InstructionDecoder(LibraryUtilities.GetVersionInstructions(this.FeaturesVersion), StoryHeader.Version);
        }

        internal void LoadCustomMemory(byte[] bytes)
        {
            this.Memory = bytes;
            this.ProgramCounter = 0;
        }

        public void Update()
        {
            if (ProgramCounter < Memory.Length - 1)
            {
                var currentInstr = InstructionDecoder.Decode(Memory, ref ProgramCounter);

                switch (currentInstr.instruction.Name)
                {
                    case "print":
                        {
                            var chars = this.TextDecoder.GetZChars(currentInstr.operands[0].operand);
                            var literal = this.TextDecoder.DecodeZChars(chars);
                            using StreamWriter sw = new StreamWriter(this.outputScreen, System.Text.Encoding.UTF8, bufferSize: literal.Length, leaveOpen: true);
                            sw.Write(literal);

                        }
                        break;
                }
            }
        }
    }
}