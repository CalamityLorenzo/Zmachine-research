namespace ZMachine.Library.V1
{
    [Flags]
    /// Instructions, methods, properties etc
    /// can be applied in varous differing ways.
    /// Good news! We know what/where they this is to discriminate.
    /// Note: there is an extension method to convert this to a actual version number too.
    public enum FeaturesVersion
    {
        None = 0,
        One = 1,
        Two = 2,
        Three = 4,
        Four = 8,
        Five = 16,
        Six = 32,
        Seven = 64,
        Eight = 128,
        UpToFour = One | Two | Three | Four,
        FiveAndUp = Five | Six | Seven | Eight,
        SixAndUp = Six | Seven | Eight,
        All = One | Two | Three | Four | Five | Six | Seven | Eight
    }
}
