namespace Zmachine.Workings
{

    public enum AlphabetType
    {
        Lower = 0,
        Upper = 4,
        Punctuation = 5
    };

    public struct EncodedChar
    {
        public AlphabetType alphabet { get; set; }
        public byte CharCode { get; set; }
        public Char Char { get; set; }

        public override string ToString()
        {
            return $"{alphabet} {Char} {CharCode}";
        }
    }
}
