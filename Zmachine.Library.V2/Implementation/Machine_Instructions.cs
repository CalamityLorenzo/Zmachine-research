using System.Data;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using Zmachine.Library.V2.Instructions;
using Zmachine.Library.V2.Utilities;

namespace Zmachine.Library.V2.Implementation
{
    /// <summary>
    /// THe implementaiton of all the instructions we need to make this muvva fly!
    /// </summary>
    public partial class Machine
    {



        internal void Add(DecodedInstruction instruct)
        {
            // Depending on the operand types depends if we have a value or pointer to a variable
            var lValue = GetVariableValue(instruct.operands[0]);
            var rValue = GetVariableValue(instruct.operands[1]);

            ushort result = (ushort)(lValue + rValue);
            LibraryUtilities.StoreResult(GameData, CallStack, instruct, StoryHeader.GlobalVariables, result);
        }
        internal void And(DecodedInstruction instruct)
        {
            var a = GetVariableValue(instruct.operands[0]);
            var b = GetVariableValue(instruct.operands[1]);

            StoreVariableValue(instruct.store, (ushort)(a & b));
        }

        /// <summary>
        /// Call Call_vs + v3
        /// </summary>
        /// <param name="instruct"></param>
        internal void Call(DecodedInstruction instruct)
        {


            var address = instruct.operands[0].operandType == OperandType.Variable
                ? GetVariableValue(instruct.operands[0])
                : (instruct.operands[0].value[0] << 8 | instruct.operands[0].value[1])
                                        .GetPackedAddress(StoryHeader.Version, 0, 0);
            if (address == 0)
                LibraryUtilities.StoreResult(GameData, CallStack, instruct, StoryHeader.GlobalVariables, 0);
            else
            {
                var returnAddress = ProgramCounter;// + instruct.store;
                this.ProgramCounter = address;
                // this first operand is the routine address as above
                List<ushort> routineArguments = new List<ushort>();
                for (var x = 1; x < instruct.operands.Length; ++x)
                {
                    //var opVal = instruct.operands[x].GetUShort();
                    var variableValue = GetVariableValue(instruct.operands[x]);
                    routineArguments.Add(variableValue);
                }
                // Local variables counted and created
                var localVarCounts = this.GameData[ProgramCounter++];
                var localVars = new ushort[localVarCounts];

                var version = StoryHeader.Version;
                for (var x = 0; x < localVarCounts; ++x)
                {
                    // this is version 1 to 4 That store the initial values for local vars in the header
                    // of a routine
                    // 6.4.4
                    if (version <= 4)
                    {
                        var ushortSegment = this.GameData[ProgramCounter..(ProgramCounter + 2)];
                        localVars[x] = LibraryUtilities.GetUShort(ushortSegment);
                        ProgramCounter += 2;
                    }
                    else
                    {  // v5+ everything is0
                        localVars[x] = 0;
                    }
                }

                // Dirty hack..We actually place the program counter in exactly the correct place.
                // the start of the next command. However at the end of each turn/update we increment the program counter
                // Thus it is now 1 byte off. This fixes that for earlier machines.
                if (version <= 4) ProgramCounter -= 1;

                // Okay so now we write the arguments
                for (var x = 0; x < routineArguments.Count; ++x)
                {
                    if (x > localVarCounts) break;
                    localVars[x] = routineArguments[x];
                }
                this.CallStack.Push(new ActivationRecord(returnAddress, address,
                                          localVars,
                                          true, instruct.store));
            }



            // Create stackframe
        }
        internal void Call_1n(DecodedInstruction instruct)
        {
            var address = instruct.operands[0].operandType == OperandType.Variable
               ? GetVariableValue(instruct.operands[0])
               : (instruct.operands[0].value[0] << 8 | instruct.operands[0].value[1]);
            var returnAddress = this.ProgramCounter;
            this.ProgramCounter = address;
            // Create stackframe
            this.CallStack.Push(new ActivationRecord(returnAddress, address,
                                      new ushort[this.GameData[ProgramCounter]],
                                false, instruct.store));
        }
        internal void Call_Vn(DecodedInstruction instruct)
        {
            Call(instruct);
            var stackFrame = this.CallStack.Pop();
            stackFrame = stackFrame with { StoreResult = false };
            this.CallStack.Push(stackFrame);
        }
        internal void Call_Vs(DecodedInstruction instruct)
        {
            Call(instruct);

        }
        internal void Dec(DecodedInstruction instruct)
        {
            var value = GetVariableValue(instruct.operands[0]);

            short result = (short)(value - 1);

            StoreVariableValue(instruct.operands[0].value.GetUShort(), (ushort)result);

        }
        internal void Div(DecodedInstruction instruct)
        {
            // Depending on the operand types depends if we have a value or pointer to a variable
            var lValue = GetVariableValue(instruct.operands[0]);
            var rValue = GetVariableValue(instruct.operands[1]);
            if (lValue == 0 || rValue == 0) throw new DivideByZeroException("Division by 0 error");
            ushort result = (ushort)(lValue / rValue);
            LibraryUtilities.StoreResult(GameData, CallStack, instruct, StoryHeader.GlobalVariables, result);
        }
        internal void GetChild(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var zObject = this.ObjectTable[objectId];
            //this.StoreVariableValue(instruct.store, zObject.Child);
            if ((zObject.Child != 0) == instruct.branch.BranchIfTrue)
                Branch(instruct.branch.Offset);

        }
        internal void GetParent(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var zObject = this.ObjectTable[objectId];
            this.StoreVariableValue(instruct.store, zObject.Parent);
        }
        internal void GetProp(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var property = GetVariableValue(instruct.operands[1]);

            var propertyData = this.ObjectTable.GetProperty(objectId, property);
            StoreVariableValue(instruct.store, propertyData);
        }
        internal void GetSibling(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var siblingId = this.ObjectTable.GetSibling(objectId);
            if ((siblingId != 0) == instruct.branch.BranchIfTrue)
                Branch(instruct.branch.Offset);
        }
        internal void IncChk(DecodedInstruction instruct)
        {
            var variableId = GetVariableValue(instruct.operands[0]);
            var valueToCompaire = GetVariableValue(instruct.operands[1]);
            var variableResult = (variableId switch
            {
                00 => this.CallStack.Peek().LocalStack.Peek(),
                >= 1 and <= 15 => this.CallStack.Peek().Locals[variableId - 1],
                >= 15 and <= 255 => (ushort)(GameData[StoryHeader.GlobalVariables + ((variableId - 16) * 2)] << 8 | GameData[StoryHeader.GlobalVariables + ((variableId - 16) * 2) + 1])
            });

            variableResult++;

            if (variableId == 0)
            {
                _ = this.CallStack.Peek().LocalStack.Pop();
                this.CallStack.Peek().LocalStack.Push(variableResult);
            }
            else if (variableId >= 1 && variableId <= 15)
                this.CallStack.Peek().Locals[variableId - 1] = variableResult;
            else if (variableId >= 16 && variableId <= 256)
                this.GlobalVariables[variableId] = variableResult;

            if ((variableResult > valueToCompaire) == instruct.branch.BranchIfTrue)
                Branch(instruct.branch.Offset);
        }
        internal void InsertObj(DecodedInstruction instruct)
        {
            var O_objectId = GetVariableValue(instruct.operands[0]);
            var D_objectId = GetVariableValue(instruct.operands[1]);

            this.ObjectTable.Insert_Obj(O_objectId, D_objectId);

        }
        // Jump if a is equal to any of the subsequent operands
        internal void Je(DecodedInstruction instruct)
        {
            var comparitor = GetVariableValue(instruct.operands[0]);
            for (var x = 0; x < instruct.operands.Length - 1; ++x)
            {
                var comparison = GetVariableValue(instruct.operands[1]);
                if (comparitor == comparison)
                {
                    if (instruct.branch.BranchIfTrue)
                    {
                        this.Branch(instruct.branch.Offset);
                    }
                    break;
                }
            }
        }
        internal void Jg(DecodedInstruction instruct)
        {
            // Compairions are signed
            var comparitor = (short)GetVariableValue(instruct.operands[0]);
            var comparison = (short)GetVariableValue(instruct.operands[1]);
            if (((short)comparitor > (short)comparison) == instruct.branch.BranchIfTrue)
                this.Branch(instruct.branch.Offset);

        }
        internal void Jl(DecodedInstruction instruct)
        {
            // Compairions are signed
            var comparitor = GetVariableValue(instruct.operands[0]);
            var comparison = GetVariableValue(instruct.operands[1]);
            if (((short)comparitor < (short)comparison) == instruct.branch.BranchIfTrue)
                this.Branch(instruct.branch.Offset);
        }
        internal void Jz(DecodedInstruction instruct)
        {
            var val = GetVariableValue(instruct.operands[0]);

            if ((val == 0) == instruct.branch.BranchIfTrue)
            {
                this.Branch(instruct.branch.Offset);
            }
        }
        internal void Jump(DecodedInstruction instruct)
        {
            // address are unsigned
            ushort offSet = GetVariableValue(instruct.operands[0]);
            this.ProgramCounter = ProgramCounter + (short)offSet - 2;
        }

