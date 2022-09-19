using System.Collections;
using System.Reflection.Metadata;
using System.Text;
using Zmachine.Library.V2.Utilities;

namespace Zmachine.Library.V2.Objects
{
    public record ZmObject(
         ushort ObjectId,
         string StartAddress,
         // Bit array stores in LSB->MSB so here give a byte[4] byte 0 = 0->16 and not 33->64
         AttributesCollection Attributes,
         ushort Parent,
         ushort Sibling,
         ushort Child,
         string PropertiesAddress,
         ObjectPropertyTable PropertyTable
        )
    {
        protected virtual bool PrintMembers(StringBuilder sb)
        {
            sb.AppendLine($"ObjectId = {ObjectId}, PropertiesAddress ={PropertiesAddress}, StartAddress={StartAddress}");
            sb.Append("Attributes=");
            sb.Append(String.Join(", ", Attributes.Select(a=>$"[{a}]")));
            sb.AppendLine();
            sb.AppendLine($"Parent = {Parent}, Sibling={Sibling}, Child={Child}");
            sb.AppendLine($" ObjectPropertyTable = {PropertyTable}");
            sb.AppendLine("");
            return true;
        }
    }
    
    public record ObjectPropertyTable(ushort nameLength, byte[] shortNameBytes, ObjectProperty[] properties );

    public record ObjectProperty(int Size, int propertyNumber, byte[] PropertyData, ushort memoryLocation)
    {
        protected virtual bool PrintMembers(StringBuilder sb)
        {
            sb.AppendLine($"Size = {Size}, PropertyNumber={propertyNumber} Memory={memoryLocation}");
            sb.Append("[");
            sb.Append(String.Join(",", PropertyData.Select(a => a.ToString("X2"))));
            sb.AppendLine("]");

            return true;
        }
    }

    public class AttributesCollection : List<byte>
    {
        public AttributesCollection(byte[] attributes)
        {
            this.AddRange(attributes.Reverse().ConvertAttributes());
        }
    }
}
