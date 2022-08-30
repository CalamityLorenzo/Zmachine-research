using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Zmachine.Library.V2
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ReturnAddress"></param>
    /// <param name="StartAdress">PRogram Counter at initialisation</param>
    /// <param name="Locals">Local variables for this routine</param>
    /// <param name="StoreResult">Are we storing/discarding the result?</param>
    /// <param name="ResultStoreLocation">Follows the same rules as operandType eg 00 = stack 01 = local etc...</param>
    public record ActivationRecord(int ReturnAddress, int StartAdress, ushort[] Locals, bool StoreResult, ushort? StoreAddress)
    {
        public Stack<ushort> LocalStack { get; init; } = new Stack<ushort>();
        public string StartAddressHex => StartAdress.ToString("X4");
        public string ReturnAddressHex => ReturnAddress.ToString("X4");
    }
}