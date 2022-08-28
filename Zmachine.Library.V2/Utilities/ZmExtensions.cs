using Zmachine.Library.V2.Instructions;

namespace Zmachine.Library.V2.Utilities
{
    public static class ZmExtensions
    {
        public static byte[] GetValidZSCIIChars(this string @this, int version) => TextProcessor.ValidZsciiChars(@this, version);

    }
}
