using ZMachineTools;

namespace Zmachine.Tests
{
    
    public class BasicTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test(Description ="Load v5 story file display header")]
        public void HeaderDetails()
        {
            var filename = "Curses\\curses.z5";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;
            var zmachineTools = new Tools(fileStream);
            zmachineTools.Header();
            Assert.Pass("Passed!");
        }

        [Test(Description ="Display all the abbreviations")]
        public void AbbreviationTable()
        {
            var filename = "Curses\\curses.z5";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;
            var zmachineTools = new Tools(fileStream);
            zmachineTools.Abbreviations();
            Assert.Pass("Passed!");
        }

        [Test(Description = "Display all the Dictionary Entries")]
        public void DictionaryTable()
        {
            var filename = "Curses\\curses.z5";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;
            var zmachineTools = new Tools(fileStream);
            zmachineTools.Dictionary();
            Assert.Pass("Passed!");
        }

        [Test(Description = "String tests")]
        public void DecodeText()
        {
            var filename = "Curses\\curses.z5";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0x22AC;

            var stringBuffer = new byte[20];
            fileStream.Read(stringBuffer, 0, 20);

            var zmachineTools = new Tools(fileStream);
            var message = zmachineTools.DecodeText(stringBuffer);
            Console.WriteLine(message);
            Console.WriteLine("History of the Meldrews (vol. II)");
            Assert.IsTrue("History of the Meldrews (vol. II)" == message);
        }

        [Test(Description = "Object Table")]
        public void ObjectTable()
        {
            var filename = "Curses\\curses.z5";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;

            var zmachineTools = new Tools(fileStream);
            zmachineTools.Objects();

            Assert.IsTrue(true);
        }


        [Test(Description = "Disassemble")]
        public void BasicDisassemble()
        {
            var filename = "Curses\\curses.z5";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;

            var zmachineTools = new Dissassembler(fileStream);
            zmachineTools.Disassemble();

            Assert.IsTrue(true);
        }
    }
}