        //2OP:16 10 loadb array byte-index → (result)
        // Stores array→_byte-index_(i.e., the byte at address array+byte-index, which must lie in static or dynamic memory).
        internal void LoadB(DecodedInstruction instruct)
        {
            var array = GetVariableValue(instruct.operands[0]);
            var idx = GetVariableValue(instruct.operands[1]);

            var data = GameData[array + idx];

            StoreVariableValue(instruct.store, data);
        }

        //2OP:15 F loadw array word-index → (result)
        //Stores array→_word-index_(i.e., the word at address array+2*word-index, which must lie in static or dynamic memory).
        internal void LoadW(DecodedInstruction instruct)
        {
            var array = GetVariableValue(instruct.operands[0]);
            var idx = GetVariableValue(instruct.operands[1]);

            var data = GameData.Get2ByteValue(array + 2 * idx);

            StoreVariableValue(instruct.store, data);
        }

        internal void Mod(DecodedInstruction instruct)
        {
            // Depending on the operand types depends if we have a value or pointer to a variable
            var lValue = GetVariableValue(instruct.operands[0]);
            var rValue = GetVariableValue(instruct.operands[1]);
            if (lValue == 0 || rValue == 0) throw new DivideByZeroException("Mod division by 0 error");

            var result = (short)(lValue % rValue);
            StoreVariableValue(instruct.store, (ushort)result);
        }

