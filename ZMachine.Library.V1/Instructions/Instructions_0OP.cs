namespace ZMachine.Library.V1.Instructions
{
    using static FeaturesVersion;
    internal class Instructions_0OP
    {
        public static List<MachineInstruction> Instructions = new()
        {
            new MachineInstruction(Store:false, Branch:false,OpCode:"0OP:176", DecCode:0, Version:All, Name:"rtrue"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"0OP:177", DecCode:1, Version:All, Name:"rfalse"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"0OP:178", DecCode:2, Version:All, Name:"print"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"0OP:179", DecCode:3, Version:All, Name:"print_ret"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"0OP:180", DecCode:4, Version:All, Name:"nop"),
            new MachineInstruction(Store:false, Branch:true,OpCode:"0OP:181", DecCode:5, Version:One, Name:"save"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"0OP:181", DecCode:5, Version: Three | Four, Name:"save"),
            // new MachineInstruction(Store:false, Branch:false,OpCode:"", DecCode:0, Version:(ZMachineVersion)5, Name:""),
            new MachineInstruction(Store:false, Branch:true,OpCode:"0OP:182", DecCode:6, Version:One, Name:"restore"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"0OP:182", DecCode:0, Version:Three | Four, Name:"restore"),
            // new MachineInstruction(Store:false, Branch:false,OpCode:"", DecCode:0, Version:(ZMachineVersion)5, Name:""),
            new MachineInstruction(Store:false, Branch:false,OpCode:"0OP:183", DecCode:7, Version:All, Name:"restart"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"0OP:184", DecCode:8, Version:All, Name:"ret_popped"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"0OP:185", DecCode:9, Version: UpToFour, Name:"pop"),
            new MachineInstruction(Store:true, Branch:false,OpCode:"0OP:185", DecCode:0, Version:FiveAndUp, Name:"catch"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"0OP:186", DecCode:10, Version:All, Name:"quit"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"0OP:187", DecCode:11, Version:All, Name:"new_line"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"0OP:188", DecCode:12, Version:Three, Name:"show_status"),
            new MachineInstruction(Store:false, Branch:true,OpCode:"0OP:189", DecCode:13, Version: Three | Four, Name:"verify"),
            new MachineInstruction(Store:false, Branch:false,OpCode:"0OP:190", DecCode:14, Version:FiveAndUp, Name:"extended"),
            new MachineInstruction(Store:false, Branch:true,OpCode:"0OP:191", DecCode:15, Version:FiveAndUp, Name:"piracy"),

        };
    }
}
