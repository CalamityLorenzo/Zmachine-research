using ZMachineTools;

namespace Zmachine.Tests
{
    
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test(Description ="Load v5 story file display header")]
        public void Test1()
        {
            var filename = "Curses\\curses.z5";

            Console.WriteLine($"==== {filename} ====");
            using var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;
            var zmachineTools = new Tools(fileStream);
            zmachineTools.Header();
            Assert.Pass("Passed!");
        }
    }
}