using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2.InstructionDefinitions
{
    internal static class Instructions_2OP
    {
        public static List<InstructionDefinition> Instructions = new()
        {
             new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:1", DecCode:1, Version:MinVersion.All, Name:"je "),
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:2", DecCode:2, Version:MinVersion.All, Name:"jl "),
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:3", DecCode:3, Version:MinVersion.All, Name:"jg "),
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:4", DecCode:4, Version:MinVersion.All, Name:"dec_chk "),
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:5", DecCode:5, Version:MinVersion.All, Name:"inc_chk "),
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:6", DecCode:6, Version:MinVersion.All, Name:"jin "),
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:7", DecCode:7, Version:MinVersion.All, Name:"test "),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:8", DecCode:8, Version:MinVersion.All, Name:"or "),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:9", DecCode:9, Version:MinVersion.All, Name:"and "),
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:10", DecCode:10, Version:MinVersion.All, Name:"test_attr "),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:11", DecCode:11, Version:MinVersion.All, Name:"set_attr "),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:12", DecCode:12, Version:MinVersion.All, Name:"clear_attr "),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:13", DecCode:13, Version:MinVersion.All, Name:"store "),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:14", DecCode:14, Version:MinVersion.All, Name:"insert_obj "),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:15", DecCode:15, Version:MinVersion.All, Name:"loadw "),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:16", DecCode:16, Version:MinVersion.All, Name:"loadb "),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:17", DecCode:17, Version:MinVersion.All, Name:"get_prop "),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:18", DecCode:18, Version:MinVersion.All, Name:"get_prop_addr "),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:19", DecCode:19, Version:MinVersion.All, Name:"get_next_prop "),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:20", DecCode:20, Version:MinVersion.All, Name:"add "),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:21", DecCode:21, Version:MinVersion.All, Name:"sub "),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:22", DecCode:22, Version:MinVersion.All, Name:"mul "),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:23", DecCode:23, Version:MinVersion.All, Name:"div "),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:24", DecCode:24, Version:MinVersion.All, Name:"mod "),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:25", DecCode:25, Version:(MinVersion)4, Name:"call_2s "),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:26", DecCode:26, Version:(MinVersion)5, Name:"call_2n "),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:27", DecCode:27, Version:(MinVersion)5, Name:"set_colour"),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"", DecCode:0, Version:(MinVersion)6, Name:"set_colour"),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:28", DecCode:28, Version:(MinVersion)44717, Name:"throw "),




        };
    }
}
