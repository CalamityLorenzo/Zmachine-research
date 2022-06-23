namespace Zmachine.Workings
{
    internal struct PropertyValue
    {
        public PropertyValue() { }
        internal int Id { get; set; } = 0;
        internal byte[] Value { get; } = new byte[4];
    }
}
