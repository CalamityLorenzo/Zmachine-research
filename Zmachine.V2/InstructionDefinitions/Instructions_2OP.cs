namespace Zmachine.V2.InstructionDefinitions
{
    using static ZMachineVersion;
    internal static class Instructions_2OP
    {
        public static List<InstructionDefinition> Instructions = new()
        {
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:1", DecCode:1, Version:All, Name:"je"),
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:2", DecCode:2, Version:All, Name:"jl"),
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:3", DecCode:3, Version:All, Name:"jg"),
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:4", DecCode:4, Version:All, Name:"dec_chk"),
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:5", DecCode:5, Version:All, Name:"inc_chk"),
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:6", DecCode:6, Version:All, Name:"jin"),
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:7", DecCode:7, Version:All, Name:"test"),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:8", DecCode:8, Version:All, Name:"or"),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:9", DecCode:9, Version:All, Name:"and"),
        new InstructionDefinition(Store:false, Branch:true,OpCode:"2OP:10", DecCode:10, Version:All, Name:"test_attr"),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:11", DecCode:11, Version:All, Name:"set_attr"),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:12", DecCode:12, Version:All, Name:"clear_attr"),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:13", DecCode:13, Version:All, Name:"store"),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:14", DecCode:14, Version:All, Name:"insert_obj"),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:15", DecCode:15, Version:All, Name:"loadw"),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:16", DecCode:16, Version:All, Name:"loadb"),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:17", DecCode:17, Version:All, Name:"get_prop"),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:18", DecCode:18, Version:All, Name:"get_prop_addr"),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:19", DecCode:19, Version:All, Name:"get_next_prop"),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:20", DecCode:20, Version:All, Name:"add"),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:21", DecCode:21, Version:All, Name:"sub"),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:22", DecCode:22, Version:All, Name:"mul"),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:23", DecCode:23, Version:All, Name:"div"),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:24", DecCode:24, Version:All, Name:"mod"),
        new InstructionDefinition(Store:true, Branch:false,OpCode:"2OP:25", DecCode:25, Version:Four | FiveAndUp, Name:"call_2s"),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:26", DecCode:26, Version:FiveAndUp, Name:"call_2n"),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:27", DecCode:27, Version:Five, Name:"set_colour"),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:27", DecCode:0, Version:SixAndUp, Name:"set_colour"),
        new InstructionDefinition(Store:false, Branch:false,OpCode:"2OP:28", DecCode:28, Version:FiveAndUp, Name:"throw"),




        };
    }
}
