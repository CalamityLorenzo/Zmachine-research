using System.Collections;
using System.Collections.Generic;

namespace Zmachine.Library.V2
{
    public record ActivationRecord(int returnAddress, int startAdress, ushort[] locals, Stack<ushort> localStack = default ) { };
}