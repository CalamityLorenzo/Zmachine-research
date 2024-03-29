﻿using System.Collections;
using ZMachine.Library.V1.Instructions;

namespace ZMachine.Library.V1.Utilities
{
    public static class LibraryUtilities
    {
        /// <summary>
        ///  Converts consecutive byte values into a short
        /// </summary>
        /// <param name="this"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static UInt16 Get2ByteValue(this byte[] @this, int idx)
        {
            return (UInt16)(@this[idx] << 8 | @this[idx + 1]);
        }

        /// <summary>
        ///  Converts consecutive byte values into a short
        /// </summary>
        /// <param name="this"></param>
        /// <param name="idx"></param>
        /// <returns>UInt16 </returns> 
        public static UInt16 Get2ByteValue(this Span<byte> @this, int idx)
        {
            return (UInt16)(@this[idx] << 8 | @this[idx + 1]);
        }

        /// <summary>
        ///  Converts consecutive byte values into a short, then formats that into a Hex string.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="idx"></param>
        /// <returns>string</returns>
        public static string Get2ByteValueHex(this byte[] @this, int idx)
        {
            return $"{LibraryUtilities.Get2ByteValue(@this, idx):X4}";
        }



        /// <summary>
        ///  Converts consecutive byte values into a short, then formats that into a Hex string.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="idx"></param>
        /// <returns>string</returns>
        public static string Get2ByteValueHex(this Span<byte> @this, int idx)
        {
            return $"{LibraryUtilities.Get2ByteValue(@this, idx):X}";
        }

        /// <summary>
        /// Takes in a numner and converts into a verion approrpiate address
        /// </summary>
        /// <param name="version"></param>
        /// <param name="address"></param>
        /// <param name="rOffset"></param>
        /// <param name="sOffset"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int GetPackedAddress(this int @this, int version, int rOffset, int sOffset)
        {
            return version switch
            {
                1 or 2 or 3 => @this * 2,
                4 or 5 => @this * 4,
                6 or 7 => @this * 4 + (rOffset > 0 ? rOffset : sOffset),
                8 => @this * 8,
                _ => throw new ArgumentOutOfRangeException("Version is out of range")
            };
        }

        public static FeaturesVersion GetFeatureVersion(int version) => version switch
        {
            1 => FeaturesVersion.One,
            2 => FeaturesVersion.Two,
            3 => FeaturesVersion.Three,
            4 => FeaturesVersion.Four,
            5 => FeaturesVersion.Five,
            6 => FeaturesVersion.Six,
            7 => FeaturesVersion.Seven,
            8 => FeaturesVersion.Eight,
            _ => throw new ArgumentOutOfRangeException("Version number not found")
        };

        /// <summary>
        /// Program counter is feature locked.
        /// obviously. Providing the routine offset is a trade off. We could always pass in the compelte header object in stead.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="programCounterInitalValue"></param>
        /// <param name="routineOffSet"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int SetProgramCounterInitialValue(int version, int programCounterInitalValue, int routineOffSet = 0)
        {
            // V1 Program counter initial value is a byte
            // v6 Packed address over two bytes
            if (version < 6)
                return programCounterInitalValue;
            else
            {
                if (version > 5 && routineOffSet == 0) throw new ArgumentNullException($"Routine offset cannot be 0 with version{version}");
                return version switch
                {
                    // 4 or 5 => programCounterInitalValue * 4,
                    6 or 7 => programCounterInitalValue * 4 + routineOffSet,
                    8 => programCounterInitalValue * 8,
                    _ => throw new ArgumentOutOfRangeException("Version is out of range")
                };
            }
        }

        /// <summary>
        /// Gets't the correct set of instructions for the current machine version.
        /// This isn't perfect ,as some instructions maybe the same between versions, but implemented differently.
        /// That is handled at runtime.
        /// </summary>
        /// <param name="featuresVersion">The binary flag version info.</param>
        /// <returns>Dictionary<string, MachineInstruction> </returns>
        public static Dictionary<string, MachineInstruction> GetVersionInstructions(FeaturesVersion featuresVersion)
        {

            // Pile them all in to a coherent pack.
            // With a little filtering for the duplicates.
            IEnumerable<MachineInstruction> ZeroOp = Instructions_0OP.Instructions.Where(a => a.Version.HasFlag(featuresVersion));
            IEnumerable<MachineInstruction> OneOp = Instructions_1OP.Instructions.Where(a => a.Version.HasFlag(featuresVersion));
            IEnumerable<MachineInstruction> TwoOp = Instructions_2OP.Instructions.Where(a => a.Version.HasFlag(featuresVersion));
            IEnumerable<MachineInstruction> VarOp = Instructions_Var.Instructions.Where(a => a.Version.HasFlag(featuresVersion));
            IEnumerable<MachineInstruction> ExtOp = Instructions_Ext.Instructions.Where(a => a.Version.HasFlag(featuresVersion));

            return ZeroOp.Select(a => KeyValuePair.Create(a.OpCode, a)).
                      Concat(OneOp.Select(a => KeyValuePair.Create(a.OpCode, a))).
                      Concat(TwoOp.Select(a => KeyValuePair.Create(a.OpCode, a))).
                      Concat(VarOp.Select(a => KeyValuePair.Create(a.OpCode, a))).
                      Concat(ExtOp.Select(a => KeyValuePair.Create(a.OpCode, a))).
                      ToDictionary(a => a.Key, b => b.Value);


        }

        public static ushort GetShort(this InstructionOperands @this)
        {
            if (@this.operand.Length > 1)
                return (ushort)(@this.operand[0] << 8 | @this.operand[1]);
            else
                return (ushort)@this.operand[0];
        }

        public static byte[] ToByteArray(this ushort @this) => new byte[]
            {
                (byte)(@this >> 8),
                (byte)(@this & 0xffff),
            };

        public static void StoreResult(byte[] Memory, Stack<ActivationRecord> stack, DecodedInstruction currentInstr, ushort globalVariables, ushort result)
        {
            switch (currentInstr.store)
            {

                case 0:             // Stack
                    stack.Peek().localStack.Push(result);
                    break;
                case > 0 and < 16: // Local vars
                    {
                        var localVars = stack.Peek().locals;
                        localVars[currentInstr.store] = result;
                    }
                    break;
                case > 15 and <= 255: // Global
                    var variable = (currentInstr.store - 15) * 2;
                    var resultArray = result.ToByteArray();
                    Memory[globalVariables + variable] = resultArray[0];
                    Memory[globalVariables + variable + 1] = resultArray[1];
                    break;
            }
        }

        public static ushort GetOperandValue(ActivationRecord stackRecord,OperandType operandType, ushort value)
        {
            return operandType switch
            {
                OperandType.Variable => stackRecord.locals[value],
                OperandType.SmallConstant => value,
                OperandType.LargeConstant => value,
            };
        }

        public static ushort[] ConvertAttributes(this IEnumerable<byte> attributes)
        {
            var bitArray = new BitArray(attributes.ToArray());
            var assignedBits = new List<ushort>();
            // To all intents and purposes the bitarry is backwards for us
            // Least significant bits to most l->r. 
            // Unlike when we eyeball it. msb->lsb.
            // so count backwards
            ushort bitOrder = 0;
            for(var ctr = bitArray.Length-1; ctr >= 0; --ctr)
            {
                if (bitArray[ctr])
                    assignedBits.Add(bitOrder);
                    bitOrder++;
            }
            return assignedBits.ToArray();
        }

    }
}
