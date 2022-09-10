using System.Diagnostics;
using Zmachine.Library.V2;
using Zmachine.Library.V2.Implementation;

namespace Zmachine.Tests
{
    internal class InstructionTests
    {

        byte[] V3Memory;
        byte[] V5Memory;
        private MemoryStream inputStream0, inputStream1, outputStream;
        private MemoryStream outputTranscript;
        private Machine V5Machine;
        private Machine V3Machine;

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

            this.V3Machine = new Machine(inputStream0, inputStream1, outputStream, outputTranscript, 60, 60, V3Memory);
            this.V5Machine = new Machine(inputStream0, inputStream1, outputStream, outputTranscript, 60, 60, V5Memory);

        }

        #region:Arithmetic
        [Test(Description = "V3 Add two numbers and put on the result on the stack")]
        public void V3AddNumbersStack()
        {

            var routine = new byte[]
           {
                // Routine start
                0,                // local variables
                0x14, 90, 80, 00,  // Add V, V -> sp
                0xb0,       // return true
           };

            ZmachineTools newTools = new(V3Machine);
            newTools.RunRoutine(routine);
            newTools.Step();             // Print

            var stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().LocalStack.Peek() == 170);
        }

        [Test(Description = "V3 Add two numbers and put on the result in local var")]
        public void V3AddNumbersLocalVars()
        {

            var routine = new byte[]
           {
                // Routine start
                1,                // local variables
                00,47, // Two bytes for alocal
                0x14, 90, 80, 01,  // Add S,S -> L1
                0xb0,       // return true
           };

            ZmachineTools newTools = new(V3Machine);
            newTools.RunRoutine(routine);
            var stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 47);
            newTools.Step();             // Print
            stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 170);
        }

        [Test(Description = "V3 Add two numbers and put on the result in local var")]
        public void V3AddVariablesToStack()
        {

            var routine = new byte[]
           {
                // Routine start
                3,                // local variables
                00,47,00,00,10,10, // Two bytes for alocal
                0x74, 01, 03, 00,  // Add V, V -> sp
                0xb0,       // return true
           };

            ZmachineTools newTools = new(V3Machine);
            newTools.RunRoutine(routine);
            var stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 47);
            newTools.Step();             // Print
            stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().LocalStack.Peek() == 2617);
        }

        [Test(Description = "V5 Add two numbers and put on the result on the stack")]
        public void V5AddNumbersStack()
        {

            var routine = new byte[]
           {
                // Routine start
                0,                // local variables
                0x14, 90, 80, 00,  // Add V, V -> sp
                0xb0,       // return true
           };

            ZmachineTools newTools = new(V5Machine);
            newTools.RunRoutine(routine);
            newTools.Step();             // Print

            var stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().LocalStack.Peek() == 170);
        }

        [Test(Description = "V5 Add two numbers and put on the result in local var")]
        public void V5AddNumbersLocalVars()
        {

            var routine = new byte[]
           {
                // Routine start
                1,                // local variables
                0x14, 90, 80, 01,  // Add S,S -> L1
                0xb0,       // return true
           };

            ZmachineTools newTools = new(V5Machine);
            newTools.RunRoutine(routine);
            var stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 0);
            newTools.Step();             // Print
            stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 170);
        }

        [Test(Description = "V5 Add two numbers and put on the result in local var")]
        public void V5AddVariablesToStack()
        {

            var routine = new byte[]
           {
                // Routine start
                3,                // local variables
                0x4d, 01, 47, // Store
                0x4d, 02, 00, // Store 
                0xcd, 0x8f, 03, 10, 10,  // Store V,L
                0x74, 01, 03, 00,  // Add V, V -> sp
           };

            ZmachineTools newTools = new(V5Machine);
            newTools.RunRoutine(routine);
            var stack = newTools.GetStack();
            newTools.Step();
            newTools.Step();
            newTools.Step();
            Assert.IsTrue(stack.Peek().Locals[0] == 47);
            newTools.Step();          // Print
            stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().LocalStack.Peek() == 2617);
        }

        [Test(Description = "V3 divide two numbers and put on the result on the stack")]
        public void V3DivisionToStack()
        {
            var routine = new byte[]
                 {
                        // Routine start
                        0,                // local variables
                        0x17, 40, 20, 00,  // div s, s -> sp
                        0xb0,       // return true
                 };

            ZmachineTools newTools = new(V3Machine);
            newTools.RunRoutine(routine);
            newTools.Step();             // Print

            var stack = newTools.GetStack();
            Console.WriteLine("Value : " + stack.Peek().LocalStack.Peek());
            Assert.IsTrue(stack.Peek().LocalStack.Peek() == 2);
        }

        [Test(Description = "V3 divide two numbers and result to local var")]
        public void V3DivisionToVar()
        {
            var routine = new byte[]
                {
                    // Routine start
                    3,                // local variables
                    00,47,00,00,10,10, // Two bytes for alocal
                    0x17, 40, 20, 01,  // div s, s -> l1
                    0xb0,       // return true
                };

            ZmachineTools newTools = new(V3Machine);
            newTools.RunRoutine(routine);
            var stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 47);
            newTools.Step();             // Print
            stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 2);
        }

        [Test(Description = "V3 Divide Large Constant to local var")]
        public void V3DivisionLargeToVar()
        {
            var routine = new byte[]
                {
                    // Routine start
                    3,                // local variables
                    00,47,00,00,10,10, // Two bytes for alocal
                    0xd7, 0x1f, 40, 40,  200, 01,  // div l, s -> l1
                    0xb0,       // return true
                };

            ZmachineTools newTools = new(V3Machine);
            newTools.RunRoutine(routine);
            var stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 47);
            newTools.Step();             // Print
            stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 51);
        }

        [Test(Description = "V3 Divide Large Constant (small) to local var")]
        public void V3DivisionLarge2ToVar()
        {
            var routine = new byte[]
                {
                    // Routine start
                    3,                // local variables
                    00,47,00,00,10,10, // Two bytes for alocal
                    0xd7, 0x1f, 00, 40,  10, 01,  // div l, s -> l1
                    0xb0,       // return true
                };

            ZmachineTools newTools = new(V3Machine);
            newTools.RunRoutine(routine);
            var stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 47);
            newTools.Step();             // Print
            stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 4);
        }

        [Test(Description = "V3 Divide by zero")]
        public void V3BadDivision()
        {

            var routine = new byte[]
                {
                    // Routine start
                    3,                // local variables
                    00,47,00,00,10,10, // Two bytes for alocal
                    0x17, 0, 20, 01,  // div s, s -> l1
                    0xb0,       // return true
                };

            ZmachineTools newTools = new(V3Machine);
            newTools.RunRoutine(routine);
            var stack = newTools.GetStack();
            Assert.Catch(() => newTools.Step());             // Print

        }

        [Test(Description = "V3 divide two numbers and put on the result on the stack")]
        public void V5DivisionToStack()
        {
            var routine = new byte[]
                 {
                        // Routine start
                        0,                // local variables
                        0x17, 40, 20, 00,  // div s, s -> sp
                        0xb0,       // return true
                 };

            ZmachineTools newTools = new(V5Machine);
            newTools.RunRoutine(routine);
            newTools.Step();             // Print

            var stack = newTools.GetStack();
            Console.WriteLine("Value : " + stack.Peek().LocalStack.Peek());
            Assert.IsTrue(stack.Peek().LocalStack.Peek() == 2);
        }

        [Test(Description = "V3 divide two numbers and result to local var")]
        public void V5DivisionToVar()
        {
            var routine = new byte[]
                {
                    // Routine start
                    3,                // local variables
                    0x4d, 01, 47, // Store 
                    0x4d, 02, 00, // Store 
                    0xcd, 0x8f,  03, 10,10, // Store 
                    0x17, 40, 20, 01,  // div s, s -> l1
                    0xb0,       // return true
                };

            ZmachineTools newTools = new(V5Machine);
            newTools.RunRoutine(routine);
            newTools.Step();
            newTools.Step();
            newTools.Step();
            var stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 47);
            stack = newTools.GetStack();
            newTools.Step();
            Assert.IsTrue(stack.Peek().Locals[0] == 2);
            newTools.Step();             // Print
        }

        [Test(Description = "V3 Divide Large Constant to local var")]
        public void V5DivisionLargeToVar()
        {
            var routine = new byte[]
                {
                    // Routine start
                    3,                // local variables
                    0x4d, 01, 47, // Store 
                    0x4d, 02, 00, // Store 
                    0xcd, 0x8f,  03, 10,10, // Store 
                    0xd7, 0x1f, 40, 40,  200, 01,  // div l, s -> l1
                    0xb0,       // return true
                };

            ZmachineTools newTools = new(V5Machine);
            newTools.RunRoutine(routine);
            newTools.Step();
            newTools.Step();
            newTools.Step();
            var stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 47);
            newTools.Step();             // Print
            stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 51);
        }

        [Test(Description = "V5 Divide Large Constant (small) to local var")]
        public void V5DivisionLarge2ToVar()
        {
            var routine = new byte[]
                {
                    // Routine start
                    3,                // local variables
                    0x4d, 01, 47, // Store 
                    0x4d, 02, 00, // Store 
                    0xcd, 0x8f,  03, 10,10, // Store 
                    0xd7, 0x1f, 00, 40,  10, 01,  // div l, s -> l1
                    0xb0,       // return true
                };

            ZmachineTools newTools = new(V5Machine);
            newTools.RunRoutine(routine);
            newTools.Step();
            newTools.Step();
            newTools.Step();
            var stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 47);
            newTools.Step();             // Print
            stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 4);
        }

        [Test(Description = "V5 Divide by zero")]
        public void V5BadDivision()
        {

            var routine = new byte[]
                {
                    // Routine start
                    3,                // local variables
                    0x4d, 01, 47, // Store 
                    0x4d, 02, 00, // Store 
                    0xcd, 0x8f,  03, 10,10, // Store                   
                    0x17, 0, 20, 01,  // div s, s -> l1
                    0xb0,       // return true
                };

            ZmachineTools newTools = new(V5Machine);
            newTools.RunRoutine(routine);
            newTools.Step();
            newTools.Step();
            newTools.Step();
            Assert.Catch(() => newTools.Step());             // Print

        }

        [Test(Description = "V3 Add two numbers and put on the result on the stack")]
        public void V3SubNumbersStack()
        {

            var routine = new byte[]
           {
                // Routine start
                1,                // local variables
                00,00,
                0x15, 90, 80, 00,  // sub S,S-> sp
                0x35, 90, 00, 01,  // sub S,S-> sp
                0x55, 01, 20, 16,  // sub V,S-> sp
                0xb0,       // return true
           };

            ZmachineTools newTools = new(V3Machine);
            newTools.RunRoutine(routine);
            newTools.Step();             // Sub
            var stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().LocalStack.Peek() == 10);
            newTools.Step();             // Sub
            stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 80);
            newTools.Step();             // Sub
            stack = newTools.GetStack();
            var s = (newTools.GetMemoryLocation(8866) <<8 | newTools.GetMemoryLocation(8867));
            var glob = newTools.GetGlobalVariable(16 - 16);
            Assert.IsTrue(glob == 60);
        }

        [Test(Description = "V5 Add two numbers and put on the result on the stack")]
        public void V5SubNumbersStack()
        {

            var routine = new byte[]
           {
                // Routine start
                1,                // local variables
                0x15, 90, 80, 00,  // sub S,S-> sp
                0x35, 90, 00, 01,  // sub S,S-> sp
                0x55, 01, 20, 16,  // sub V,S-> sp
                0xb0,       // return true
           };

            ZmachineTools newTools = new(V5Machine);
            newTools.RunRoutine(routine);
            newTools.Step();             // Sub
            var stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().LocalStack.Peek() == 10);
            newTools.Step();             // Sub
            stack = newTools.GetStack();
            Assert.IsTrue(stack.Peek().Locals[0] == 80);
            newTools.Step();             // Sub
            stack = newTools.GetStack();
            var s = (newTools.GetMemoryLocation(8866) << 8 | newTools.GetMemoryLocation(8867));
            var glob = newTools.GetGlobalVariable(16 - 16);
            Assert.IsTrue(glob == 60);
        }

        #endregion
    }
}