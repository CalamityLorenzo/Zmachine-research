namespace ZMachine.Monogame.Components
{
    // Collects the known text input from an event handler,  and keyboard state, and puts them intoa stream.
    // Also appends other chars (like arrow keys, home, ctrl) into the stream
    // Tihs is the start of the input process, it populates a stream for another component to handle.
    public class StreamInputProcessor : GameComponent
    {
        private Stream TextStream { get; set; }
        private GameWindow gameWindow;
        private bool isEnterPressed;
        private bool isLeftArrowPressed;
        private bool isRightArrowPressed;
        private bool isUpArrowPressed;
        private bool isDownArrowPressed;

        public long TextStreamLength => TextStream.Length;

        public StreamInputProcessor(Game game) : base(game)
        {
            TextStream = new MemoryStream();
            gameWindow = game.Window;
            gameWindow.TextInput += GameWindow_TextInput;
        }
        public StreamInputProcessor(Game game, Stream textStream) : this(game)
        {
            TextStream = textStream;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            CheckNonCharacterKeys(Keyboard.GetState());
        }

        private void CheckNonCharacterKeys(KeyboardState kboard)
        {
            if (kboard.IsKeyDown(Keys.Enter) && isEnterPressed == false)
            {
                isEnterPressed = true;
            //    TextStream.WriteByte(13); // ZSCII new line.
            }

            if (kboard.IsKeyDown(Keys.Left) && isLeftArrowPressed == false)
            {
                isLeftArrowPressed = true;
                TextStream.WriteByte(37);
            }
            if (kboard.IsKeyDown(Keys.Up) && isUpArrowPressed == false)
            {
                isUpArrowPressed = true;
                TextStream.WriteByte(38);
            }

            if (kboard.IsKeyDown(Keys.Right) && isRightArrowPressed == false)
            {
                isRightArrowPressed = true;
                TextStream.WriteByte(39);
            }

            if (kboard.IsKeyDown(Keys.Down) && isDownArrowPressed == false)
            {
                isDownArrowPressed= true;
                TextStream.WriteByte(40);
            }

            if (kboard.IsKeyUp(Keys.Enter) && isEnterPressed) isEnterPressed = false;
            if (kboard.IsKeyUp(Keys.Left) && isLeftArrowPressed) isLeftArrowPressed = false;
            if (kboard.IsKeyUp(Keys.Right) && isRightArrowPressed) isRightArrowPressed = false;

            if (kboard.IsKeyUp(Keys.Up) && isUpArrowPressed) isUpArrowPressed = false;
            if (kboard.IsKeyUp(Keys.Down) && isDownArrowPressed) isDownArrowPressed = false;
        }

        private void GameWindow_TextInput(object sender, TextInputEventArgs e)
        {
            var currentChar = e.Character;
            TextStream.Write(new[] { (byte)currentChar }, 0, 1);
        }

        // Cleans out the stream and resets it.
        public byte[] GetCurrentCharacters()
        {
            var byteArr = new byte[TextStreamLength];
            TextStream.Position = 0;
            TextStream.Read(byteArr, 0, (int)TextStreamLength);
            TextStream.Position = 0;
            TextStream.Close();
            TextStream = new MemoryStream();
            return byteArr;
        }
    }
}