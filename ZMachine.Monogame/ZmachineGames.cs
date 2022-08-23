﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZMachine.Library.V1;
using ZMachine.Library.V1.Instructions;
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
        private Stack<ActivationRecord> CallStack = new Stack<ActivationRecord>();
        private DecodedInstruction currentInstr;
        private string readInputText;
        private byte[] terminatingChars; // Which characters can terminate a read command.
        private Dictionary<short, Color> ColorMapper = new Dictionary<short, Color>();

        public bool IsReadingInstruction { get; private set; }

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

            if (this.StoryHeader.Version < 5)
            {
                this.terminatingChars = new byte[] { (byte)'\r' };
            }
            else
            {
                if (this.StoryHeader.TerminatingCharsTable == 0)
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
            this.CallStack.Push(new(-1, 0, Array.Empty<ushort>(), new Stack<ushort>()));
        }

        private void MapColors()
        {

            // Current
            ColorMapper.Add(-2, Color.White);
            // Default
            ColorMapper.Add(-1, Color.White);

            ColorMapper.Add(0x0000, Color.Black);
            ColorMapper.Add(0x001D, Color.Red);
            ColorMapper.Add(0x0340, Color.Green);
            ColorMapper.Add(0x03BD, Color.Yellow);
            ColorMapper.Add(0x59A0, Color.Blue);
            ColorMapper.Add(0x7C1F, Color.Magenta);

            ColorMapper.Add(0x77A0, Color.Cyan);
            ColorMapper.Add(0x7FFF, Color.White);
            ColorMapper.Add(0x5AD6, Color.LightGray);
            ColorMapper.Add(0x4631, Color.DimGray);
            ColorMapper.Add(0x2D6B, Color.DarkGray);
            ColorMapper.Add(-4, Color.Transparent);


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

            if (ProgramCounter < Memory.Length - 1)
            {
                this.currentInstr = InstructionDecoder.Decode(Memory, ref ProgramCounter);
                switch (currentInstr.instruction.Name)
                {
                    case "print":
                        {
                            var chars = this.TextDecoder.GetZChars(currentInstr.operands[0].operand);
                            var literal = this.TextDecoder.DecodeZChars(chars);
                            using StreamWriter sw = new StreamWriter(this.outputScreen, System.Text.Encoding.UTF8, bufferSize: literal.Length, leaveOpen: true);
                            sw.Write(literal);
                            sw.Close();
                        }
                        break;
                    case "call_1n":
                        {
                            var address = (currentInstr.operands[0].operand[0] << 8 | currentInstr.operands[0].operand[1]).GetPackedAddress(StoryHeader.Version, 0, 0);
                            var returnAddress = this.ProgramCounter;
                            this.ProgramCounter = address;
                            // Create stackframe
                            this.CallStack.Push(new ActivationRecord(returnAddress, address,
                                                      new ushort[this.Memory[ProgramCounter]],
                                                      new Stack<ushort>()));
                        }
                        break;
                    case "new_line":
                        {
                            using StreamWriter sw = new StreamWriter(this.outputScreen, System.Text.Encoding.UTF8, bufferSize: 1, leaveOpen: true);
                            sw.Write((char)13);
                            sw.Close();
                            sw.Dispose();
                        }
                        break;
                    case "rtrue":
                        {
                            var record = this.CallStack.Pop();
                            this.ProgramCounter = record.returnAdrress;
                        }
                        break;
                    case "aread":
                        {
                            // Kill the current stream
                            this.input0.SetLength(0);
                            // reset the input
                            this.readInputText = "";
                            var record = currentInstr;
                            this.IsReadingInstruction = true;
                        }
                        break;
                    case "jump":
                        {
                            ushort offSet = currentInstr.operands[0].GetShort();
                            this.ProgramCounter = ProgramCounter + offSet - 2;
                        }
                        break;
                    case "mod":
                        {
                            var left = currentInstr.operands[0].GetShort();
                            var right = currentInstr.operands[1].GetShort();
                            if (left == 0 || right == 0) throw new DivideByZeroException("Mod division by 0 error");
                            var result = (ushort)(left % right);
                            LibraryUtilities.StoreResult(Memory, CallStack, currentInstr, StoryHeader.GlobalVariables, result);
                        }
                        break;
                    case "mul":
                        {
                            var left = currentInstr.operands[0].GetShort();
                            var right = currentInstr.operands[1].GetShort();
                            if (left == 0 || right == 0) throw new DivideByZeroException("Mod division by 0 error");
                            var result = (ushort)(left * right);
                            LibraryUtilities.StoreResult(Memory, CallStack, currentInstr, StoryHeader.GlobalVariables, result);
                        }
                        break;
                    case "add":
                        {
                            var left = currentInstr.operands[0].GetShort();
                            var right = currentInstr.operands[1].GetShort();
                            var result = (ushort)(left + right);
                            LibraryUtilities.StoreResult(Memory, CallStack, currentInstr, StoryHeader.GlobalVariables, result);
                        }
                        break;
                    case "div":
                        {
                            var left = currentInstr.operands[0].GetShort();
                            var right = currentInstr.operands[1].GetShort();
                            if (left == 0 || right == 0) throw new DivideByZeroException("Mod division by 0 error");
                            var result = (ushort)(left * right);
                            LibraryUtilities.StoreResult(Memory, CallStack, currentInstr, StoryHeader.GlobalVariables, result);
                        }
                        break;
                    case "store":
                        {
                            var left = currentInstr.operands[0].GetShort();
                            var right = currentInstr.operands[1].GetShort();
                            // Store is NOT itself a store md.
                            //LibraryUtilities.StoreResult(Memory, CallStack, currentInstr, 0, right);
                        }
                        break;
                }
                this.ProgramCounter += 1;
            }
        }

        // A read collects a command
        // and terminates when the return key is found.
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