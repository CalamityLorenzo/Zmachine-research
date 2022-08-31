using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zmachine.Library.V2.Utilities;

namespace Zmachine.Library.V2
{
    internal class GlobalVariables
    {
        private readonly byte[] memory;
        private readonly ushort globalStartAddress;

        public GlobalVariables(Span<byte> memory, ushort globalStartAddress)
        {
            this.memory = memory.ToArray();
            this.globalStartAddress = globalStartAddress;
        }

        public ushort this[int idx] { 
            get => this.GetGlobalVariable(idx);
            set => this.SetGlobalVariable(idx, value);
        }

        private void SetGlobalVariable(int idx, ushort value)
        {
            if (idx < 0 || idx > 239) throw new ArgumentOutOfRangeException($"Index out of range {idx}");
            var globalVarAddress = globalStartAddress + (idx * 2);
            var array = value.ToByteArray();
            memory[globalVarAddress] = array[0];
            memory[globalVarAddress+1] = array[1];
        }

        /// <summary>
        /// This assumes the incoming variable is direect from the memory sauce.
        /// </summary>
        /// <param name="hexAddress"></param>
        /// <returns>Contents of the that global memorty pobjets</returns>
        public ushort this[string hexAddress]=>this.GetGlobalVariable(int.Parse(hexAddress, System.Globalization.NumberStyles.HexNumber)-16);

        private ushort GetGlobalVariable(int idx)
        {
            if (idx < 0 || idx > 239) throw new ArgumentOutOfRangeException($"Index out of range {idx}");
            var globalVarAddress = globalStartAddress + (idx * 2);
            return (ushort)(memory[globalVarAddress] << 8 | memory[globalVarAddress + 1]);
        }
    }
}
