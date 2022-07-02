using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zmachine.V2.InstructionDefinitions;

namespace Zmachine.V2
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
        ///  Converts consecutive byte values into a short, then formats that into a Hex string.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static string Get2ByteValueHex(this byte[] @this, int idx)
        {
            return $"{MachineExtensions.Get2ByteValue(@this,idx):X}";
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

        internal static ZMachineVersion GetZMachineVersion(int version) => version switch
        {
            1 => ZMachineVersion.One,
            2 => ZMachineVersion.Two,
            3 => ZMachineVersion.Three,
            4 => ZMachineVersion.Four,
            5 => ZMachineVersion.Five,
            6 => ZMachineVersion.Six,
            7 => ZMachineVersion.Seven,
            8 => ZMachineVersion.Eight,
            _ => throw new ArgumentOutOfRangeException("Version number not found")
        };
    }
}
