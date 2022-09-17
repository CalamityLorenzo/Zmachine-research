﻿using System.Diagnostics;
using System.Text;
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
        internal Stream input0;
        internal Stream input1;

        internal readonly Stream outputScreen;
        internal readonly Stream outputTranscript;
        internal byte[] GameData;
        internal int ProgramCounter;

        private int screenWidthInChars;
        private int screenHeightInChars;


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
        internal GlobalVariables GlobalVariables;

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

        public Machine(Stream input0, Stream input1, Stream outputScreen, Stream outputTranscript, int screenWidthInChars, int screenHeightInChars, byte[] storyData)
        {
            this.input0 = input0;
            this.input1 = input1;
            this.outputScreen = outputScreen;
            this.outputTranscript = outputTranscript;
            this.screenHeightInChars = screenHeightInChars;
            this.screenWidthInChars = screenWidthInChars;
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
            this.GlobalVariables = new(this.GameData, this.StoryHeader.GlobalVariables);
            this.ProgramCounter = LibraryUtilities.SetProgramCounterInitialValue(this.StoryHeader.Version, this.StoryHeader.ProgramCounterInitalValue, this.StoryHeader.RoutinesOffset);
            if (this.StoryHeader.Version <= 37)
            {
                this.CallStack.Push(new ActivationRecord
                (
                    ReturnAddress: 0,
                    StartAdress: this.ProgramCounter,
                    Locals: new ushort[0],
                    false, null
                ));
            }
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

        private ushort GetVariableValue(Operand operand)
        => LibraryUtilities.GetOperandValue(GameData, StoryHeader.GlobalVariables, CallStack.Peek(), operand.operandType, operand.value.GetUShort());


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
                    case "add":
                        Add(currentInstr);
                        break;
                    case "and":
                        And(currentInstr);
                        break;
                    case "call":
                    case "call_vs":
                        Call(currentInstr);
                        break;
                    case "call_1n":
                        Call_1n(currentInstr);
                        break;
                    case "dec":
                        Dec(currentInstr);
                        break;
                    case "dec_chk":
                        DecChk(currentInstr);
                        break;
                    case "div":
                        Div(currentInstr);
                        break;
                    case "get_parent":
                        GetParent(currentInstr);
                        break;
                    case "get_child":
                        GetChild(currentInstr);
                        break;
                    case "get_prop":
                        GetProp(currentInstr);
                        break;
                    case "get_sibling":
                        GetSibling(currentInstr);
                        break;
                    case "inc":
                        Inc(currentInstr);
                        break;
                    case "inc_chk":
                        IncChk(currentInstr);
                        break;
                    case "je":
                        Je(currentInstr);
                        break;
                    case "jg":
                        Jg(currentInstr);
                        break;
                    case "jin":
                        Jin(currentInstr);
                        break;
                    case "jl":
                        Jl(currentInstr);
                        break;
                    case "jz":
                        Jz(currentInstr);
                        break;
                    case "insert_obj":
                        InsertObj(currentInstr);
                        break;
                    case "jump":
                        Jump(currentInstr);
                        break;
                    case "loadb":
                        LoadB(currentInstr);
                        break;
                    case "loadw":
                        LoadW(currentInstr);
                        break;
                    case "mod":
                        Mod(currentInstr);
                        break;
                    case "mul":
                        Mul(currentInstr);
                        break;
                    case "new_line":
                        NewLine();
                        break;
                    case "or":
                        Or(currentInstr);
                        break;
                    case "print":
                        Print(currentInstr);
                        break;
                    case "print_addr":
                        PrintAddr(currentInstr);
                        break;
                    case "print_paddr":
                        PrintPAddr(currentInstr);
                        break;
                    case "print_char":
                        PrintChar(currentInstr);
                        break;
                    case "print_num":
                        PrintNum(currentInstr);
                        break;
                    case "print_ret":
                        PrintRet(currentInstr);
                        break;
                    case "print_obj":
                        PrintObj(currentInstr);
                        break;
                    case "push":
                        Push(currentInstr);
                        break;
                    case "ret":
                        Ret(currentInstr);
                        break;
                    case "aread":
                        ARead(currentInstr);
                        break;
                    case "sread":
                        SRead(currentInstr);
                        break;
                    case "ret_popped":
                        RetPopped();
                        break;
                    case "rtrue":
                        RTrue();
                        break;
                    case "rfalse":
                        RFalse();
                        break;
                    case "set_attr":
                        SetAttr(currentInstr);
                        break;
                    case "show_status":
                        ShowStatus(currentInstr);
                        break;
                    case "store":
                        Store(currentInstr);
                        break;
                    case "storeb":
                        StoreB(currentInstr);
                        break;
                    case "storew":
                        StoreW(currentInstr);
                        break;
                    case "sub":
                        Sub(currentInstr);
                        break;
                    case "test":
                        Test(currentInstr);
                        break;
                    case "test_attr":
                        Test_Attr(currentInstr);
                        break;
                    default:
                        Debug.WriteLine(currentInstr.instruction.Name);
                        break;
                }
                this.ProgramCounter += 1;
            }
        }

        private void SimpleReturn(ActivationRecord callingRecord, ushort value)
        {
            this.ProgramCounter = callingRecord.ReturnAddress;
            if (callingRecord.StoreResult)
            {
                //ushort? t = callingRecord.StoreAddress;
                //if (t.HasValue)
                //    StoreVariableValue(t.Value, value);
                //else throw new ArgumentOutOfRangeException("Cannot find Storage return address for call.");
                ushort t = GameData[ProgramCounter];
                StoreVariableValue(t, value);
            }

        }

        private void StoreVariableValue(ushort variableId, ushort value)
        {
            switch (variableId)
            {

                case 0:             // Stack
                    CallStack.Peek().LocalStack.Push(value);
                    break;
                case > 0 and <= 15: // Local vars
                    {
                        var localVars = CallStack.Peek().Locals;
                        localVars[variableId - 1] = value;
                    }
                    break;
                case >= 16 and <= 255: // Global
                    // 6.2 Storage of global variables
                    // Convert the varible number into the memort off set from the global vars table.
                    var variablePosition = (variableId - 16) * 2;
                    var resultArray = value.ToByteArray();
                    var globalVariables = GameData[StoryHeader.GlobalVariables];
                    // they are words/
                    GameData[StoryHeader.GlobalVariables + variablePosition] = resultArray[0];
                    GameData[StoryHeader.GlobalVariables + variablePosition + 1] = resultArray[1];
                    break;
            }

        }

        private void Branch(short branchOffset)
        {
            if (branchOffset == 0 || branchOffset == 1)
            {
                var callingRecord = this.CallStack.Pop();
                this.ProgramCounter = callingRecord.ReturnAddress;
                if (callingRecord.StoreResult)
                {

                    //ushort? t = callingRecord.StoreAddress;
                    //if (t.HasValue)
                    //    StoreVariableValue(t.Value, value);
                    //else throw new ArgumentOutOfRangeException("Cannot find Storage return address for call.");
                    ushort t = GameData[ProgramCounter];
                    StoreVariableValue(t, (ushort)branchOffset);
                    //ushort? t = callingRecord.StoreAddress;
                    //if (t.HasValue)
                    //    StoreVariableValue(t.Value, (ushort)branchOffset);
                    //else throw new ArgumentOutOfRangeException("Cannot find Storage return address for call.");
                }
            }
            else
            {
                this.ProgramCounter = ProgramCounter + (short)branchOffset - 2;
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

        internal void PrintToScreen(string outputLiteral)
        {
            using StreamWriter sw = new StreamWriter(this.outputScreen, System.Text.Encoding.UTF8, bufferSize: outputLiteral.Length, leaveOpen: true);
            sw.Write(outputLiteral);
            sw.Close();
        }

        private string PrepareForScreen(string literal)
        {
            if (literal.Length > this.screenWidthInChars)
            {
                var splitLiteral = new StringBuilder();
                // Is the currently string over the limit to fit on the screen.
                // This is the rows loop. 1 loop per row.
                bool widerTheScreen = true;
                // The position in the string that starts this row
                string AllData = literal;
                // You can end up with a dangling part of text dependiong on how the text split.
                int rowLengthOffset = 0;
                var RowStart = 0;
                while (widerTheScreen)
                {
                    var CurrentRowLength = this.screenWidthInChars - rowLengthOffset;
                    var CreateRows = true;
                    var rowLiteral = "";
                    if (rowLengthOffset > 0) rowLengthOffset = 0;
                    while (CreateRows)
                    {
                        if (RowStart + CurrentRowLength > AllData.Length)
                        {
                            CreateRows = false;
                            CurrentRowLength = (AllData.Length) - RowStart;
                            rowLiteral = AllData.Substring(RowStart, CurrentRowLength);
                        }
                        else
                        {
                            // COunting from the limitz (screenWidthInChars) downwards find the first space character
                            if (AllData[RowStart + CurrentRowLength] != ' ')
                            {
                                CurrentRowLength -= 1;
                            }
                            else
                            {
                                // We have found a complete row.
                                CreateRows = false;
                                rowLiteral = AllData.Substring(RowStart, CurrentRowLength + 1); // grabs the space
                            }
                        }
                    }

                    if (!rowLiteral.Contains('\r'))
                    {
                        // last line in this sagas
                        if (RowStart + CurrentRowLength >= AllData.Length)
                        {
                            splitLiteral.Append(rowLiteral);
                            widerTheScreen = false;
                        }
                        else
                        {
                            splitLiteral.Append(rowLiteral + '\r');
                            RowStart += CurrentRowLength + 1; // Ensures the space char is on the previous line.
                        }
                    }
                    else
                    {
                        // okay the row has line breaks in it.
                        // quantize them into discrete units.
                        // The last entry will *not* end on a '\r' (The loops above split on a new line)
                        // So now we print this partial line, with no line break, this is now an offset
                        // That must be carried throught to the next run of text being processed.
                        // (three attempts two days for this fucking algo and I KNOW I've written it before)
                        // Who fucks up an accumulator?
                        var rows = rowLiteral.Split('\r');
                        for (var x = 0; x < rows.Length; ++x)
                        {
                            if (x == rows.Length - 1)
                            {
                                RowStart += CurrentRowLength + 1;
                                splitLiteral.Append(rows[x]);
                                rowLengthOffset = rows[x].Length;
                            }
                            else
                            {
                                splitLiteral.Append(rows[x] + '\r');
                            }
                        }
                    }
                    if (RowStart >= AllData.Length - 1)
                        widerTheScreen = false;
                }

                return splitLiteral.ToString();

            }
            else
            {
                return literal;
            }
        }


    }
}