        internal void Mul(DecodedInstruction instruct)
        {
            // Depending on the operand types depends if we have a value or pointer to a variable
            var lValue = GetVariableValue(instruct.operands[0]);
            var rValue = GetVariableValue(instruct.operands[1]);

            short result = (short)(lValue * rValue);
            StoreVariableValue(instruct.store, (ushort)result);
        }

        // TODO more stream experiments required for speed.
        internal void NewLine()
        {
            this.PrintToScreen(new string('\r', 1));
        }

        internal void Or(DecodedInstruction instruct)
        {
            var a = GetVariableValue(instruct.operands[0]);
            var b = GetVariableValue(instruct.operands[1]);

            StoreVariableValue(instruct.store, (ushort)(a | b));

        }

        internal void Print(DecodedInstruction instruct)
        {
            var chars = this.TextDecoder.GetZChars(instruct.operands[0].value);
            var literal = this.TextDecoder.DecodeZChars(chars);
            var screenChar = PrepareForScreen(literal);
            PrintToScreen(screenChar);
        }

        internal void PrintAddr(DecodedInstruction instruct)
        {
            int memoryLocation = GetVariableValue(instruct.operands[0]);
            var chars = TextProcessor.GetZChars(this.GameData, ref memoryLocation);
            var literal = this.TextDecoder.DecodeZChars(chars);
            PrintToScreen(literal);
        }

