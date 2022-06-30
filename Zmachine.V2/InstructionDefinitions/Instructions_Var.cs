using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2.InstructionDefinitions
{
    internal class Instructions_Var
    {
        public List<InstructionDefinition> InstructionDefinitions = new()
        {
            new InstructionDefinition(Store: true, Branch: false, OpCode: "VAR:224", DecCode: 0, Version: (MinVersion)1, Name: "call "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "", DecCode: 0, Version: (MinVersion)4, Name: "call_vs "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:225", DecCode: 1, Version: MinVersion.All, Name: "storew "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:226", DecCode: 2, Version: MinVersion.All, Name: "storeb "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:227", DecCode: 3, Version: MinVersion.All, Name: "put_prop "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:228", DecCode: 4, Version: (MinVersion)1, Name: "sread "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "", DecCode: 0, Version: (MinVersion)4, Name: "sread "),
            new InstructionDefinition(Store: true, Branch: false, OpCode: "", DecCode: 0, Version: (MinVersion)5, Name: "aread "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:229", DecCode: 5, Version: MinVersion.All, Name: "print_char "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:230", DecCode: 6, Version: MinVersion.All, Name: "print_num "),
            new InstructionDefinition(Store: true, Branch: false, OpCode: "VAR:231", DecCode: 7, Version: MinVersion.All, Name: "random "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:232", DecCode: 8, Version: MinVersion.All, Name: "push "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:233", DecCode: 9, Version: (MinVersion)1, Name: "pull "),
            new InstructionDefinition(Store: true, Branch: false, OpCode: "", DecCode: 0, Version: (MinVersion)6, Name: "pull "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:234", DecCode: 10, Version: (MinVersion)3, Name: "split_window "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:235", DecCode: 11, Version: (MinVersion)3, Name: "set_window "),
            new InstructionDefinition(Store: true, Branch: false, OpCode: "VAR:236", DecCode: 12, Version: (MinVersion)4, Name: "call_vs2 "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:237", DecCode: 13, Version: (MinVersion)4, Name: "erase_window "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:238", DecCode: 14, Version: MinVersion.FourOnly, Name: "erase_line "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "", DecCode: 0, Version: (MinVersion)6, Name: "erase_line "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:239", DecCode: 15, Version: (MinVersion)4, Name: "set_cursor "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "", DecCode: 0, Version: (MinVersion)6, Name: "set_cursor "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:240", DecCode: 16, Version: MinVersion.FourOrSix, Name: "get_cursor "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:241", DecCode: 17, Version: (MinVersion)4, Name: "set_text_style "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:242", DecCode: 18, Version: (MinVersion)4, Name: "buffer_mode "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:243", DecCode: 19, Version: (MinVersion)3, Name: "output_stream "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "", DecCode: 0, Version: (MinVersion)5, Name: "output_stream "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "", DecCode: 0, Version: (MinVersion)6, Name: "output_stream "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:244", DecCode: 20, Version: (MinVersion)3, Name: "input_stream "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:245", DecCode: 21, Version: (MinVersion)MinVersion.FiveOrThree, Name: "sound_effect "),
            new InstructionDefinition(Store: true, Branch: false, OpCode: "VAR:246", DecCode: 22, Version: (MinVersion)4, Name: "read_char "),
            new InstructionDefinition(Store: true, Branch: true, OpCode: "VAR:247", DecCode: 23, Version: (MinVersion)4, Name: "scan_table "),
            new InstructionDefinition(Store: true, Branch: false, OpCode: "VAR:248", DecCode: 24, Version: MinVersion.FiveOrSix, Name: "not "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:249", DecCode: 25, Version: (MinVersion)5, Name: "call_vn "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:250", DecCode: 26, Version: (MinVersion)5, Name: "call_vn2 "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:251", DecCode: 27, Version: (MinVersion)5, Name: "tokenise "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:252", DecCode: 28, Version: (MinVersion)5, Name: "encode_text "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:253", DecCode: 29, Version: (MinVersion)5, Name: "copy_table "),
            new InstructionDefinition(Store: false, Branch: false, OpCode: "VAR:254", DecCode: 30, Version: (MinVersion)5, Name: "print_table "),
            new InstructionDefinition(Store: false, Branch: true, OpCode: "VAR:255", DecCode: 31, Version: (MinVersion)5, Name: "check_arg_count "),

        };
    }
}
