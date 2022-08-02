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

        [Test(Description = "String tests Encode")]
        public void EncodeText()
        {
            var filename = "Curses\\curses.z5";

            //Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0x22AC;

            var stringBuffer = new byte[20];
            fileStream.Read(stringBuffer, 0, 20);
            var zmachineTools = new Tools(fileStream);
            //var message2 = zmachineTools.DecodeText(stringBuffer);
            var messageToEncode = "(£$%^&\"\r)";
            var encodedBytes = zmachineTools.EncodeText(messageToEncode);
            var message = zmachineTools.DecodeZChars(encodedBytes);
            Console.WriteLine(message);
            Console.WriteLine(messageToEncode);
            Assert.IsTrue(messageToEncode == message);
        }


        [Test(Description = "String tests Encode")]
        public void EncodeWords()
        {
            var filename = "Curses\\curses.z5";

            //Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            var zmachineTools = new Tools(fileStream);
            //var message2 = zmachineTools.DecodeText(stringBuffer);
            var messageToEncode = "Where the weary wander @\"£ and the mighty cheese drowned.";
            var zChars = zmachineTools.EncodeText(messageToEncode);
            var decodedZChars = zmachineTools.DecodeZChars(zChars);
            var encodedBytes= zmachineTools.EncodeWords(zChars);
            var wordsEncoded = zmachineTools.DecodeText(encodedBytes);
            Console.WriteLine($"Raw :\t{messageToEncode}");
            Console.WriteLine($"Chars :\t {decodedZChars}");
            Console.WriteLine($"Words :\t {wordsEncoded}");
            Assert.IsTrue(decodedZChars == wordsEncoded);
            Assert.IsTrue(messageToEncode == wordsEncoded);
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


        [Test(Description = "Disassemble")]
        public void BasicInstructionDecoding()
        {
            var filename = "Curses\\curses.z5";

            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;

            var zmachineTools = new Dissassembler(fileStream);
            var instructions = new byte[]
            {
                0xb2, 19,141,42,234,3,45,40,28,40,215,120,28,26,105,42,224,20,194,0,185,20,197,12,6,77,32,101,170,2,78,49,185,120,8,53,74,97,64,38,244,114,106,164,178
            };
            zmachineTools.Decode(instructions);

            Assert.IsTrue(true);
        }
    }
}