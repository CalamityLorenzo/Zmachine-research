using System.Text.Encodings.Web;
using ZMachine.Library.V1;
using ZMachine.Library.V1.Objects;
using ZMachineTools;

namespace Zmachine.Tests
{

    public class BasicTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test(Description = "V5 Load story file display header")]
        public void V5HeaderDetails()
        {
            var filename = "Curses\\curses.z5";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;
            var zmachineTools = new Tools(fileStream);
            zmachineTools.Header();
            Assert.Pass("Passed!");
        }

        [Test(Description = "V3 story file display header")]
        public void V3HeaderDetails()
        {
            var filename = "Curses\\hollywoo.dat";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;
            var zmachineTools = new Tools(fileStream);
            zmachineTools.Header();
            Assert.Pass("Passed!");
        }

        [Test(Description = "V3 Display all the abbreviations")]
        public void V3AbbreviationTable()
        {
            var filename = "Curses\\hollywoo.dat";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;
            var zmachineTools = new Tools(fileStream);
            zmachineTools.Abbreviations();
            Assert.Pass("Passed!");
        }


        [Test(Description = "V5 Display all the abbreviations")]
        public void V5AbbreviationTable()
        {
            var filename = "Curses\\curses.z5";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;
            var zmachineTools = new Tools(fileStream);
            zmachineTools.Abbreviations();
            Assert.Pass("Passed!");
        }

        [Test(Description = "V5 Display all the Dictionary Entries")]
        public void V5DictionaryTable()
        {
            var filename = "Curses\\curses.z5";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;
            var zmachineTools = new Tools(fileStream);
            zmachineTools.Dictionary();
            Assert.Pass("Passed!");
        }

        [Test(Description = "V3 Display all the Dictionary Entries")]
        public void V3DictionaryTable()
        {
            var filename = "Curses\\hollywoo.dat";

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
            //var message = zmachineTools.DecodeText(new byte[] { 17, 52, 79, 32, 122, 154, 3, 45, 58, 112, 3, 45, 42, 234, 3, 13, 83, 81, 36, 7, 40, 18, 82, 234, 2, 139, 3, 45, 27, 37, 212, 165 });
            var message = zmachineTools.DecodeText(new byte[] { 20, 193, 147 , 106});
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


        [Test(Description = "String tests Encode words")]
        public void EncodeWords()
        {
            var filename = "Curses\\curses.z5";
            using var fileStream = File.Open(filename, FileMode.Open);
            var zmachineTools = new Tools(fileStream);
            
            //var message2 = zmachineTools.DecodeText(stringBuffer);
            var messageToEncode = "Maybe\r\rThere could be more of this?\r         instead~@:$£^\r";
            // Create raw zchars
            var zChars = zmachineTools.EncodeText(messageToEncode);
            // Encode into words and then spit into byte array
            var encodedBytes = zmachineTools.EncodeWords(zChars);
            // finally the text
            var completeDecodedText = zmachineTools.DecodeText(encodedBytes);

            var i = 0;
            var decodedZChars = TextProcessor.GetZChars(encodedBytes, ref i);

            Console.WriteLine($"Raw :\t{messageToEncode}");
            //Console.WriteLine($"Chars :\t {zChars}");
            Console.WriteLine($"Words :\t{completeDecodedText}");

            // Are these two the same?
            Console.WriteLine(String.Join(",", decodedZChars));
            Console.WriteLine(String.Join(",", zChars));
            
            Console.WriteLine(String.Join(",", encodedBytes));

            Assert.IsTrue(messageToEncode == completeDecodedText);
        }






        [Test(Description = "V5 Object Table")]
        public void V5ObjectTable()
        {
            var filename = "Curses\\curses.z5";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;

            var zmachineTools = new Tools(fileStream);
            zmachineTools.Objects();

            Assert.IsTrue(true);
        }


        [Test(Description = "V3 Object Table")]
        public void V3ObjectTable()
        {
            var filename = "Curses\\hollywoo.dat";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;

            var zmachineTools = new Tools(fileStream);
            zmachineTools.Objects();

            Assert.IsTrue(true);
        }

        [Test(Description = "V3 Validate an object")]
        public void V3ConfirmObjects()
        {
            var filename = "Curses\\hollywoo.dat";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;

            var zmachineTools = new Tools(fileStream);
            
            // Get a ridiculous id for a v3
            //Assert.Catch(typeof(ArgumentOutOfRangeException), new TestDelegate(()=> zmachineTools.GetObject(65535)));
            
            ZmObject v3Object1 = zmachineTools.GetObject(1);
            ZmObject v3Object2 = zmachineTools.GetObject(2);
            ZmObject v3Object21 = zmachineTools.GetObject(21);
            ZmObject v3Object22 = zmachineTools.GetObject(22);
            Assert.IsNotNull(v3Object1);
            Console.WriteLine(zmachineTools.DecodeText(v3Object21.PropertyTable.shortNameBytes));
            Console.WriteLine(v3Object1);
            Console.WriteLine(v3Object2);
            Console.WriteLine(v3Object21);
            Assert.IsTrue(true);

        }

        [Test(Description = "V5 Validate an object")]
        public void V5ConfirmObjects()
        {
            var filename = "Curses\\curses.z5";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;

            var zmachineTools = new Tools(fileStream);

            // Get a ridiculous id for a v3
            //Assert.Catch(typeof(ArgumentOutOfRangeException), new TestDelegate(()=> zmachineTools.GetObject(65535)));

            ZmObject v5Object1 = zmachineTools.GetObject(1);
            Console.WriteLine(zmachineTools.DecodeText(v5Object1.PropertyTable.shortNameBytes));
            Assert.IsNotNull(v5Object1);
            ZmObject v5Object21 = zmachineTools.GetObject(21);
            ZmObject v5Object40 = zmachineTools.GetObject(40);
            ZmObject v5Object39 = zmachineTools.GetObject(39);
            Console.WriteLine(v5Object1);
            Console.WriteLine(v5Object40);
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