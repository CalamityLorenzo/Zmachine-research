namespace Zmachine.Workings
{
    internal struct PropertyValue
    {
        internal int Id { get; set; }
        internal byte[] Value { get; } = new byte[4];
    }
}
