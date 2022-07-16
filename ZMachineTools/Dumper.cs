using ZMachine.Library.V1;
using ZMachine.Library.V1.Utilities;

namespace ZMachineTools
{
    public class Dumper
    {
        private MemoryStream StoryData;
        private byte[] Memory;
        private StoryHeader StoryHeader;
        private FeaturesVersion FeaturesVersion;
        private DictionaryTable DictionaryTable;

        public AbbreviationsTable AbbreviationTable { get; private set; }

        public Dumper(Stream storyData)
        {
            // Here's an assumption...
            storyData.Position = 0;
            StoryData = new MemoryStream();
            storyData.CopyTo(StoryData);
            StoryData.Position = 0;

            Memory = new byte[StoryData.Length];
            // Nom, nom nom game data.
            StoryData.Read(Memory, 0, Memory.Length);
            this.StoryHeader = StoryHeader.CreateHeader(Memory);
            this.FeaturesVersion = LibraryUtilities.GetFeatureVersion(StoryHeader.Version);
        }

        public void Dictionary()
        {
            this.DictionaryTable = new DictionaryTable(this.StoryHeader.DictionaryTable, Memory);

            for (var x = 0; x < this.DictionaryTable.Length; ++x)
            {
                // All the bytes
                var dictionaryBytes = this.DictionaryTable[x];
                // get the text
            }
        }

        public void Abbreviations()
        {
            this.AbbreviationTable = new AbbreviationsTable(this.StoryHeader.AbbreviationTable, Memory, this.StoryHeader.Version);

            for (var x = 0; x < AbbreviationTable.Length; ++x)
            {
                var abbreviationsBytes = this.AbbreviationTable[x];
                // Do the thing with the text;
            }
        }

        public void Objects()
        {

        }

        public void Header()
        {

        }
    }
}