using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2.InstructionDefinitions
{
    internal class Instrictions_Ext
    {
        public static List<InstructionDefinition> InstructionDefinitions => new()
        {
            new InstructionDefinition(Store:true, Branch:false,OpCode:"EXT:0", DecCode:0, Version:(MinVersion)5, Name:"save "),
            new InstructionDefinition(Store:true, Branch:false,OpCode:"EXT:1", DecCode:1, Version:(MinVersion)5, Name:"restore "),
            new InstructionDefinition(Store:true, Branch:false,OpCode:"EXT:2", DecCode:2, Version:(MinVersion)5, Name:"log_shift "),
            new InstructionDefinition(Store:true, Branch:false,OpCode:"EXT:3", DecCode:3, Version:MinVersion.FiveOnly, Name:"art_shift "),
            new InstructionDefinition(Store:true, Branch:false,OpCode:"EXT:4", DecCode:4, Version:(MinVersion)5, Name:"set_font "),
            new InstructionDefinition(Store:true, Branch:false,OpCode:"", DecCode:0, Version:MinVersion.SixOnly, Name:"set_font "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:5", DecCode:5, Version:(MinVersion)6, Name:"draw_picture "),
            new InstructionDefinition(Store:false, Branch:true,OpCode:"EXT:6", DecCode:6, Version:(MinVersion)6, Name:"picture_data "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:7", DecCode:7, Version:(MinVersion)6, Name:"erase_picture "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:8", DecCode:8, Version:(MinVersion)6, Name:"set_margins "),
            new InstructionDefinition(Store:true, Branch:false,OpCode:"EXT:9", DecCode:9, Version:(MinVersion)5, Name:"save_undo "),
            new InstructionDefinition(Store:true, Branch:false,OpCode:"EXT:10", DecCode:10, Version:(MinVersion)5, Name:"restore_undo "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:11", DecCode:11, Version:(MinVersion)5, Name:"print_unicode "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:12", DecCode:12, Version:(MinVersion)5, Name:"check_unicode "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:13", DecCode:13, Version:(MinVersion)5, Name:"set_true_colour "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"", DecCode:0, Version:(MinVersion)6, Name:"set_true_colour "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"-------", DecCode:14, Version:MinVersion.NotFound, Name:""),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"-------", DecCode:15, Version:MinVersion.NotFound, Name:""),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:16", DecCode:16, Version:(MinVersion)6, Name:"move_window "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:17", DecCode:17, Version:(MinVersion)6, Name:"window_size "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:18", DecCode:18, Version:(MinVersion)6, Name:"window_style "),
            new InstructionDefinition(Store:true, Branch:false,OpCode:"EXT:19", DecCode:19, Version:(MinVersion)6, Name:"get_wind_prop "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:20", DecCode:20, Version:(MinVersion)6, Name:"scroll_window "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:21", DecCode:21, Version:(MinVersion)6, Name:"pop_stack "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:22", DecCode:22, Version:(MinVersion)6, Name:"read_mouse "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:23", DecCode:23, Version:(MinVersion)6, Name:"mouse_window "),
            new InstructionDefinition(Store:false, Branch:true,OpCode:"EXT:24", DecCode:24, Version:(MinVersion)6, Name:"push_stack "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:25", DecCode:25, Version:(MinVersion)6, Name:"put_wind_prop "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:26", DecCode:26, Version:(MinVersion)6, Name:"print_form "),
            new InstructionDefinition(Store:false, Branch:true,OpCode:"EXT:27", DecCode:27, Version:(MinVersion)6, Name:"make_menu "),
            new InstructionDefinition(Store:false, Branch:false,OpCode:"EXT:28", DecCode:28, Version:(MinVersion)6, Name:"picture_table "),
            new InstructionDefinition(Store:true, Branch:false,OpCode:"EXT:29", DecCode:29, Version:(MinVersion)6, Name:"buffer_screen "),

        };
    }
}
