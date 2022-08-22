using System.Collections;
using System.Collections.Generic;

namespace ZMachine.Library.V1
{
    public record ActivationRecord(int returnAdrress, int startAdress, ushort[] locals, Stack<ushort> localStack = default ) { };
}