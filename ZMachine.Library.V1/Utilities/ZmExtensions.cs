namespace ZMachine.Library.V1.Utilities
{
    public static class ZmExtensions
    {
        public static byte[] GetValidZSCIIChars(this string @this, int version) => TextProcessor.ValidZsciiChars(@this, version);
    }
}
