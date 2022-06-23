using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
