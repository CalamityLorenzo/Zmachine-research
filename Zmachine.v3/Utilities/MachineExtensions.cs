using Zmachine.V3.Instructions;
using Zmachine.V3.Machines;

namespace Zmachine.V3
{
    internal static class MachineExtensions
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
            return $"{MachineExtensions.Get2ByteValue(@this, idx):X}";
        }



        /// <summary>
        ///  Converts consecutive byte values into a short, then formats that into a Hex string.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="idx"></param>
        /// <returns>string</returns>
        public static string Get2ByteValueHex(this Span<byte> @this, int idx)
        {
            return $"{MachineExtensions.Get2ByteValue(@this, idx):X}";
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
        internal static int GetPackedAddress(this int @this, int version, int rOffset, int sOffset)
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

        internal static MachineVersion GetFeatureVersion(int version) => version switch
        {
            1 => MachineVersion.One,
            2 => MachineVersion.Two,
            3 => MachineVersion.Three,
            4 => MachineVersion.Four,
            5 => MachineVersion.Five,
            6 => MachineVersion.Six,
            7 => MachineVersion.Seven,
            8 => MachineVersion.Eight,
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
        internal static int SetProgramCounterInitialValue(int version, int programCounterInitalValue, int routineOffSet = 0)
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
        internal static Dictionary<string, MachineInstruction> GetVersionInstructions(MachineVersion featuresVersion)
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
    }
}
