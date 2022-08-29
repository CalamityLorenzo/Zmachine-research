using Zmachine.Library.V2.Instructions;
using Zmachine.Library.V2.Utilities;

namespace Zmachine.Library.V2.Implementation
{
    /// <summary>
    /// THe implementaiton of all the instructions we need to make this muvva fly!
    /// </summary>
    public partial class Machine
    {
        internal void Print(DecodedInstruction instruct)
        {
            var chars = this.TextDecoder.GetZChars(instruct.operands[0].operand);
            var literal = this.TextDecoder.DecodeZChars(chars);
            using StreamWriter sw = new StreamWriter(this.outputScreen, System.Text.Encoding.UTF8, bufferSize: literal.Length, leaveOpen: true);
            sw.Write(literal);
            sw.Close();
        }
        internal void Call_1n(DecodedInstruction instruct)
        {
            var address = (instruct.operands[0].operand[0] << 8 | instruct.operands[0].operand[1])
                                        .GetPackedAddress(StoryHeader.Version, 0, 0);
            var returnAddress = this.ProgramCounter;
            this.ProgramCounter = address;
            // Create stackframe
            this.CallStack.Push(new ActivationRecord(returnAddress, address,
                                      new ushort[this.GameData[ProgramCounter]],
                                      new Stack<ushort>()));
        }
        // TODO more stream experiments required for speed.
        internal void NewLine()
        {
            using StreamWriter sw = new StreamWriter(this.outputScreen, System.Text.Encoding.UTF8, bufferSize: 1, leaveOpen: true);
            sw.Write((char)13);
            sw.Close();
            sw.Dispose();
        }

        internal void RTrue()
        {
            var record = this.CallStack.Pop();
            this.ProgramCounter = record.returnAdrress;
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
        internal void Jump(DecodedInstruction instruct)
        {
            ushort offSet = instruct.operands[0].GetShort();
            this.ProgramCounter = ProgramCounter + offSet - 2;
        }
        internal void Mod(DecodedInstruction instruct)
        {
            var left = instruct.operands[0].GetShort();
            var right = instruct.operands[1].GetShort();
            if (left == 0 || right == 0) throw new DivideByZeroException("Mod division by 0 error");
            // Depending on the operand types depends if we have a value or pointer to a variable
            var lValue = LibraryUtilities.GetOperandValue(this.CallStack.Peek(), instruct.operands[0].operandType, left);
            var rValue = LibraryUtilities.GetOperandValue(this.CallStack.Peek(), instruct.operands[1].operandType, right);

            ushort result = (ushort)(lValue % rValue);
            LibraryUtilities.StoreResult(GameData, CallStack, instruct, StoryHeader.GlobalVariables, result);
        }
        internal void Mul(DecodedInstruction instruct)
        {
            var left = instruct.operands[0].GetShort();
            var right = instruct.operands[1].GetShort();

            // Depending on the operand types depends if we have a value or pointer to a variable
            var lValue = LibraryUtilities.GetOperandValue(this.CallStack.Peek(), instruct.operands[0].operandType, left);
            var rValue = LibraryUtilities.GetOperandValue(this.CallStack.Peek(), instruct.operands[1].operandType, right);

            ushort result = (ushort)(lValue * rValue);
            LibraryUtilities.StoreResult(GameData, CallStack, instruct, StoryHeader.GlobalVariables, result);
        }
        internal void Add(DecodedInstruction instruct)
        {
            var left = instruct.operands[0].GetShort();
            var right = instruct.operands[1].GetShort();

            // Depending on the operand types depends if we have a value or pointer to a variable
            var lValue = LibraryUtilities.GetOperandValue(this.CallStack.Peek(), instruct.operands[0].operandType, left);
            var rValue = LibraryUtilities.GetOperandValue(this.CallStack.Peek(), instruct.operands[1].operandType, right);

            ushort result = (ushort)(lValue + rValue);
            LibraryUtilities.StoreResult(GameData, CallStack, instruct, StoryHeader.GlobalVariables, result);
        }

        internal void Div(DecodedInstruction instruct)
        {
            var left = instruct.operands[0].GetShort();
            var right = instruct.operands[1].GetShort();
            if (left == 0 || right == 0) throw new DivideByZeroException("Division by 0 error");
            // Depending on the operand types depends if we have a value or pointer to a variable
            var lValue = LibraryUtilities.GetOperandValue(this.CallStack.Peek(), instruct.operands[0].operandType, left);
            var rValue = LibraryUtilities.GetOperandValue(this.CallStack.Peek(), instruct.operands[1].operandType, right);

            ushort result = (ushort)(lValue / rValue);
            LibraryUtilities.StoreResult(GameData, CallStack, instruct, StoryHeader.GlobalVariables, result);
        }

        internal void Store(DecodedInstruction instruct)
        {
            var left = instruct.operands[0].GetShort();
            var right = instruct.operands[1].GetShort();
            // Store is NOT itself a store md.
            // LibraryUtilities.StoreResult(GameData, CallStack, instruct, 0, right);
            switch (left)
            {

                case 0:             // Stack
                    CallStack.Peek().localStack.Push(right);
                    break;
                case > 0 and < 16: // Local vars
                    {
                        var localVars = CallStack.Peek().locals;
                        localVars[left] = right;
                    }
                    break;
                case > 15 and <= 255: // Global
                    var variable = (instruct.store - 15) * 2;
                    var resultArray = left.ToByteArray();
                    var globalVariables = GameData[StoryHeader.GlobalVariables];
                    GameData[globalVariables + variable] = resultArray[0];
                    GameData[globalVariables + variable + 1] = resultArray[1];
                    break;
            }

        }
    }
}
