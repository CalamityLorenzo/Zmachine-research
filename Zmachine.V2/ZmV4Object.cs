using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2
{
    internal record ZmV4Object(
         string StartAddress ,
         BitArray Attributes ,
         UInt16 Parent ,
         UInt16 Sibling ,
         UInt16 Child ,
         string PropertiesAddress ,
         string PropertiesName,
         object? Properties
        );
    
}
