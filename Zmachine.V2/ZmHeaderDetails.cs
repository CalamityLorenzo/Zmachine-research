﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2
{
    public struct ZmHeaderDetails
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
        public BitArray Flags { get; set; }
        public BitArray Flags2 { get; set; }
        public int AbbreviationsTable { get; set; }
        public int LengthOfFile { get; set; }
        public string DateCompiled { get; set; }
        public ushort StaticStringsOffSet { get; set; }
        public ushort RoutinesOffset { get; set; }

    }
}
