using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2.Machines
{
    public partial class Zmachine_v2
    {
        public void Tick()
        {
            ProcessOpcode();
            ProgramCounter += 1;
        }

        private void ProcessOpcode()
        {
            switch (Memory[ProgramCounter])
            {
                case 143: // 0x8F
                    {
                        if (this.HeaderDetails.Version < 5)
                            this.or(ProgramCounter);
                        else
                        {
                            this.call_1n(ProgramCounter);
                        }
                        break;
                    }

            }
            
        }

        /// <summary>
        /// Executes routine() and throws away result.
        /// </summary>
        /// <param name="address"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void call_1n(int address)
        {
            // It's a packed address so those weird rules apply.
            //var routineAddress =  GetPackedAddress(HeaderDetails.Version,  Memory.Get2ByteValue(address + 1), HeaderDetails.RoutinesOffset, HeaderDetails.StaticStringsOffSet);

        }

        private void or(int address)
        {
            throw new NotImplementedException();
        }

        // Load a byte from memory
        internal void loadb(byte address)
        {
            var data = this.Memory[address];
        }

        // lord a word from memory{
        internal void loadw(byte address)
        {
            var data = this.Memory.Get2ByteValue(address * 2);
        }
    }
}
