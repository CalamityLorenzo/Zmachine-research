﻿using System.Collections;
using System.Net.Mail;
using System.Security;
using Zmachine.Library.V2.Utilities;

namespace Zmachine.Library.V2.Objects
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

        private int maximumObjects;

        public int ObjectSize { get; }
        // Client Side are numbered from 1 to 62
        public ushort[] PropertyDefaultsTable { get; }


        public ObjectTable(int objectTableLocation, int version, byte[] memory)
        {
            this.version = version;
            this.memory = memory;
            int propertyDefaultsSize = version > 3 ? 63 : 31;

            this.maximumObjects = version > 3 ? 65535 : 255;
            ObjectSize = version > 3 ? 14 : 9;
            int objectEntryByteSize = (version > 3 ? 12 : 7); // Width of the table entry minus the property byte

            // Byte address of a list of default properties.
            PropertyDefaultsTable = new ushort[propertyDefaultsSize];
            int PropertyHeaderIdx = objectTableLocation;
            // Each Property entry is word sized
            for (var x = 0; x < propertyDefaultsSize; x++)
            {
                ushort newEntry = memory.Get2ByteValue(PropertyHeaderIdx);
                PropertyDefaultsTable[x] = newEntry;
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
                    var proprtyTableLocation = objectAddress + objectEntryByteSize;
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
            if(TotalObjects> maximumObjects) throw new ArgumentOutOfRangeException($"Total Objects count is nonsense. {TotalObjects}/{maximumObjects}");

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

            // 12.3.1
            // v1->3 object layout
            //                  4          (3)              7      (2)     9
            // 32 atribute flags           PArent Sibling Child    properties
            // 
            // 12.3.2
            // v4 object layout
            //                  6           (6)              12      (2)       14
            // the 48 attribute flags       Parent Sibling Child     propertyTable
            // ---48 bits in 6 bytes--- ---3 words, i.e. 6 bytes---- ---2 bytes--


            // Size of the flags in bytes
            var attrbFlagsLength = version > 3 ? 6 : 4;
            // size of the Parent/Sibling/Child entry
            var paSibChLength = version > 3 ? 6 : 3;
            // We do a check against the maximum objects in the constructor.
            if (objectId > this.TotalObjects) throw new ArgumentOutOfRangeException($"object id is nonsense. {objectId} / {TotalObjects}");
            var startAttributeFlags = ObjectTreeStart + ObjectSize * (objectId-1);
            var startPaSibCh = startAttributeFlags + attrbFlagsLength;
            // Extrude the attributes Flags in a range.
            var attributes = memory[startAttributeFlags..startPaSibCh]; 

            // Get the object properties tableaddress
            var propertyTableAddress = memory.Get2ByteValue(startPaSibCh + paSibChLength);
            // Start of the property header table.
            var propertyHeaderLength = memory[propertyTableAddress];
            var headerNameStart = propertyTableAddress + 1;
            var headerNameEnd = headerNameStart + (propertyHeaderLength * 2);
            // Short name of property
            var propertyHeaderNameBytes = memory[headerNameStart..headerNameEnd];

            // Object properties are stored in descending numnerical do-hickeys
            //12.4 Property tables
            List<ObjectProperty> objectProperties = new();
            var propertyStart = headerNameEnd;
            var propertyNumber = 0;
            var propertyLength = 0;
            // This has a different layout depending on version.
            var propertySizeByte = memory[propertyStart];
            // The 
            while (propertySizeByte != 0)
            {

                // note we have to ensure the counter (propertyStart) is in the correct position.
                // 12.4.2.1 ( we have a 2 byte size entry) 
                if ((propertySizeByte & 0b10000000) == 0b10000000 && version > 3)
                {
                    //bits 0 to 5 contain the property number
                    propertyNumber = (memory[propertyStart] & 0b111111);
                    propertyLength = (memory[propertyStart+=1] & 0b111111);
                    //propertyHeaderSize = memory[propertyStart] << 8 | memory[propertyStart += 1];
                }
                // 12.4.2.2
                else if ((propertySizeByte & 0b10000000) == 0b00000000 &&  version > 3)
                {
                    propertyNumber = propertySizeByte & 0b111111;
                    propertyLength = (propertySizeByte >> 6) + 1;

                }
                // 12.4.1
                else if (version<4)
                {
                    propertyNumber = propertySizeByte & 0b11111;
                    propertyLength = (propertySizeByte >> 5) + 1;
                }

                var propertyEntryStart = propertyStart += 1;
                var propertyEntryEnd = propertyStart += propertyLength;
                var propertyData = memory[propertyEntryStart..propertyEntryEnd];
                objectProperties.Add(new(propertyLength, propertyNumber, propertyData));
                // Beginning of the next property record
                propertySizeByte = memory[propertyStart];
            }

            var objectPropertyTable = new ObjectPropertyTable(propertyHeaderLength, propertyHeaderNameBytes, objectProperties.ToArray());
            // Create an acutal object.
            var objectDetails = new ZmObject(
                ObjectId: objectId,
                StartAddress: $"{ObjectTreeStart + ObjectSize * objectId} : ${ObjectTreeStart + ObjectSize * objectId:X}",
                Attributes: new AttributesCollection(attributes),
                Parent: version > 3 ? memory.Get2ByteValue(startPaSibCh) : memory[startPaSibCh],
                Sibling: version > 3 ? memory.Get2ByteValue(startPaSibCh + 2) : memory[startPaSibCh+1],
                Child: version > 3 ? memory.Get2ByteValue(startPaSibCh + 4) : memory[startPaSibCh + 2],
                PropertiesAddress: $"{propertyTableAddress} : {propertyTableAddress.ToString("X4")}",
                PropertyTable: objectPropertyTable
              );

            var arrybits = attributes.Reverse().ToArray().ConvertAttributes();
            return objectDetails;
        }
    }
}