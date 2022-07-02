using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2.InstructionDefinitions
{
    internal class Instructions
    {
        private readonly ZMachineVersion version;
        private Dictionary<string, InstructionDefinition> instructions = new Dictionary<string, InstructionDefinition>();
        public Instructions(int version)
        {
            this.version = MachineExtensions.GetZMachineVersion(version);
            CreateInstructionList();
        }

        // Pile them all in to a coherent pack.
        // With a little filtering for the duplicates.
        private void CreateInstructionList()
        {
            IEnumerable<InstructionDefinition> ZeroOp = Instructions_0OP.Instructions.Where(a => a.Version.HasFlag(this.version));
            IEnumerable<InstructionDefinition> OneOp = Instructions_1OP.Instructions.Where(a => a.Version.HasFlag(this.version));
            IEnumerable<InstructionDefinition> TwoOp = Instructions_2OP.Instructions.Where(a => a.Version.HasFlag(this.version));
            IEnumerable<InstructionDefinition> VarOp = Instructions_Var.Instructions.Where(a => a.Version.HasFlag(this.version));
            IEnumerable<InstructionDefinition> ExtOp = Instructions_Ext.Instructions.Where(a => a.Version.HasFlag(this.version));

            instructions = instructions.
                                Concat(ZeroOp.Select(a => KeyValuePair.Create(a.OpCode, a))).
                                Concat(OneOp.Select(a => KeyValuePair.Create(a.OpCode, a))).
                                Concat(TwoOp.Select(a => KeyValuePair.Create(a.OpCode, a))).
                                Concat(VarOp.Select(a => KeyValuePair.Create(a.OpCode, a))).
                                Concat(ExtOp.Select(a => KeyValuePair.Create(a.OpCode, a))).
                                ToDictionary(a => a.Key, b => b.Value);

        }

        public InstructionDefinition GetInstruction(string OpCode) => instructions[OpCode];
    }
}
