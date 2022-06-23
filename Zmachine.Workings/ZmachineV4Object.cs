using System.Collections;

namespace Zmachine.Workings
{
    internal record ZmachineV4Object
    {
        public string StartAddress { get; set; }
        public BitArray Attributes { get; init; }
        public UInt16 Parent { get; init; }
        public UInt16 Sibling { get; init; }
        public UInt16 Child { get; init; }
        public string PropertiesAddress { get; init; }
        public object Properties { get; init; }
        public string PropertiesName { get; internal set; }
    }
}
