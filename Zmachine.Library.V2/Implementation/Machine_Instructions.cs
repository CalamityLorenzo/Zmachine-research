using System;
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

            short result = (short)(lValue + (short)rValue);
            SetVariableValue(instruct.store, (ushort)result);
        }
        internal void And(DecodedInstruction instruct)
        {
            var a = GetVariableValue(instruct.operands[0]);
            var b = GetVariableValue(instruct.operands[1]);

            SetVariableValue(instruct.store, (ushort)(a & b));
        }

        /// <summary>
        /// Call Call_vs + v3
        /// </summary>
        /// <param name="instruct"></param>
        internal void Call(DecodedInstruction instruct)
        {


            var address = instruct.operands[0].operandType == OperandType.Variable
                ? GetVariableValue(instruct.operands[0])
                : (instruct.operands[0].value[0] << 8 | instruct.operands[0].value[1]);

            address = address.GetPackedAddress(this.StoryHeader.Version, this.StoryHeader.RoutinesOffset, this.StoryHeader.StaticStringsOffset);
            if (address == 0)
            {
                var dest = GameData[ProgramCounter++];
                SetVariableValue((ushort)dest, 0);
                //LibraryUtilities.StoreResult(GameData, CallStack, instruct, StoryHeader.GlobalVariables, 0);
                ProgramCounter--;
                return;
            }
            else
            {
                ushort returnAddress = (ushort)ProgramCounter;// + instruct.store;
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

                // Dirty hack..We actually placed the program counter in exactly the correct place.
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
            var returnAddress = (ushort)this.ProgramCounter;
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

        internal void ClearAttr(DecodedInstruction instruct)
        {
            var obj = GetVariableValue(instruct.operands[0]);
            var attr = GetVariableValue(instruct.operands[1]);

            this.ObjectTable.ClearAttribute(obj, attr);
        }

        internal void Dec(DecodedInstruction instruct)
        {
            var value = GetVariableValue(instruct.operands[0]);
            short result = (short)(value - 1);
            SetVariableValue(instruct.operands[0].value.GetUShort(), (ushort)result);

        }
        internal void DecChk(DecodedInstruction instruct)
        {
            var operand = instruct.operands[0] with
            {
                operandType = OperandType.Variable
            };

            var value = GetVariableValue(operand);
            var resultCheck = GetVariableValue(instruct.operands[1]);
            short result = (short)(value - 1);
            SetVariableValue(operand.value.GetUShort(), (ushort)result);
            if ((result < resultCheck) == instruct.branch.BranchIfTrue)
                Branch(instruct.branch.Offset);

        }
        internal void Div(DecodedInstruction instruct)
        {
            // Depending on the operand types depends if we have a value or pointer to a variable
            var lValue = GetVariableValue(instruct.operands[0]);
            var rValue = GetVariableValue(instruct.operands[1]);
            if (lValue == 0 || rValue == 0) throw new DivideByZeroException("Division by 0 error");
            short result = (short)(lValue / (short)rValue);
            LibraryUtilities.StoreResult(GameData, CallStack, instruct, StoryHeader.GlobalVariables, (ushort)result);
        }
        internal void GetChild(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var zObject = this.ObjectTable[objectId];
            this.SetVariableValue(instruct.store, zObject.Child);
            if ((zObject.Child != 0) == instruct.branch.BranchIfTrue)
                Branch(instruct.branch.Offset);

        }
        internal void GetParent(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var zObject = this.ObjectTable[objectId];
            this.SetVariableValue(instruct.store, zObject.Parent);
        }
        internal void GetProp(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var property = GetVariableValue(instruct.operands[1]);

            var propertyData = this.ObjectTable.GetProperty(objectId, property);
            SetVariableValue(instruct.store, propertyData);
        }
        internal void GetPropAddress(DecodedInstruction instruct)
        {
            var obj = GetVariableValue(instruct.operands[0]);
            var prop = GetVariableValue(instruct.operands[1]);

            var propertyAddress = this.ObjectTable.PropertyAddress(obj, prop);
            //if (propertyAddress > 0)
            //{
            //    byte propInfo = GameData[propertyAddress + 1];

            //    if (this.StoryHeader.Version > 3 && (propInfo & 0x80) == 0x80)
            //        propertyAddress += 2;
            //    else
            //        propertyAddress += 1;
            //}
            SetVariableValue(instruct.store, propertyAddress);
        }

        internal void GetPropLen(DecodedInstruction instruct)
        {
            var propAddress = GetVariableValue(instruct.operands[0]);
            //var prop = GetVariableValue(instruct.operands[1]);

           var length =  this.ObjectTable.GetPropertyLength(propAddress);
            SetVariableValue(instruct.store, length);

        }
        internal void GetSibling(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var siblingId = this.ObjectTable.GetSibling(objectId);
            this.SetVariableValue(instruct.store, siblingId);
            if ((siblingId != 0) == instruct.branch.BranchIfTrue)
                Branch(instruct.branch.Offset);
        }
        internal void Inc(DecodedInstruction instruct)
        {
            var variableId = GetVariableValue(instruct.operands[0]);
            var variableResult = (variableId switch
            {
                00 => this.CallStack.Peek().LocalStack.Peek(),
                >= 1 and <= 15 => this.CallStack.Peek().Locals[variableId - 1],
                >= 15 and <= 255 => this.GlobalVariables[variableId-16]
            });

            variableResult++;
            SetVariableValue(variableId, variableResult);
        }
        internal void IncChk(DecodedInstruction instruct)
        {
            var variableId = GetVariableValue(instruct.operands[0]);
            var valueToCompaire = GetVariableValue(instruct.operands[1]);
            var variableResult = (variableId switch
            {
                00 => this.CallStack.Peek().LocalStack.Peek(),
                >= 1 and <= 15 => this.CallStack.Peek().Locals[variableId - 1],
                >= 15 and <= 255 => this.GlobalVariables[variableId - 16]
            });

            variableResult++;
            SetVariableValue(variableId, variableResult);

            if (((short)variableResult > (short)valueToCompaire) == instruct.branch.BranchIfTrue)
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
            var expressionIsTrue = false;
            for (var x = 1; x < instruct.operands.Length; ++x)
            {
                var comparison = GetVariableValue(instruct.operands[x]);
                if (comparitor == comparison)
                {
                    expressionIsTrue = true;
                    break;
                }
            }

            if (expressionIsTrue == instruct.branch.BranchIfTrue)
            {
                // this.ProgramCounter = ProgramCounter + (short)instruct.branch.Offset - 2;
                Branch(instruct.branch.Offset);
            }

        }
        internal void Jg(DecodedInstruction instruct)
        {
            // Compairions are signed
            var comparitor = (short)GetVariableValue(instruct.operands[0]);
            var comparison = (short)GetVariableValue(instruct.operands[1]);
            if (((short)comparitor > (short)comparison) == instruct.branch.BranchIfTrue)
                 Branch(instruct.branch.Offset);
                //this.ProgramCounter = ProgramCounter + (short)instruct.branch.Offset - 2;

        }
        internal void Jin(DecodedInstruction instruct)
        {
            var op1 = GetVariableValue(instruct.operands[0]);
            var op2 = GetVariableValue(instruct.operands[1]);

            var objPa = this.ObjectTable[op1];
            //var op2ParentId = this.ObjectTable.GetParent(op1);
            if((op2 == objPa.Parent) ==instruct.branch.BranchIfTrue)
                          Branch(instruct.branch.Offset);
            //this.ProgramCounter = ProgramCounter + (short)instruct.branch.Offset - 2;

        }
        internal void Jl(DecodedInstruction instruct)
        {
            // Compairions are signed
            var comparitor = GetVariableValue(instruct.operands[0]);
            var comparison = GetVariableValue(instruct.operands[1]);
            if (((short)comparitor < (short)comparison) == instruct.branch.BranchIfTrue)
                          Branch(instruct.branch.Offset);
                //this.ProgramCounter = ProgramCounter + (short)instruct.branch.Offset - 2;
        }
        internal void Jz(DecodedInstruction instruct)
        {
            var val = GetVariableValue(instruct.operands[0]);

            if ((val == 0) == instruct.branch.BranchIfTrue)
            {
                //  Branch(instruct.branch.Offset);
                     Branch(instruct.branch.Offset);

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

            SetVariableValue(instruct.store, data);
        }

        //2OP:15 F loadw array word-index → (result)
        //Stores array→_word-index_(i.e., the word at address array+2*word-index, which must lie in static or dynamic memory).
        internal void LoadW(DecodedInstruction instruct)
        {
            var array = GetVariableValue(instruct.operands[0]);
            var idx = GetVariableValue(instruct.operands[1]);

            var data = GameData.Get2ByteValue(array + 2 * idx);

            SetVariableValue(instruct.store, data);
        }

        internal void Mod(DecodedInstruction instruct)
        {
            // Depending on the operand types depends if we have a value or pointer to a variable
            var lValue = GetVariableValue(instruct.operands[0]);
            var rValue = GetVariableValue(instruct.operands[1]);
            if (lValue == 0 || rValue == 0) throw new DivideByZeroException("Mod division by 0 error");

            var result = (short)(lValue % (short)rValue);
            SetVariableValue(instruct.store, (ushort)result);
        }

        internal void Mul(DecodedInstruction instruct)
        {
            // Depending on the operand types depends if we have a value or pointer to a variable
            var lValue = GetVariableValue(instruct.operands[0]);
            var rValue = GetVariableValue(instruct.operands[1]);

            short result = (short)(lValue * (short)rValue);
            SetVariableValue(instruct.store, (ushort)result);
        }
        // TODO more stream experiments required for speed.
        internal void NewLine()
        {
            this.PrintToScreen(new string('\r', 1));
        }
        internal void Not(DecodedInstruction instruct)
        {
            var num = GetVariableValue(instruct.operands[0]);
            SetVariableValue(instruct.store, (ushort)~num);
        }
        internal void Or(DecodedInstruction instruct)
        {
            var a = GetVariableValue(instruct.operands[0]);
            var b = GetVariableValue(instruct.operands[1]);

            SetVariableValue(instruct.store, (ushort)(a | b));

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
            PrintToScreen(PrepareForScreen(literal));
        }
        internal void PrintPAddr(DecodedInstruction instruct)
        {
            int memoryLocation = ((int)GetVariableValue(instruct.operands[0])).GetPackedAddress(this.StoryHeader.Version, this.StoryHeader.RoutinesOffset, this.StoryHeader.StaticStringsOffset);
            var chars = TextProcessor.GetZChars(this.GameData, ref memoryLocation);
            var literal = this.TextDecoder.DecodeZChars(chars);
            PrintToScreen(PrepareForScreen(literal));
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

        internal void PrintObj(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            if (objectId != 0)
            {
                var chars = this.TextDecoder.GetZChars(this.ObjectTable[objectId].PropertyTable.shortNameBytes);
                var literal = this.TextDecoder.DecodeZChars(chars);
                this.PrintToScreen( PrepareForScreen(literal));
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

            //  var obj = this.ObjectTable[objectId];
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
            // create the status line
            this.PrintToScreen(this.StatusLineText());


        }
        internal void SetAttr(DecodedInstruction instruct)
        {
            var objectId = GetVariableValue(instruct.operands[0]);
            var attribute = GetVariableValue(instruct.operands[1]);

            this.ObjectTable.SetAttribute(objectId, attribute);
            var obj = this.ObjectTable[objectId];
        }
        internal void ShowStatus(DecodedInstruction instruct)
        {
            var statusText = this.StatusLineText();
            this.PrintToScreen(statusText);
        }

        internal void Store(DecodedInstruction instruct)
        {
            // set the var(l) to value
            var left = GetVariableValue(instruct.operands[0]);
            var right = GetVariableValue(instruct.operands[1]);
            // Store is NOT itself a store md.
            SetVariableValue(left, right);
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
            this.SetVariableValue(instruct.store, (ushort)result);
            //LibraryUtilities.StoreResult(GameData, CallStack, instruct, StoryHeader.GlobalVariables, (ushort)result);
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
