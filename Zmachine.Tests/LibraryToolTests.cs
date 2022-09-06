using Zmachine.Library.V2;
using Zmachine.Library.V2.Implementation;
using Zmachine.Library.V2.Objects;

namespace Zmachine.Tests
{
    internal class LibraryToolTests
    {
        byte[] V3Memory;
        byte[] V5Memory;
        private MemoryStream inputStream0, inputStream1, outputStream;
        private MemoryStream outputTranscript;
        private Machine v5Machine;
        private Machine v3Machine;

        [SetUp]
        public void Setup()
        {
            var filename = "Curses\\curses.z5";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;
            var v5Length = fileStream.Length;

            var V5MemoryStream = new MemoryStream();
            fileStream.CopyTo(V5MemoryStream);
            V5MemoryStream.Position = 0;
            V5Memory = new byte[V5MemoryStream.Length];
            V5MemoryStream.Read(V5Memory, 0, V5Memory.Length);

         //   fileStream.Write(V5Memory.AsSpan());

            filename = "Curses\\hollywoo.dat";
            Console.WriteLine($"==== {filename} ====");
            using var v3FileStream = File.Open(filename, FileMode.Open);
            v3FileStream.Position = 0;
            var v3Length = v3FileStream.Length;
            V3Memory = new byte[v3Length];
            v3FileStream.Read(V3Memory.AsSpan());

            this.inputStream0 = new MemoryStream();
            this.inputStream1 = new MemoryStream();
            this.outputStream = new MemoryStream();
            this.outputTranscript = new MemoryStream();

            this.v3Machine = new Machine(inputStream0, inputStream1, outputStream, outputTranscript, V3Memory);
            this.v5Machine = new Machine(inputStream0, inputStream1, outputStream, outputTranscript, V5Memory);

        }

        [Test(Description = "V5 Load story file display header")]
        public void V5HeaderDetails()
        {
            ZmachineTools newTools = new(v5Machine);
            newTools.DumpHeader();
            Assert.IsTrue(true);
        }

        [Test(Description = "V3 story file display header")]
        public void V3HeaderDetails()
        {
            ZmachineTools newTools = new(v3Machine);
            newTools.DumpHeader();
            Assert.IsTrue(true);
            Assert.Pass("Passed!");
        }

        [Test(Description = "V3 Display all the abbreviations")]
        public void V3AbbreviationTable()
        {
            ZmachineTools newTools = new(v3Machine);
            newTools.DumpAbbreviations();
            Assert.IsTrue(true);
            Assert.Pass("Passed!");
        }

        [Test(Description = "V5 Display all the abbreviations")]
        public void V5AbbreviationTable()
        {
            ZmachineTools newTools = new(v5Machine);
            newTools.DumpAbbreviations();
            Assert.IsTrue(true);
            Assert.Pass("Passed!");
        }

        [Test(Description = "V5 Display all the Dictionary Entries")]
        public void V5DictionaryTable()
        {
            ZmachineTools newTools = new(v5Machine);
            newTools.DumpDictionary();
            Assert.IsTrue(true);
            Assert.Pass("Passed!");

        }

        [Test(Description = "V3 Display all the Dictionary Entries")]
        public void V3DictionaryTable()
        {
            ZmachineTools newTools = new(v3Machine);
            newTools.DumpDictionary();
            Assert.IsTrue(true);
            Assert.Pass("Passed!");
        }

        [Test(Description = "V5 Decode Provided Text")]
        public void V5DecodeText()
        {
            ZmachineTools newTools = new(v5Machine);

            var message = newTools.DecodeZChars(new byte[] { 4, 13, 14, 24, 25, 20, 23, 30, 0, 20, 11, 0, 25, 13, 10, 0, 4, 18, 10, 17, 9, 23, 10, 28, 24, 0, 5, 30, 27, 20, 17, 5, 18, 0, 4, 14, 4, 14, 5, 31, 5, 5 });
            //var message = zmachineTools.DecodeText(new byte[] { 20, 193, 147 , 106});
            Console.WriteLine(message);
            Console.WriteLine("History of the Meldrews (vol. II)");
            Assert.IsTrue("History of the Meldrews (vol. II)" == message);
        }

        [Test(Description = "V3 Decode Provided Text")]
        public void V3DecodeText()
        {
            ZmachineTools newTools = new(v3Machine);
            var leChars = newTools.GetMemoryRange(0xB273, 0xB27D - 0xb273);
            var refInt = 0;
            var leZChars = TextProcessor.GetZChars(leChars, ref refInt);
            var chars = newTools.EncodeToZChars("It's too dark to see!");
            var message = newTools.DecodeZChars(leZChars);

            //var message = newTools.DecodeZChars(new byte[] { 4, 13, 14, 24, 25, 20, 23, 30, 0, 20, 11, 0, 25, 13, 10, 0, 4, 18, 10, 17, 9, 23, 10, 28, 24, 0, 5, 30, 27, 20, 17, 5, 18, 0, 4, 14, 4, 14, 5, 31, 5, 5 });
            //var message = zmachineTools.DecodeText(new byte[] { 20, 193, 147 , 106});
            Console.WriteLine(message);
            Console.WriteLine("History of the Meldrews (vol. II)");
            Assert.IsTrue("History of the Meldrews (vol. II)" == message);
        }