        internal void PrintNum(DecodedInstruction instruct)
        {
            string number = GetVariableValue(instruct.operands[0]).ToString(); //.ToString("0.##");
            PrintToScreen(number);
        }

        internal void PrintRet(DecodedInstruction instruct)
        {
            var slice = this.GameData.AsSpan().Slice(start: instruct.startAddress + 1);
            var achars = this.TextDecoder.GetZChars(slice.ToArray());
            var literal = this.TextDecoder.DecodeZChars(achars);
            var screenReady = PrepareForScreen(literal);
            PrintToScreen(screenReady + '\r');
            SimpleReturn(CallStack.Pop(), 1);
        }
        /// <summary>
        /// Ensures a sttring can actually fit on the fookin screen
        /// </summary>
        /// <param name="literal"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private string PrepareForScreen(string literal)
        {
            if (literal.Length > this.screenWidthInChars)
            {
                var splitLiteral = new StringBuilder();
                // Is the currently string over the limit to fit on the screen.
                // This is the rows loop. 1 loop per row.
                bool overTheLimit = true;
                // The position in the string that starts this row
                string AllData = literal;
                // You can end up with a dangling part of text dependiong on how the text split.
                int leftOverChars = 0;
                var RowStart = 0;
                while (overTheLimit)
                {
                    var CurrentRowLength = this.screenWidthInChars - leftOverChars;
                    var GetCurrentRow = true;
                    var rowLiteral = "";
                    while (GetCurrentRow)
                    {
                        // COunting from the limit (screenWidthInChars) downwards find the first space character
                        if (AllData[RowStart + CurrentRowLength] != ' ')
                        {
                            CurrentRowLength -= 1;
                        }
                        else
                        {
                            // We have found a complete row.
                            GetCurrentRow = false;
                            rowLiteral = AllData.Substring(RowStart, CurrentRowLength);
                        }
                    }

                    // We now have a full line, but it may contain newline chars so.
                    // Now it just needs a little cleaning up.
                    if (!rowLiteral.Contains('\r'))
                    {
                        splitLiteral.Append(rowLiteral + '\r');
                        // Set the next row start position.
                        RowStart = RowStart + CurrentRowLength;
                    }
                    else // if we have new lines, we need to isolate those segments and append them
                    {
                        var startScan = 0;
                        for (var x = 0; x < rowLiteral.Length; ++x)
                        {
                            if (rowLiteral[x] == '\r')
                            {
                                splitLiteral.Append(rowLiteral.Substring(startScan, startScan==0? x : x-startScan) + '\r');
                                startScan = x + 1;
                            }

                            if (x == rowLiteral.Length - 1 && rowLiteral[x]!='\r')
                            {
                                splitLiteral.Append(rowLiteral.Substring(startScan));
                                leftOverChars = rowLiteral.Length - startScan;
                            }
                            else
                            {
                                leftOverChars = 0;
                            }
                        }

                        RowStart = RowStart + CurrentRowLength;
                    }
                }


                return splitLiteral.ToString();

            }
            else
            {
                return literal;
            }
        }

        internal void PrintObj(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            if (objectId != 0)
            {
                var chars = this.TextDecoder.GetZChars(this.ObjectTable[objectId].PropertyTable.shortNameBytes);
                var literal = this.TextDecoder.DecodeZChars(chars);
                this.PrintToScreen(literal);
            }
        }

