using System.Collections;
using System.Collections.Generic;

namespace Zmachine.Library.V2
{
    public record ActivationRecord
    {
        public readonly Stack<ushort> localStack;

        public ActivationRecord(int returnAddress, int startAdress, ushort[] locals, bool storeResult)
        {
            localStack = new Stack<ushort>();
            ReturnAddress = returnAddress;
            StartAdress = startAdress;
            Locals = locals;
            StoreResult = storeResult;
        }

        public int ReturnAddress { get; }
        public int StartAdress { get; }
        public ushort[] Locals { get; }
        public bool StoreResult { get; init; }
    }
}