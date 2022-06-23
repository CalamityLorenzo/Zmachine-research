using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.Workings
{
    public struct ZMachineDetails
    {
        public string ReleaseNumber { get; internal set; }
        public ushort HighMemStart { get; internal set; }
        public byte InterpreterVersion { get; internal set; }
        public byte InterpreterNumber { get; internal set; }
        public int Version { get; set; }

        public int ProgramCounterInitalValue { get; set; }
        public int Dictionary { get; set; }
        public int ObjectTable { get; set; }

        public int GlobalObjects { get; set; }
        public int StaticStart { get; set; }

        public int Flags { get; set; }
        public int Flags2 { get; set; }
        public int AbbreviationsTable { get; set; }
        public int LengthOfFile { get; set; }
        public string DateCompiled { get; set; }
        public string StaticStringsOffSet { get; set; }

    }

}
