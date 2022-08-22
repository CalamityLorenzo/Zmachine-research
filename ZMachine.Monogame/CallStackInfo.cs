using System.Collections;
using System.Collections.Generic;

namespace ZMachine.Monogame
{
    internal record ActivationRecord(int returnAdrress, int startAdress, byte[] locals, Stack<ushort> localStack = default ) { };
}