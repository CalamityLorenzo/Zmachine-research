using System.Collections;

namespace ZMachine.Library.V1.Objects
{
    public record ZmObject(
         ushort ObjectId,
         string StartAddress,
         BitArray Attributes,
         ushort Parent,
         ushort Sibling,
         ushort Child,
         string PropertiesAddress,
         ObjectPropertyTable PropertyTable
        );
    public record ObjectPropertyTable(ushort length, byte[] shortNameBytres, ObjectProperty[] Properties );

    public record ObjectProperty(int Size, int propertyNumber, byte[] PropertyData);
}