        [Test(Description = "String tests Encode")]
        public void EncodeText()
        {
            var filename = "Curses\\curses.z5";
            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0x22AC;
            var stringBuffer = new byte[20];
            fileStream.Read(stringBuffer, 0, 20);
            ZmachineTools newTools = new(v5Machine);
            //var message2 = zmachineTools.DecodeText(stringBuffer);
            var messageToEncode = "(£$%^&\"\r)";
            messageToEncode = "History of the Meldrews (vol. II)";
            var encodedBytes = newTools.EncodeToZChars(messageToEncode);
            var message = newTools.DecodeZChars(encodedBytes);
            Console.WriteLine(message);
            Console.WriteLine(messageToEncode);
            Console.WriteLine(String.Join(",", encodedBytes));
            Assert.IsTrue(messageToEncode == message);
        }

        [Test(Description = "String tests Encode words")]
        public void EncodeWords()
        {
            ZmachineTools newTools = new(v5Machine);

            //var message2 = zmachineTools.DecodeText(stringBuffer);
            var messageToEncode = "Maybe\r\rThere could be more of this?\r         instead~@:$£^\r";
            // Create raw zchars
            var zChars = newTools.EncodeToZChars(messageToEncode);
            // Encode into words and then spit into byte array
            var encodedBytes = newTools.EncodeWords(zChars);
            // finally the text
            var completeDecodedText = newTools.DecodeEncodedText(encodedBytes);

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
            ZmachineTools newTools = new(v5Machine);

            newTools.DumpObjects();

            Assert.IsTrue(true);
        }

        [Test(Description = "V3 Object Table")]
        public void V3ObjectTable()
        {
            ZmachineTools newTools = new(v3Machine);

            newTools.DumpObjects();
            Assert.IsTrue(true);
        }

        [Test(Description = "V3 Validate an object")]
        public void V3ConfirmObjects()
        {
            ZmachineTools newTools = new(v3Machine);

            // Get a ridiculous id for a v3
            Assert.Catch(typeof(ArgumentOutOfRangeException), new TestDelegate(() => newTools.GetObject(65535)));


            ZmObject v3Object22 = newTools.GetObject(22);
            Assert.IsTrue(newTools.DecodeEncodedText(v3Object22.PropertyTable.shortNameBytes) == "indigo punch card");
            Assert.IsTrue(Enumerable.SequenceEqual(v3Object22.Attributes, new List<byte> { 17, 21, 29 }));
            Assert.IsTrue(Enumerable.SequenceEqual(v3Object22.PropertyTable.properties[1].PropertyData, new byte[] { 0x42, 0x02, 0x42, 0x09 }));
            Assert.IsTrue(v3Object22.Parent == 223 &&
                          v3Object22.Sibling == 0 &&
                          v3Object22.Child == 0);
        }

        [Test(Description = "V5 Validate an object")]
        public void V5ConfirmObjects()
        {
            ZmachineTools newTools = new(v5Machine);
            // Get a ridiculous id for a v3
            Assert.Catch(typeof(ArgumentOutOfRangeException), new TestDelegate(() => newTools.GetObject(65535)));

            ZmObject v5Object1 = newTools.GetObject(1);
            //Console.WriteLine(newTools.DecodeEncodedText(v5Object1.PropertyTable.shortNameBytes));
            Assert.IsNotNull(v5Object1);
            ZmObject v5Object21 = newTools.GetObject(21);
            Assert.IsTrue((v5Object21.Attributes[0] == 28 &&
                           v5Object21.Attributes[1] == 34));
            Assert.IsTrue(newTools.DecodeEncodedText(v5Object21.PropertyTable.shortNameBytes) == "Husbandry");
            var propertyTable = v5Object21.PropertyTable.properties;
            Assert.IsTrue(propertyTable[0].Size == 2);
            Assert.IsTrue(propertyTable[0].propertyNumber == 35);
            Assert.IsTrue(Enumerable.SequenceEqual(propertyTable[0].PropertyData, new byte[] { 0xb6, 0xd8 }));
        }
        [Test(Description = "V5 Ouputs all the globals available")]
        public void V5ConfirmGlobals()
        {
            ZmachineTools newTools = new(v5Machine);
            newTools.DumpGlobals();
            Assert.IsTrue(true);
        }

        [Test(Description = "V3 Ouputs all the globals available")]
        public void V3ConfirmGlobals()
        {
            ZmachineTools newTools = new(v3Machine);
            newTools.DumpGlobals();
            Assert.IsTrue(true);
        }

