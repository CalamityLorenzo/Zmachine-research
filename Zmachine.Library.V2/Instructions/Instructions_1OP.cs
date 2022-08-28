namespace Zmachine.Library.V2.Instructions
{
    using static FeaturesVersion;
    internal class Instructions_1OP
    {
        public static List<MachineInstruction> Instructions => new()
        {
            new MachineInstruction(Store:false, Branch:true,OpCode:"1OP:128", DecCode:0, Version:All, Name:"jz"),
            new MachineInstruction(Store:true, Branch:true,OpCode:"1OP:129", DecCode:1, Version:All, Name:"get_sibling"),
            new MachineInstruction(Store:true, Branch:true,OpCode:"1OP:130", DecCode:2, Version:All, Name:"get_child"),
            new MachineInstruction(Store:true, Branch:false,OpCode:"1OP:131", DecCode:3, Version:All, Name:"get_parent"),
            new MachineInstruction(Store:true, Branch:false,OpCode:"1OP:132", DecCode:4, Version:All, Name:"get_prop_len"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"1OP:133", DecCode:5, Version:All, Name:"inc"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"1OP:134", DecCode:6, Version:All, Name:"dec"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"1OP:135", DecCode:7, Version:All, Name:"print_addr"),
            new MachineInstruction(Store:true, Branch:false,OpCode:"1OP:136", DecCode:8, Version:Four | FiveAndUp, Name:"call_1s"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"1OP:137", DecCode:9, Version:All, Name:"remove_obj"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"1OP:138", DecCode:10, Version:All, Name:"print_obj"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"1OP:139", DecCode:11, Version:All, Name:"ret"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"1OP:140", DecCode:12, Version:All, Name:"jump"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"1OP:141", DecCode:13, Version:All, Name:"print_paddr"),
            new MachineInstruction(Store:true, Branch:false,OpCode:"1OP:142", DecCode:14, Version:All, Name:"load"),
            new MachineInstruction(Store:true, Branch:false,OpCode:"1OP:143", DecCode:15, Version:UpToFour, Name:"not"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"1OP:143", DecCode:15, Version:FiveAndUp, Name:"call_1n"),
        };
    }
}
