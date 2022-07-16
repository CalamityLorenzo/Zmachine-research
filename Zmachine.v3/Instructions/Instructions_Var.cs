using Zmachine.V3.Machines;

namespace Zmachine.V3.Instructions
{
    using static MachineVersion;
    internal class Instructions_Var
    {
        public static List<MachineInstruction> Instructions = new()
        {
            new MachineInstruction(Store: true, Branch: false, OpCode:"VAR:224", DecCode: 0, Version: One | Three, Name:"call"),
            new MachineInstruction(Store: true, Branch: false, OpCode:"VAR:224", DecCode: 0, Version:  Four | FiveAndUp , Name:"call_vs"),

            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:225", DecCode: 1, Version: All, Name:"storew"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:226", DecCode: 2, Version: All, Name:"storeb"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:227", DecCode: 3, Version: All, Name:"put_prop"),

            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:228", DecCode: 4, Version: One, Name:"sread"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:228", DecCode: 0, Version: Three | Four, Name:"sread"),
            new MachineInstruction(Store: true, Branch: false, OpCode:"VAR:228", DecCode: 0, Version: FiveAndUp, Name:"aread"),

            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:229", DecCode: 5, Version: All, Name:"print_char"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:230", DecCode: 6, Version: All, Name:"print_num"),
            new MachineInstruction(Store: true, Branch: false, OpCode:"VAR:231", DecCode: 7, Version: All, Name:"random"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:232", DecCode: 8, Version: All, Name:"push"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:233", DecCode: 9, Version: UpToFour | Five, Name:"pull"),
             
            new MachineInstruction(Store: true, Branch: false, OpCode:"VAR:233", DecCode: 0, Version: Six | Seven, Name:"pull"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:234", DecCode: 10, Version: Three | Four, Name:"split_window"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:235", DecCode: 11, Version: Three | Four | FiveAndUp, Name:"set_window"),
            new MachineInstruction(Store: true, Branch: false, OpCode:"VAR:236", DecCode: 12, Version:  Four|FiveAndUp, Name:"call_vs2"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:237", DecCode: 13, Version: Four|FiveAndUp, Name:"erase_window"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:238", DecCode: 14, Version: Four|Five, Name:"erase_line"),
            //
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:238", DecCode: 0, Version: SixAndUp, Name:"erase_line"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:239", DecCode: 15, Version: Four|Five, Name:"set_cursor"),
            //
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:239", DecCode: 0, Version: SixAndUp, Name:"set_cursor"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:240", DecCode: 16, Version: Four| FiveAndUp, Name:"get_cursor"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:241", DecCode: 17, Version:Four | FiveAndUp, Name:"set_text_style"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:242", DecCode: 18, Version: Four | FiveAndUp, Name:"buffer_mode"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:243", DecCode: 19, Version: Three | Four, Name:"output_stream"),
            //
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:243", DecCode: 0, Version: Five, Name:"output_stream"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:243", DecCode: 0, Version: SixAndUp, Name:"output_stream"),

            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:244", DecCode: 20, Version: Three | Four | FiveAndUp, Name:"input_stream"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:245", DecCode: 21, Version: Three| Five, Name:"sound_effect"), // that's the weird one described in 5 yet used in a 3
            new MachineInstruction(Store: true, Branch: false, OpCode:"VAR:246", DecCode: 22, Version:Four | FiveAndUp, Name:"read_char"),
            new MachineInstruction(Store: true, Branch: true, OpCode:"VAR:247", DecCode: 23, Version: Four | FiveAndUp, Name:"scan_table"),
            new MachineInstruction(Store: true, Branch: false, OpCode:"VAR:248", DecCode: 24, Version: FiveAndUp, Name:"not"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:249", DecCode: 25, Version: FiveAndUp, Name:"call_vn"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:250", DecCode: 26, Version: FiveAndUp, Name:"call_vn2"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:251", DecCode: 27, Version: FiveAndUp, Name:"tokenise"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:252", DecCode: 28, Version: FiveAndUp, Name:"encode_text"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:253", DecCode: 29, Version: FiveAndUp, Name:"copy_table"),
            new MachineInstruction(Store: false, Branch: false, OpCode:"VAR:254", DecCode: 30, Version: FiveAndUp, Name:"print_table"),
            new MachineInstruction(Store: false, Branch: true, OpCode:"VAR:255", DecCode: 31, Version: FiveAndUp, Name:"check_arg_count"),

        };
    }
}
