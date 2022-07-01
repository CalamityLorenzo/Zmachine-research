using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2.InstructionDefinitions
{
    internal class Instructions_1OP
    {
        public static List<InstructionDefinition> Instructions => new()
        {
            new InstructionDefinition(Store:false, Branch:true,OpCode:"1OP:128", DecCode:0, Version:MinVersion.All, Name:"jz"),
            new InstructionDefinition(Store:true, Branch:true,OpCode:"1OP:129", DecCode:1, Version:MinVersion.All, Name:"get_sibling"),
            new InstructionDefinition(Store:true, Branch:true,OpCode:"1OP:130", DecCode:2, Version:MinVersion.All, Name:"get_child"),
            new InstructionDefinition(Store:true, Branch:false,OpCode:"1OP:131", DecCode:3, Version:MinVersion.All, Name:"get_parent"),
            new InstructionDefinition(Store:true, Branch:false,OpCode:"1OP:132", DecCode:4, Version:MinVersion.All, Name:"get_prop_len"),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"1OP:133", DecCode:5, Version:MinVersion.All, Name:"inc"),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"1OP:134", DecCode:6, Version:MinVersion.All, Name:"dec"),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"1OP:135", DecCode:7, Version:MinVersion.All, Name:"print_addr"),
            new InstructionDefinition(Store:true, Branch:false,OpCode:"1OP:136", DecCode:8, Version:(MinVersion)4, Name:"call_1s"),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"1OP:137", DecCode:9, Version:MinVersion.All, Name:"remove_obj"),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"1OP:138", DecCode:10, Version:MinVersion.All, Name:"print_obj"),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"1OP:139", DecCode:11, Version:MinVersion.All, Name:"ret"),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"1OP:140", DecCode:12, Version:MinVersion.All, Name:"jump"),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"1OP:141", DecCode:13, Version:MinVersion.All, Name:"print_paddr"),
            new InstructionDefinition(Store:true, Branch:false,OpCode:"1OP:142", DecCode:14, Version:MinVersion.All, Name:"load"),
            new InstructionDefinition(Store:true, Branch:false,OpCode:"1OP:143", DecCode:15, Version:MinVersion.OneToFour, Name:"not"),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"1OP:143", DecCode:15, Version:(MinVersion)5, Name:"call_1n"),
        };
    }
}
