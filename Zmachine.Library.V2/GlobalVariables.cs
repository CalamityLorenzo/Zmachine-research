using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ushort this[int idx] => this.GetGlobalVariable(idx);

        private ushort GetGlobalVariable(int idx)
        {
            if (idx < 0 || idx > 239) throw new ArgumentOutOfRangeException($"Index out of range {idx}");
            var globalVarAddress = globalStartAddress + ((idx - 16) * 2);
            return (ushort)(memory[globalVarAddress] << 8 | memory[globalVarAddress + 1]);
        }
    }
}