        [Test(Description = "V5 Sets a global variable to value and checks it.")]
        public void V5SetGlobal()
        {
            ZmachineTools newTools = new(v5Machine);
            newTools.SetGlobalVariable(0, 57);
            var result =  newTools.GetGlobalVariable(0);
            Assert.IsTrue(result == 57);
            newTools.DumpGlobals();
        }

        [Test(Description = "V3 Sets a global variable to value and checks it.")]
        public void V3SetGlobal()
        {
            ZmachineTools newTools = new(v3Machine);
            newTools.SetGlobalVariable(37, 44);
            var result = newTools.GetGlobalVariable(37);
            Assert.IsTrue(result == 44);
            newTools.DumpGlobals();
        }

        [Test(Description = "5 Inject and run a custom routine.")]
        public void V5RunCustomRoutine()
        {
            var routine = new byte[]
            {
                // Routine start
                3,           // local variables
                0xb2, 18,42,103,0,25,41,3,20,73,64,79,82,29,87,96,180,148,229,  //Print a big fat string.
                //0xbb,
                //0xb2, 17,83,101,87,1,110,95,25,2,122,72,234,92,189,148,229,    // More groovy strings
                //0xe4, 15, 0x5d, 0xd5, 0x5e, 0x4e, 0xff,
                //0xb2, 17,52,79,32,122,154,3,45,58,112,3,45,42,234,3,13,83,81,36,7,40,18,82,234,2,139,3,45,27,37,212,167,
                //0xb2, 18,70,120,234,20,229,28,153,53,87,40,8,83,81,36,7,40,18,82,234,2,139,3,45,59,5,84,167,0,0,0,0,0,0,58,120,101,70,36,166,15,197,24,64,23,165,24,36,20,197,12,166,11,197,156,165,
                //0xbb,
                //0xbb,
                0x0d, 01, 15,     // Store 15 lVar1
                0x0d, 02, 05,     // Store 05 lVar2
                0x14, 1, 2, 00,  // Add V, V -> sp
                
                0x8c, 255,119,   // Jump, jump back around
                0xb0,       // return true
            };


            ZmachineTools newTools = new(v5Machine);
            newTools.RunRoutine(routine);
            newTools.Step();             // local vars
            newTools.Step();             // Print
            newTools.Step();             // Store ->l1
            newTools.Step();             // Store ->l2
            newTools.Step();             // Add -> Sp

            var callStack = newTools.GetStack();

            Assert.IsTrue(callStack.Peek().LocalStack.Peek() == 3);
            Assert.IsTrue(callStack.Peek().Locals[0] == 15);
            Assert.IsTrue(callStack.Peek().Locals[1] == 05);

            if (this.outputStream.Length > 0)
            {
                outputStream.Position = 0;
                using var sr = new StreamReader(outputStream);
                var x = sr.ReadToEnd();
                Console.WriteLine(x);
                Assert.IsTrue("Lets add some numbers!\r".Equals(x));
            }
        }

        [Test(Description = "3 Inject and run a custom routine.")]
        public void V3RunCustomRoutine()
        {
            var routine = new byte[]
            {
                // Routine start
                3,                // local variables
                0,00,00,20,40,40, // local variables
                0xb2, 18,42,103,0,25,41,3,20,73,64,79,82,29,87,96,180,148,229,  //Print a big fat string.,
                0x0d, 01, 15,     // Store 15 lVar1
                0x0d, 02, 05,     // Store 05 lVar2
                0x14, 1, 2, 00,  // Add V, V -> sp
                0xb0,       // return true
            };


            ZmachineTools newTools = new(v3Machine);
            newTools.RunRoutine(routine);
            newTools.Step();             // Print
            var callStack = newTools.GetStack();
            Assert.IsTrue(callStack.Peek().Locals[0] == 0);
            Assert.IsTrue(callStack.Peek().Locals[2] == 10280);

            newTools.Step();             // Store ->l1
            newTools.Step();             // Store ->l2
            newTools.Step();             // Add -> Sp

            callStack = newTools.GetStack();

            var callStackSize = callStack.Count;
            Assert.IsTrue(callStack.Peek().LocalStack.Peek() == 3);
            Assert.IsTrue(callStack.Peek().Locals[0] == 15);
            Assert.IsTrue(callStack.Peek().Locals[1] == 05);

            newTools.Step();              // return true
            // Call stack is decremented.
            Assert.IsTrue(callStackSize > newTools.GetStack().Count);
            if (this.outputStream.Length > 0)
            {
                outputStream.Position = 0;
                using var sr = new StreamReader(outputStream);
                var x = sr.ReadToEnd();
                Console.WriteLine(x);
                Assert.IsTrue("Lets add some numbers!\r".Equals(x));
            }
        }
    }
}