        internal void PrintChar(DecodedInstruction instruct)
        {
            var outputChar = GetVariableValue(instruct.operands[0]);
            if (outputChar > 1023) throw new Exception("NOOOOO! TOO HIGH!");
            if (outputChar >= 32 && outputChar <= 255)
                this.PrintToScreen(new String(new char[] { (char)outputChar }));
            else
                this.PrintToScreen(outputChar.ToString());
        }
        internal void Push(DecodedInstruction instruct)
        {
            var value = GetVariableValue(instruct.operands[0]);
            CallStack.Peek().LocalStack.Push(value);
        }
        internal void PutProp(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var property = GetVariableValue(instruct.operands[1]);
            var value = GetVariableValue(instruct.operands[2]);
            this.ObjectTable.SetProperty(objectId, property, value);

            var obj = this.ObjectTable[objectId];
            //           obj.PropertyTable.properties.Contains(p=>p)
        }

        internal void Ret(DecodedInstruction instruct)
        {
            var valueToReturn = GetVariableValue(instruct.operands[0]);
            var stackFrame = this.CallStack.Pop();
            SimpleReturn(stackFrame, valueToReturn);

        }

        internal void RetPopped()
        {
            var pk = this.CallStack.Pop();
            SimpleReturn(pk, pk.LocalStack.Pop());
        }

        internal void RTrue() => SimpleReturn(this.CallStack.Pop(), 1);

        internal void RFalse() => SimpleReturn(this.CallStack.Pop(), 0);

        internal void ARead(DecodedInstruction instruct)
        {
            // Kill the current stream
            this.input0.SetLength(0);
            // reset the input
            this.readInputText = "";
            //var record = instruct;
            this.IsReadingInstruction = true;
        }

        internal void SRead(DecodedInstruction instruct)
        {
            // Kill the current stream
            this.input0.SetLength(0);
            // reset the input
            this.readInputText = "";
            //var record = instruct;
            this.IsReadingInstruction = true;
        }

        internal void SetAttr(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var attribute = GetVariableValue(instruct.operands[1]);

            this.ObjectTable.Set_Attribute(objectId, attribute);
            var obj = this.ObjectTable[objectId];
        }
        internal void Store(DecodedInstruction instruct)
        {
            // set the var(l) to value
            var left = GetVariableValue(instruct.operands[0]);
            var right = GetVariableValue(instruct.operands[1]);
            // Store is NOT itself a store md.
            StoreVariableValue(left, right);
        }
        internal void StoreB(DecodedInstruction instruct)
        {
            var array = GetVariableValue(instruct.operands[0]);
            var idx = GetVariableValue(instruct.operands[1]);
            var value = GetVariableValue(instruct.operands[2]);

            ushort address = (ushort)(array + idx);
            this.GameData[address] = (byte)value;
        }
        internal void StoreW(DecodedInstruction instruct)
        {
            var array = GetVariableValue(instruct.operands[0]);
            var idx = GetVariableValue(instruct.operands[1]);
            var value = GetVariableValue(instruct.operands[2]);

            ushort address = (ushort)(array + (2 * idx));
            GameData[address] = (byte)(value >> 8);
            GameData[address + 1] = (byte)(value & 0b11111111);
        }
        internal void Sub(DecodedInstruction instruct)
        {
            var left = GetVariableValue(instruct.operands[0]);
            var right = GetVariableValue(instruct.operands[1]);

            // Subtraction is signed...
            // Infact all of the arithimatic is signed.
            short result = (short)(left - right);
            LibraryUtilities.StoreResult(GameData, CallStack, instruct, StoryHeader.GlobalVariables, (ushort)result);
        }
        internal void Test(DecodedInstruction instruct)
        {
            var bitMap = GetVariableValue(instruct.operands[0]);
            var flags = GetVariableValue(instruct.operands[1]);
            if (((bitMap & flags) == flags) == instruct.branch.BranchIfTrue)
                Branch(instruct.branch.Offset);
        }
        internal void Test_Attr(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var attribute = GetVariableValue(instruct.operands[1]);

            var obj = this.ObjectTable[objectId];
            if ((obj.Attributes.Contains((byte)attribute)) == instruct.branch.BranchIfTrue)
                Branch(instruct.branch.Offset);
        }
    }
}
