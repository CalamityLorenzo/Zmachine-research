using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2.Machines
{
    public partial class Zmachine_v2
    {
        public void DumpDictionary()
        {
            //var dictionaryStart = Memory[this.HeaderDetails.Dictionary];

            var abbreviations = new ZmAbbreviations(this.HeaderDetails.AbbreviationsTable, this.Memory);

            var zDict = new ZmDictionaryTable(this.HeaderDetails.Dictionary, Memory);

            Console.WriteLine("=== Entries ===");
            foreach (var word in zDict.Entries)
            {
                Console.WriteLine(word);
            }

            Console.WriteLine("=== Seperators ===");
            foreach (var seperator in zDict.WordSeparators)
            {
                Console.WriteLine(seperator);
            }

        }

        public void DumpObjects()
        {
            var abbreviations = new ZmAbbreviations(this.HeaderDetails.AbbreviationsTable, this.Memory);

            var zObjs = new ZmObjects(this.HeaderDetails.ObjectTable, this.HeaderDetails.Version, Memory, abbreviations);
            for (int prop = 0; prop < zObjs.PropertyDefaults.Length; prop++)
            {
                Console.WriteLine($"{prop}\t:\t{zObjs.PropertyDefaults[prop]}");
            }
            Console.WriteLine($"Total number of objects\t:\t{zObjs.TotalObjects}");

            Console.WriteLine(zObjs.GetObject(0));
            Console.WriteLine(zObjs.GetObject(6));
            Console.WriteLine(zObjs.GetObject(4));
            Console.WriteLine(zObjs.GetObject(15));

        }
        public void DumpHeader()
        {
            Console.WriteLine("==== HEADER ====");
            Console.WriteLine($"Version:\t\t\t $00\t:\t{Memory[0]}");
            Console.WriteLine($"Tandy Bit And Flags:\t\t $01\t:\t{Memory[1]}");
            Console.WriteLine($"Flags2:\t\t\t\t $10\t:\t{Memory[16]}");
            Console.WriteLine($"Release Number:\t\t\t $02\t:\t{Memory[2]}\t:\t{Memory[2]}");
            Console.WriteLine($"High Memory Start:\t\t $04\t:\t{Memory.Get2ByteValue(4)}\t:\t{Memory.Get2ByteValueHex(4)}");
            Console.WriteLine($"\"main\" | First Instr Execute:\t $06\t:\t{Memory.Get2ByteValue(6)}\t:\t{Memory.Get2ByteValueHex(6)}");
            Console.WriteLine($"Dictionary table:\t\t $08\t:\t{Memory.Get2ByteValue(8)}\t:\t{Memory.Get2ByteValueHex(8)}");
            Console.WriteLine($"Abbreviations table:\t\t $18\t:\t{Memory.Get2ByteValue(24)}\t:\t{Memory.Get2ByteValueHex(24)}");
            Console.WriteLine($"Object table:\t\t\t $0A\t:\t{Memory.Get2ByteValue(10)}\t:\t{Memory.Get2ByteValueHex(10)}");
            Console.WriteLine($"Global variables:\t\t $0C\t:\t{Memory.Get2ByteValue(12)}\t:\t{Memory.Get2ByteValueHex(12)}");
            Console.WriteLine($"Static Memory start:\t\t $0E\t:\t{Memory.Get2ByteValue(0xE)}\t:\t{Memory.Get2ByteValueHex(0xE)}");
            Console.WriteLine($"Length of file (divide by 4):\t $1A\t:\t{Memory.Get2ByteValue(26)}\t:\t{Memory.Get2ByteValueHex(26)}");
            Console.WriteLine($"Checksum:\t\t\t $1C\t:\t{Memory.Get2ByteValue(28)}\t:\t{Memory.Get2ByteValueHex(28)}");
            Console.WriteLine($"Interpreter number:\t\t $1D\t:\t{Memory[30]}");
            Console.WriteLine($"Interpreter version:\t\t $1E\t:\t{Memory[31]}");

            Console.WriteLine($"Date Compiled:\t\t\t $12\t{(char)Memory[0x12]}{(char)Memory[0x13]}-{(char)Memory[20]}{(char)Memory[21]}-{(char)Memory[22]}{(char)Memory[23]}");

            Console.WriteLine($"Static strings offset (div by 8):\t\t $2A {Memory[0x2A]}\t:\t{Memory[0x2A]:X}");

        }
    }
}
