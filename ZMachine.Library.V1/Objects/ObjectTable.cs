using System.Collections;
using ZMachine.Library.V1.Utilities;

namespace ZMachine.Library.V1.Objects
{
    public class ObjectTable
    {
        // v4 object layout       6                           12             14
        // the 48 attribute flags       parent sibling child     propertyTable
        // ---48 bits in 6 bytes--- ---3 words, i.e. 6 bytes---- ---2 bytes--

        // v1->3 object layout
        //                  4                       7             9
        // 32 atribute flags    parent sibling child    properties
        // 

        private int ObjectTreeStart;
        private readonly int version;
        private byte[] memory;

        public int TotalObjects { get; set; }
        public int ObjectSize { get; }
        // Client Side are numbered from 1 to 62
        public ushort[] PropertyDefaults { get; }


        public ObjectTable(int objectTableLocation, int version, byte[] memory)
        {
            this.version = version;
            this.memory = memory;
            int defaultSize = version > 3 ? 63 : 31;

            int maximumObjects = version > 3 ? 65535 : 255;
            ObjectSize = version > 3 ? 14 : 9;

            // Byte address of a list of default properties.
            PropertyDefaults = new ushort[defaultSize];
            int PropertyHeaderIdx = objectTableLocation;
            for (var x = 0; x < defaultSize; x++)
            {
                ushort newEntry = memory.Get2ByteValue(PropertyHeaderIdx);
                PropertyDefaults[x] = newEntry;
                PropertyHeaderIdx += 2;
            }

            // We should be at the start location for the actual objects table.
            ObjectTreeStart = PropertyHeaderIdx;

            // Now populate the objects
            // 1. Find the first property table address
            // 2. Calculate the distance from the start location -> First Property Table Address
            // 3. Divide by 9 (v1-3) / 14 (v4+)
            // 4. Total number of objects!
            int propertyTableStartAddress = 0;
            for (int x = 0; x < maximumObjects; ++x)
            {
                var objectAddress = ObjectTreeStart + ObjectSize * x;
                if (x == 0)
                {
                    var proprtyTableLocation = objectAddress + (version > 3 ? 12 : 8);
                    propertyTableStartAddress = memory.Get2ByteValue(proprtyTableLocation);
                }
                else
                {
                    if (objectAddress > propertyTableStartAddress)
                        break;
                }
            }
            var totalObjectTableSize = propertyTableStartAddress - ObjectTreeStart;
            if (totalObjectTableSize == 0) throw new ArgumentOutOfRangeException("Cannnot calculate object table");

            TotalObjects = totalObjectTableSize / ObjectSize;

        }

        /// <summary>
        /// We build the object back to front.
        /// calculate all the relevnant boundaries, and then add propties, propertyheader, finally Objevt.
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ZmObject GetObject(ushort objectId)
        {

            var attrbFlagsLength = version > 3 ? 6 : 4;

            if (objectId > TotalObjects) throw new ArgumentOutOfRangeException("object id is nonsense.");
            var startAttributes = ObjectTreeStart + ObjectSize * (objectId-1);
            var startPaSibCh = startAttributes + attrbFlagsLength;
            var attributes = memory[startAttributes..startPaSibCh]; // First 6 bytes are the attributes (48/32 bits)


            // Get the object properties list.
            var propertyTableAddress = memory.Get2ByteValue(startPaSibCh + attrbFlagsLength);
            var propertyHeaderLength = memory[propertyTableAddress];
            var headerNameStart = propertyTableAddress + 1;
            var headerNameEnd = headerNameStart + (propertyHeaderLength * 2);
            var propertyHeaderNameBytes = memory[headerNameStart..headerNameEnd];

            // Object properties are stored in descending numnerical do-hickeys
            List<ObjectProperty> objectProperties = new();

            // if top bit ==1 two byte size.                
            var propertyHeaderSize = 0;
            var propertyStart = headerNameEnd + 1;
            var propertyNumber = 0;
            var propertyLength = 0;

            var propertySizeByte = memory[propertyStart];
            while (propertySizeByte != 0)
            {

                // note we have to ensure the counter (propertyStart) is in the correct position.
                if ((propertySizeByte & 0b10000000) == 0b10000000 && version > 3)
                {
                    //bits 0 to 5 contain the property number
                    propertyNumber = (memory[propertyStart] & 0b11111);
                    propertyLength = (memory[propertyStart += 1] & 0b11111);
                    propertyHeaderSize = memory[propertyStart] << 8 | memory[propertyStart += 1];
                }
                else
                {
                    propertyNumber = propertySizeByte & 0b1111;
                    propertyLength = (propertySizeByte >> 4) + 1;
                }

                var propertyEntryStart = propertyStart += 1;
                var propertyEntryEnd = propertyStart += (propertyLength * 2);
                var propertyData = memory[propertyEntryStart..propertyEntryEnd];
                objectProperties.Add(new(propertyLength, propertyNumber, propertyData));
                // Beginning of the next property record
                propertySizeByte = memory[propertyStart += 1];
            }

            var objectPropertyTable = new ObjectPropertyTable(propertyHeaderLength, propertyHeaderNameBytes, objectProperties.ToArray());
            var objectDetails = new ZmObject(
                ObjectId: objectId,
                StartAddress: $"{ObjectTreeStart + ObjectSize * objectId} : ${ObjectTreeStart + ObjectSize * objectId:X}",
                Attributes: new BitArray(attributes),
                Parent: memory.Get2ByteValue(startPaSibCh),
                Sibling: memory.Get2ByteValue(startPaSibCh + 2),
                Child: memory.Get2ByteValue(startPaSibCh + 4),
                PropertiesAddress: $"{memory.Get2ByteValue(startPaSibCh + 6)} : {memory.Get2ByteValueHex(startPaSibCh + 6)}",
                PropertyTable: objectPropertyTable
              );
            return objectDetails;
        }

        //private string PropertyHeaderName(ushort propertyAddress)
        //{
        //    var HeaderLength = memory[propertyAddress]; // This value is 2 bytes words ie 3 = 6.
        //    var start = propertyAddress + 1;
        //    var end = propertyAddress + 1 + HeaderLength * 2;
        //    // the full range of bytes for the name
        //    byte[] NameBytes = memory[start..end];
        //    int Address = 0;
        //    byte[] rawZChars = ZmTextDecoder.GetZChars(NameBytes, ref Address);
        //    // Find if we have an abbreviation in our midst
        //    var complete = false;
        //    var ctr = 0;
        //    List<byte> completeMessage = new List<byte>();
        //    while (!complete)
        //    {
        //        if (rawZChars[ctr] == 2)
        //        {
        //            var abbreviationIdx = rawZChars[ctr + 1];
        //            var abbreviationAddress = abbreviations[abbreviationIdx];

        //            var abbreviationChars = ZmTextDecoder.GetZChars(memory, ref abbreviationAddress);
        //            completeMessage.AddRange(abbreviationChars);
        //            ctr += 2;
        //        }
        //        else
        //        {
        //            completeMessage.Add(rawZChars[ctr]);
        //            ctr += 1;
        //        }
        //        if (ctr > rawZChars.Length - 1)
        //            complete = true;
        //    }


        //    return ZmTextDecoder.DecodeZChars(completeMessage.ToArray());
        //}

    }
}
