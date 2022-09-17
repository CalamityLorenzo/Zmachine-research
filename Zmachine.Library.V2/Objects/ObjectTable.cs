using System;
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
        private int propertyTableLength;

        public int TotalObjects { get; set; }

        private int maximumObjects;
        private int attrbFlagsLength;
        private int paSibChLength;

        public int ObjectSize { get; }
        // Client Side are numbered from 1 to 62
        public ushort[] PropertyDefaultsTable { get; }

        public ZmObject this[ushort objectId] => this.GetObject(objectId);


        public ObjectTable(int objectTableLocation, int version, byte[] memory)
        {
            this.version = version;
            this.memory = memory;
            int propertyDefaultsSize = version > 3 ? 63 : 31;
            // size of attributes in bytes
            this.attrbFlagsLength = version > 3 ? 6 : 4;
            // size of the Parent/Sibling/Child entry
            this.paSibChLength = version > 3 ? 6 : 3;

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
            if (TotalObjects > maximumObjects) throw new ArgumentOutOfRangeException($"Total Objects count is nonsense. {TotalObjects}/{maximumObjects}");

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

            if (objectId > this.TotalObjects) throw new ArgumentOutOfRangeException($"object id is nonsense. {objectId} / {TotalObjects}");
            if (objectId < 0) throw new ArgumentOutOfRangeException($"Object id must be greater than 0. {objectId}");

            // We do a check against the maximum objects in the constructor.
            var startAttributeFlags = ObjectTreeStart + ObjectSize * (objectId - 1);
            var startPaSibCh = startAttributeFlags + attrbFlagsLength;
            // Extrude the attributes Flags in a range.
            var attributes = memory[startAttributeFlags..startPaSibCh];

            // Get the object properties tableaddress
            var propertyTableAddress = memory.Get2ByteValue(startPaSibCh + paSibChLength);
            // Start of the property header table.


            var objectPropertyTable = GetObjectPropertyTable(propertyTableAddress); // new ObjectPropertyTable(propertyHeaderLength, propertyHeaderNameBytes, objectProperties.ToArray());
            // Create an acutal object.
            var objectDetails = new ZmObject(
                ObjectId: objectId,
                StartAddress: $"{ObjectTreeStart + ObjectSize * objectId} : ${ObjectTreeStart + ObjectSize * objectId:X}",
                Attributes: new AttributesCollection(attributes),
                Parent: version > 3 ? memory.Get2ByteValue(startPaSibCh) : memory[startPaSibCh],
                Sibling: version > 3 ? memory.Get2ByteValue(startPaSibCh + 2) : memory[startPaSibCh + 1],
                Child: version > 3 ? memory.Get2ByteValue(startPaSibCh + 4) : memory[startPaSibCh + 2],
                PropertiesAddress: $"{propertyTableAddress} : {propertyTableAddress.ToString("X4")}",
                PropertyTable: objectPropertyTable
              );

            var arrybits = attributes.Reverse().ToArray().ConvertAttributes();
            return objectDetails;
        }

        // Wehllllllll fuck.
        // Having to update the object in memory.
        internal void Insert_Obj(ushort Obj_objectId, ushort Dest_objectId)
        {

            var D_StartAddress = this.GetObjectStartAddress(Dest_objectId);
            var D_PaSibCh = D_StartAddress + attrbFlagsLength;
            // Update the DChild Value = 
            ushort D_Old_Child = 0;
            if (version > 3)
            {
                D_Old_Child = this.memory.Get2ByteValue(D_PaSibCh + 4);// (ushort)(this.memory[D_PaSibCh + 4] << 8 | this.memory[D_PaSibCh + 5]);
                this.memory[D_PaSibCh + 4] = (byte)(Obj_objectId >> 8);
                this.memory[D_PaSibCh + 5] = (byte)(Obj_objectId & 0b11111111);
            }
            else
            {
                D_Old_Child = this.memory[D_PaSibCh + 2];
                this.memory[D_PaSibCh + 2] = (byte)Obj_objectId;
            }

            var O_StartAddress = this.GetObjectStartAddress(Obj_objectId);
            var O_PaSibCh = O_StartAddress + attrbFlagsLength;

            if (version > 3)
            {
                this.memory[O_PaSibCh] = (byte)(Dest_objectId >> 8);
                this.memory[O_PaSibCh + 1] = (byte)(Dest_objectId & 0b11111111);

                this.memory[O_PaSibCh + 2] = (byte)(D_Old_Child >> 8);
                this.memory[O_PaSibCh + 3] = (byte)(D_Old_Child & 0b11111111);
            }
            else
            {
                this.memory[O_PaSibCh] = (byte)Dest_objectId;
                this.memory[O_PaSibCh + 1] = (byte)D_Old_Child;
            }

        }

        internal void Set_Attribute(ushort objectId, ushort attrFlag)
        {
            var objStartAddress = this.GetObjectStartAddress(objectId);
            for (var byteCounter = 0; byteCounter < attrbFlagsLength; byteCounter++)
            {
                if (byteCounter * 8 <= attrFlag && (byteCounter * 8) + 8 >= attrFlag)
                {
                    var localFlagId = attrFlag - (byteCounter * 8);

                    var byteToChange = memory[objStartAddress + byteCounter];
                    var positionToChange = 7 - localFlagId;
                    byteToChange ^= (byte)((-1 ^ byteToChange) & (1 << positionToChange));
                    memory[objStartAddress + byteCounter] = byteToChange;
                    break;
                }

            }
        }
        internal void SetProperty(ushort objectId, ushort property, ushort value)
        {
            var objectAddress = GetObjectStartAddress(objectId);
            var propertiesAddress = objectAddress + attrbFlagsLength + paSibChLength;



        }
        internal ushort GetProperty(ushort objectId, ushort property)
        {
            var propertyTableAddress = memory.Get2ByteValue(this.GetObjectStartAddress(objectId) + paSibChLength + attrbFlagsLength);

            var objectPropertyTable = GetObjectPropertyTable(propertyTableAddress);


            var obj = this.GetObject(objectId).PropertyTable;

            var getProperty = objectPropertyTable.properties.FirstOrDefault(a => a.propertyNumber == property);
            if (getProperty != null)
                return getProperty.PropertyData.GetUShort();
            else
            {
                return this.PropertyDefaultsTable[property];
            }

        }

        internal ushort GetSibling(ushort objectId)
        {
            var obj = GetObject(objectId);
            return obj.Sibling;
        }

        private int GetObjectStartAddress(ushort objectId)
        {
            return ObjectTreeStart + ObjectSize * (objectId - 1);
        }

        private ObjectPropertyTable GetObjectPropertyTable(ushort propertyTableAddress)
        {
            // Get the object properties tableaddress
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
                    propertyLength = (memory[propertyStart += 1] & 0b111111);
                    //propertyHeaderSize = memory[propertyStart] << 8 | memory[propertyStart += 1];
                }
                // 12.4.2.2
                else if ((propertySizeByte & 0b10000000) == 0b00000000 && version > 3)
                {
                    propertyNumber = propertySizeByte & 0b111111;
                    propertyLength = (propertySizeByte >> 6) + 1;

                }
                // 12.4.1
                else if (version < 4)
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
            return new ObjectPropertyTable(propertyHeaderLength, propertyHeaderNameBytes, objectProperties.ToArray());
        }

        internal ZmObject[] GetChildren(ushort parentIdx)
        {
            // We don't need to hydrate every object, 
            // They are all fixed width so we are literally readting a byte or a word from memory
            var startObjectAddress = this.GetObjectStartAddress(0);
            // Size in bytes
            var objectTotalSize = this.version < 4 ? 9 : 14;
            var parentAddress = this.attrbFlagsLength;
            List<ZmObject> results = new();
            // iterate through allllll the objects to find the children.
            for (ushort x = 0; x < this.TotalObjects; ++x)
            {
                var parentId = version > 4 ? (ushort)(this.memory.Get2ByteValue(startObjectAddress + parentAddress))
                        : this.memory[startObjectAddress + parentAddress];
                if (parentId == parentIdx)
                {
                    results.Add(GetObject(x));
                }

                startObjectAddress += objectTotalSize;
            }

            return results.ToArray();
        }
        internal ZmObject[] GetSiblings(ushort objectId)
        {
            // Gets' us to the parent BYTE
            var parentByte = this.GetObjectStartAddress(objectId) + attrbFlagsLength;

            var parentId = version > 4 ? (ushort)this.memory.Get2ByteValue(parentByte)
                        : this.memory[parentByte];
            return this.GetChildren(parentId).Where(p=>p.ObjectId!=objectId).ToArray();
        }

        internal ZmObject[] GetMentionedAsSibling(ushort siblingId)
        {
            // Gets' us to the parent BYTE
            var results = new List<ZmObject>();
            for (ushort x = 0; x < this.TotalObjects; ++x)
            {
                var objectId = x;
                var siblingByte = this.GetObjectStartAddress(objectId) + attrbFlagsLength + (version > 3 ? 2 : 1);


                var objectSiblingId = version > 4 ? (ushort)this.memory.Get2ByteValue(siblingByte)
                            : this.memory[siblingByte];
                if (objectSiblingId == siblingId)
                    results.Add(GetObject(objectId));

            }

            return results.ToArray();
        }


        internal ZmObject GetParent(ushort objectId)
        {
            var parentByte = this.GetObjectStartAddress(objectId) + attrbFlagsLength;

            var parentId = version > 4 ? (ushort)this.memory.Get2ByteValue(parentByte)
                        : this.memory[parentByte];
            return this.GetObject(parentId);
        }
    }
}
