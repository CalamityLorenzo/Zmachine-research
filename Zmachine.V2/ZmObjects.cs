using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmachine.V2
{
    internal class ZmObjects
    {
        // v4 object layout
        // the 48 attribute flags       parent sibling child     properties
        // ---48 bits in 6 bytes--- ---3 words, i.e. 6 bytes---- ---2 bytes--

        internal struct zObject
        {
            BitArray Flags { get; }
            ushort Parent { get; }
            ushort Sibling { get; }
            ushort Child { get; }

            ushort Properties { get; set; }
        }

        private int objectTableStartLocation;
        private readonly int version;
        private byte[] memory;
        private readonly ZmAbbreviations abbreviations;

        public int TotalObjects { get; set; }
        public int ObjectSize { get; }
        public ushort[] PropertyDefaults { get; }


        public ZmObjects(int objectTableLocation, int version, byte[] memory, ZmAbbreviations abbreviations)
        {
            this.version = version;
            this.memory = memory;
            this.abbreviations = abbreviations;
            int defaultSize = (version > 3 ? 63 : 31);

            int maximumObjects = version > 3 ? 65535 : 255;
            this.ObjectSize = version > 3 ? 14 : 9;

            // Byte address of a list of properties.
            this.PropertyDefaults = new ushort[defaultSize];
            int PropertyHeaderIdx = objectTableLocation;
            for (var x = 0; x < defaultSize; x++)
            {
                ushort newEntry = memory.Get2ByteValue(PropertyHeaderIdx);
                PropertyDefaults[x] = newEntry;
                PropertyHeaderIdx += 2;
            }

            // We should be at the start location for the actual objects table.
            this.objectTableStartLocation = PropertyHeaderIdx;

            // Now populate the objects
            // 1. Find the first property table address
            // 2. Calculate the distance from the start location -> First Property Table Address
            // 3. Divide by 9 (v1-3) / 14 (v4+)
            // 4. Total number of objects!
            int propertyTableStartAddress = 0;
            for (int x = 0; x < maximumObjects; ++x)
            {
                var objectStartLocation = memory[objectTableStartLocation + (ObjectSize * x)];
                propertyTableStartAddress = memory.Get2ByteValue(objectStartLocation);
                if (propertyTableStartAddress != 0)
                {
                    break;
                }
            }
            var totalObjectTableSize = propertyTableStartAddress - objectTableStartLocation;
            if (totalObjectTableSize == 0) throw new ArgumentOutOfRangeException("Cannnot calculate object table");

            this.TotalObjects = totalObjectTableSize / 14;

        }

        public ZmV4Object GetObject(ushort objectId)
        {

            if (objectId > this.TotalObjects) throw new ArgumentOutOfRangeException("object id is nonsense.");

            var results = new byte[this.TotalObjects];
            var startAttributes = objectTableStartLocation + (ObjectSize * objectId);
            var endAttributes = startAttributes + 6;
            var attributes = memory[startAttributes..endAttributes]; // First 6 bytes are the (48 bits)

            //var NameBytes = PropertyHeaderName(memory.Get2ByteValue(endAttributes + 6));

            var objectDetails = new ZmV4Object(
                StartAddress: $"{(objectTableStartLocation + (ObjectSize * objectId))} : ${(objectTableStartLocation + (ObjectSize * objectId)):X}",
                Attributes: new BitArray(attributes),
                Parent: memory.Get2ByteValue(endAttributes),
                Sibling: memory.Get2ByteValue(endAttributes + 2),
                Child: memory.Get2ByteValue(endAttributes + 4),
                PropertiesAddress: $"{memory.Get2ByteValue(endAttributes + 6)} : {memory.Get2ByteValueHex(endAttributes + 6)}",
                PropertiesName: new ZmPropertyHeader(memory.Get2ByteValue(endAttributes + 6), memory).Name,
                Properties:null
            );



            return objectDetails;
        }

        private string PropertyHeaderName(ushort propertyAddress)
        {
            var HeaderLength = memory[propertyAddress]; // This value is 2 bytes words ie 3 = 6.
            var start = propertyAddress + 1;
            var end = propertyAddress + 1 + HeaderLength * 2;
            // the full range of bytes for the name
            byte[] NameBytes = memory[start..end];

            byte[] rawZChars = ZmTextDecoder.GetZChars(NameBytes);
            // Find if we have an abbreviation in our midst
            var complete = false;
            var ctr = 0;
            List<byte> completeMessage = new List<byte>();
            while (!complete)
            {
                if (rawZChars[ctr] == 2)
                {
                    var abbreviationIdx = rawZChars[ctr + 1];
                    var abbreviationAddress = abbreviations[abbreviationIdx];

                    var abbreviationChars = ZmTextDecoder.GetZChars(memory, abbreviationAddress);
                    completeMessage.AddRange(abbreviationChars);
                    ctr += 2;
                }
                else
                {
                    completeMessage.Add(rawZChars[ctr]);
                    ctr += 1;
                }
                if (ctr > rawZChars.Length - 1)
                    complete = true;
            }


            return ZmTextDecoder.DecodeZChars(completeMessage.ToArray());
        }

    }
}
