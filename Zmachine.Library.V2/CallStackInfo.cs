using System.Collections;
using System.Collections.Generic;

namespace Zmachine.Library.V2
{
    public record ActivationRecord(int returnAdrress, int startAdress, ushort[] locals, Stack<ushort> localStack = default ) { };
}