﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2.InstructionDefinitions
{
    internal class Instructions_0OP
    {
        public List<InstructionDefinition> Instructions = new()
        {
            new InstructionDefinition(Store:false, Branch:false,OpCode:"0OP:176", DecCode:0, Version:MinVersion.All, Name:"rtrue"),
new InstructionDefinition(Store:false, Branch:false,OpCode:"0OP:177", DecCode:1, Version:MinVersion.All, Name:"rfalse"),
new InstructionDefinition(Store:false, Branch:false,OpCode:"0OP:178", DecCode:2, Version:MinVersion.All, Name:"print "),
new InstructionDefinition(Store:false, Branch:false,OpCode:"0OP:179", DecCode:3, Version:MinVersion.All, Name:"print_ret "),
new InstructionDefinition(Store:false, Branch:false,OpCode:"0OP:180", DecCode:4, Version:MinVersion.OneOnly, Name:"nop"),
new InstructionDefinition(Store:false, Branch:true,OpCode:"0OP:181", DecCode:5, Version:(MinVersion)1, Name:"save "),
new InstructionDefinition(Store:false, Branch:false,OpCode:"", DecCode:0, Version:(MinVersion)4, Name:"save "),
new InstructionDefinition(Store:false, Branch:false,OpCode:"", DecCode:0, Version:(MinVersion)5, Name:""),
new InstructionDefinition(Store:false, Branch:true,OpCode:"0OP:182", DecCode:6, Version:(MinVersion)1, Name:"restore "),
new InstructionDefinition(Store:false, Branch:false,OpCode:"", DecCode:0, Version:(MinVersion)4, Name:"restore "),
new InstructionDefinition(Store:false, Branch:false,OpCode:"", DecCode:0, Version:(MinVersion)5, Name:""),
new InstructionDefinition(Store:false, Branch:false,OpCode:"0OP:183", DecCode:7, Version:MinVersion.All, Name:"restart"),
new InstructionDefinition(Store:false, Branch:false,OpCode:"0OP:184", DecCode:8, Version:MinVersion.All, Name:"ret_popped"),
new InstructionDefinition(Store:false, Branch:false,OpCode:"0OP:185", DecCode:9, Version:(MinVersion)1, Name:"pop"),
new InstructionDefinition(Store:true, Branch:false,OpCode:"", DecCode:0, Version:MinVersion.FiveOrThree, Name:"catch "),
new InstructionDefinition(Store:false, Branch:false,OpCode:"0OP:186", DecCode:10, Version:MinVersion.All, Name:"quit"),
new InstructionDefinition(Store:false, Branch:false,OpCode:"0OP:187", DecCode:11, Version:MinVersion.All, Name:"new_line"),
new InstructionDefinition(Store:false, Branch:false,OpCode:"0OP:188", DecCode:12, Version:(MinVersion)3, Name:"show_status"),
new InstructionDefinition(Store:false, Branch:false,OpCode:"", DecCode:0, Version:(MinVersion)4, Name:""),
new InstructionDefinition(Store:false, Branch:true,OpCode:"0OP:189", DecCode:13, Version:(MinVersion)3, Name:"verify "),
new InstructionDefinition(Store:false, Branch:false,OpCode:"0OP:190", DecCode:14, Version:(MinVersion)5, Name:"extended"),
new InstructionDefinition(Store:false, Branch:true,OpCode:"0OP:191", DecCode:15, Version:MinVersion.FiveOnly, Name:"piracy "),

        };
    }
}
