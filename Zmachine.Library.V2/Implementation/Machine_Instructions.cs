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

        /// <summary>
        /// Call Call_vs + v3
        /// </summary>
        /// <param name="instruct"></param>
        internal void Call(DecodedInstruction instruct)
        {
            var address = (instruct.operands[0].value[0] << 8 | instruct.operands[0].value[1])
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
                    var opVal = instruct.operands[x].GetUShort();
                    var variableValue = GetVariableValue(instruct.operands[x]);
                    routineArguments.Add(variableValue);
                }

                var localVarCounts = this.GameData[ProgramCounter++];
                var localVars = new ushort[localVarCounts];
                // this is version 1 to 4 That store the initial values for local vars in the header
                // of a routine
                // 6.4.4
                var version = StoryHeader.Version;
                for (var x = 0; x < localVarCounts; ++x)
                {
                    if (version <= 4)
                    {
                        var ushortSegment = this.GameData[ProgramCounter..(ProgramCounter + 2)];
                        localVars[x] = LibraryUtilities.GetUShort(ushortSegment);
                        ProgramCounter += 2;
                    }
                    else
                    {
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
            var address = (instruct.operands[0].value[0] << 8 | instruct.operands[0].value[1])
                                        .GetPackedAddress(StoryHeader.Version, 0, 0);
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
        internal void Div(DecodedInstruction instruct)
        {
            var left = instruct.operands[0].GetUShort();
            var right = instruct.operands[1].GetUShort();
            if (left == 0 || right == 0) throw new DivideByZeroException("Division by 0 error");
            // Depending on the operand types depends if we have a value or pointer to a variable
            var lValue = GetVariableValue(instruct.operands[0]);
            var rValue = GetVariableValue(instruct.operands[1]);
            ushort result = (ushort)(lValue / rValue);
            LibraryUtilities.StoreResult(GameData, CallStack, instruct, StoryHeader.GlobalVariables, result);
        }

        internal void GetParent(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var zObject = this.ObjectTable[objectId];
            this.StoreVariableValue(instruct.store, zObject.Parent);
        }

        internal void GetChild(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var zObject = this.ObjectTable[objectId];
            this.StoreVariableValue(instruct.store, zObject.Child);

        }
        internal void IncChk(DecodedInstruction instruct)
        {
            var variableId = GetVariableValue(instruct.operands[0]);
            var valueToCompaire = GetVariableValue(instruct.operands[1]);
            var variableResult = (variableId switch
            {
                00 => this.CallStack.Peek().LocalStack.Peek(),
                >= 1 and <= 15 => this.CallStack.Peek().Locals[variableId],
                >= 16 and <= 256 => this.GlobalVariables[variableId]
            });

            variableResult++;

            if (variableId == 0)
            {
                _ = this.CallStack.Peek().LocalStack.Pop();
            }
            else if (variableId >= 1 && variableId <= 15)
                this.CallStack.Peek().Locals[variableId] = variableResult;
            else if (variableId >= 16 && variableId <= 256)
                this.GlobalVariables[variableId] = variableResult;

            if (variableResult > valueToCompaire)
                Branch(instruct.branch.Offset);
            //            if()
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
            //var comparitor = instruct.operands[0].operandType;
            for (var x = 0; x < instruct.operands.Length - 1; ++x)
            {
                if (comparitor == instruct.operands[1].GetUShort())
                    this.Branch(instruct.branch.Offset);
            }
        }

        internal void Jg(DecodedInstruction instruct)
        {
            // Compairions are signed
            var comparitor = GetVariableValue(instruct.operands[0]);
            var comparison = GetVariableValue(instruct.operands[1]);
            if (comparitor > comparison)
                this.Branch(instruct.branch.Offset);

        }

        internal void Jl(DecodedInstruction instruct)
        {
            // Compairions are signed
            var comparitor = instruct.operands[0].GetShort();
            var comparison = instruct.operands[1].GetShort();
            if (comparitor < comparison)
                this.Branch(instruct.branch.Offset);
        }

        internal void Jz(DecodedInstruction instruct)
        {
            var val = GetVariableValue(instruct.operands[0]);

            if (val == 0)
            {
                this.Branch(instruct.branch.Offset);
            }
        }
        internal void Jump(DecodedInstruction instruct)
        {
            // address are unsigned
            ushort offSet = instruct.operands[0].GetUShort();
            this.ProgramCounter = ProgramCounter + offSet - 2;
        }


        //2OP:16 10 loadb array byte-index → (result)
        // Stores array→_byte-index_(i.e., the byte at address array+byte-index, which must lie in static or dynamic memory).

        internal void LoadB(DecodedInstruction instruct)
        {
            var array = instruct.operands[0].GetUShort();
            var idx = instruct.operands[1].GetUShort();

            var data = GameData[array + idx];

            LibraryUtilities.StoreResult(this.GameData, this.CallStack, instruct, this.StoryHeader.GlobalVariables, data);
        }

        //2OP:15 F loadw array word-index → (result)
        //Stores array→_word-index_(i.e., the word at address array+2*word-index, which must lie in static or dynamic memory).
        internal void LoadW(DecodedInstruction instruct)
        {
            var array = instruct.operands[0].GetUShort();
            var idx = instruct.operands[1].GetUShort();

            var data = GameData[array + 2 * idx];

            LibraryUtilities.StoreResult(this.GameData, this.CallStack, instruct, this.StoryHeader.GlobalVariables, data);
        }



        internal void Mod(DecodedInstruction instruct)
        {
            var left = GetVariableValue(instruct.operands[0]);
            var right = GetVariableValue(instruct.operands[1]);
            if (left == 0 || right == 0) throw new DivideByZeroException("Mod division by 0 error");
            // Depending on the operand types depends if we have a value or pointer to a variable
            var lValue = GetVariableValue(instruct.operands[0]);
            var rValue = GetVariableValue(instruct.operands[1]);

            ushort result = (ushort)(lValue % rValue);
            LibraryUtilities.StoreResult(GameData, CallStack, instruct, StoryHeader.GlobalVariables, result);
        }
        internal void Mul(DecodedInstruction instruct)
        {
            var left = instruct.operands[0].GetUShort();
            var right = instruct.operands[1].GetUShort();

            // Depending on the operand types depends if we have a value or pointer to a variable
            var lValue = GetVariableValue(instruct.operands[0]);
            var rValue = GetVariableValue(instruct.operands[1]);

            ushort result = (ushort)(lValue * rValue);
            LibraryUtilities.StoreResult(GameData, CallStack, instruct, StoryHeader.GlobalVariables, result);
        }

        // TODO more stream experiments required for speed.
        internal void NewLine()
        {
            using StreamWriter sw = new StreamWriter(this.outputScreen, System.Text.Encoding.UTF8, bufferSize: 1, leaveOpen: true);
            sw.Write((char)13);
            sw.Close();
            sw.Dispose();
        }

        internal void Or(DecodedInstruction instruct)
        {
            var a = GetVariableValue(instruct.operands[0]);
            var b = GetVariableValue(instruct.operands[1]);
            LibraryUtilities.StoreResult(GameData, this.CallStack, instruct, StoryHeader.GlobalVariables, (ushort)(a | b));
        }

        internal void Print(DecodedInstruction instruct)
        {
            var chars = this.TextDecoder.GetZChars(instruct.operands[0].value);
            var literal = this.TextDecoder.DecodeZChars(chars);
            using StreamWriter sw = new StreamWriter(this.outputScreen, System.Text.Encoding.UTF8, bufferSize: literal.Length, leaveOpen: true);
            sw.Write(literal);
            sw.Close();
        }

        internal void PrintAddr(DecodedInstruction instruct)
        {
            int memoryLocation = GetVariableValue(instruct.operands[0]);


            var chars = TextProcessor.GetZChars(this.GameData, ref memoryLocation);
            var literal = this.TextDecoder.DecodeZChars(chars);
            using StreamWriter sw = new StreamWriter(this.outputScreen, System.Text.Encoding.UTF8, bufferSize: literal.Length, leaveOpen: true);
            sw.Write(literal);
            sw.Close();
        }

        internal void PrintNum(DecodedInstruction instruct)
        {
            string number = GetVariableValue(instruct.operands[0]).ToString("0.##");


            using StreamWriter sw = new StreamWriter(this.outputScreen, System.Text.Encoding.UTF8, bufferSize: number.Length, leaveOpen: true);
            sw.Write(number);
            sw.Close();
        }


        internal void RTrue()
        {
            var record = this.CallStack.Pop();
            this.ProgramCounter = record.ReturnAddress;

            var callingRecord = this.CallStack.Peek();
            if (callingRecord.StoreResult)
                callingRecord = callingRecord with { StoreAddress = 1 };
        }

        internal void RFalse()
        {
            var record = this.CallStack.Pop();
            this.ProgramCounter = record.ReturnAddress;

            var callingRecord = this.CallStack.Peek();
            if (callingRecord.StoreResult)
            {
                ushort? t = callingRecord.StoreAddress;
                if (t.HasValue)
                    StoreVariableValue(t.Value, 0);
                else throw new ArgumentOutOfRangeException("Cannot find Storage return address for call.");
                //callingRecord = callingRecord with { StoreAddress = 0 };
            }
        }

        internal void ARead(DecodedInstruction instruct)
        {
            // Kill the current stream
            this.input0.SetLength(0);
            // reset the input
            this.readInputText = "";
            //var record = instruct;
            this.IsReadingInstruction = true;
        }

        internal void Ret(DecodedInstruction instruct)
        {
            var valueToReturn = GetVariableValue(instruct.operands[0]);
            var stackFrame = this.CallStack.Pop();
            if (stackFrame.StoreResult)
            {
                StoreVariableValue(stackFrame.StoreAddress ?? throw new ArgumentException("No Store Adress found in return call.")
                    , valueToReturn);
            }
            this.ProgramCounter = stackFrame.ReturnAddress;
        }

        internal void Store(DecodedInstruction instruct)
        {
            // set the var(l) to value
            var left = instruct.operands[0].GetUShort();
            var right = instruct.operands[1].GetUShort();
            // Store is NOT itself a store md.
            StoreVariableValue(left, right);
        }

        internal void StoreW(DecodedInstruction instruct)
        {
            var array = GetVariableValue(instruct.operands[0]);
            var idx = instruct.operands[1].GetUShort();
            var value = GetVariableValue(instruct.operands[2]);

            ushort address = (ushort)(array + (2 * idx));
            StoreVariableValue(address, value);
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
            if ((bitMap & flags) == flags)
                Branch(instruct.branch.Offset);
        }
        internal void Test_Attr(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var attribute = GetVariableValue(instruct.operands[1]);

            var obj = this.ObjectTable[objectId];
            if (obj.Attributes.Contains((byte)attribute))
                Branch(instruct.branch.Offset);
                //this.ProgramCounter = ((int)instruct.branch.Offset)
                //        .GetPackedAddress(this.StoryHeader.Version, this.StoryHeader.RoutinesOffset, this.StoryHeader.StaticStringsOffset);
        }
    }
}
