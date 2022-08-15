namespace ZMachine.Monogame.Components
{
    public class StreamInputComponent : GameComponent
    {
        private Stream TextStream { get; set; }
        private GameWindow gameWindow;
        private bool isEnterPressed;
        private bool isLeftArrowPressed;
        private int _cursorCharPosition;
        private bool isRightArrowPressed;

        public long TextStreamLength => TextStream.Length;

        public StreamInputComponent(Game game) : base(game)
        {
            TextStream = new MemoryStream();
            gameWindow = game.Window;
            gameWindow.TextInput += GameWindow_TextInput;
        }
        public StreamInputComponent(Game game, Stream textStream) : this(game)
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
                TextStream.WriteByte(13); // ZSCII new line.
                _cursorCharPosition = 0;
            }

            if (kboard.IsKeyDown(Keys.Left) && isLeftArrowPressed == false)
            {
                isLeftArrowPressed = true;
                if (_cursorCharPosition == 0) return;

                _cursorCharPosition -= 1;
            }

            if (kboard.IsKeyDown(Keys.Right) && isRightArrowPressed == false)
            {
                isRightArrowPressed = true;
                _cursorCharPosition += 1;
            }

            if (kboard.IsKeyUp(Keys.Enter) && isEnterPressed) isEnterPressed = false;
            if (kboard.IsKeyUp(Keys.Left) && isLeftArrowPressed) isLeftArrowPressed = false;
            if (kboard.IsKeyUp(Keys.Right) && isRightArrowPressed) isRightArrowPressed = false;
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