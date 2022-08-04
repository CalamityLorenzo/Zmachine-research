using Microsoft.Xna.Framework;
using System.IO;

namespace ZMachine.Monogame.Component
{
    internal class TypedTextSource
    {
        private Stream TextStream { get; set; }
        private GameWindow gameWindow;

        public long TextStreamLength => this.TextStream.Length;

        public TypedTextSource(GameWindow window)
        {
            TextStream = new MemoryStream();
            gameWindow = window;
            gameWindow.TextInput += GameWindow_TextInput;
        }

        public TypedTextSource(GameWindow window, Stream input) : this(window)
        {
            TextStream = input;
        }


        private void GameWindow_TextInput(object sender, TextInputEventArgs e)
        {
            var currentChar = e.Character;
            this.TextStream.Write(new[] { (byte)currentChar }, 0, 1);
        }

        // Cleans out the stream and resets it.
        public byte[] GetCurrentCharacters()
        {
            var byteArr = new byte[this.TextStreamLength];
            this.TextStream.Position = 0;
            this.TextStream.Read(byteArr, 0, (int)this.TextStreamLength);
            this.TextStream.Position = 0;
            this.TextStream.Close();
            this.TextStream = new MemoryStream();
            return byteArr;
        }
    }
}