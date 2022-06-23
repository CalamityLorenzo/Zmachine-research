namespace Zmachine.Workings
{
    internal record ZmObject
    {
        internal ZmObject(int parent, int sibling, int child, int propertyAddress, string description, List<int> attributes, Dictionary<int, byte[]> properties, string title)
        {
            Parent = parent;
            Sibling = sibling;
            Child = child;
            PropertyAddress = propertyAddress;
            Description = description;
            Attributes = attributes;
            Properties = properties;
            Title = title;
        }

        internal int Parent { get; set; }
        internal int Sibling { get; set; }
        internal int Child { get; set; }
        internal int PropertyAddress{get;set;}
        internal string Description { get; set; }
        internal List<int> Attributes { get; set; }
        internal Dictionary<int, byte[]> Properties { get; set; }
        internal string Title { get; set; }
    }
}