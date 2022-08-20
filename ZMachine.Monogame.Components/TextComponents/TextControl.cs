using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZMachine.Monogame.Components.TextComponents
{

    public class LineAddedEventArgs : EventArgs
    {
        public LineAddedEventArgs(string newLine)
        {
            NewLine = newLine;
        }

        public string NewLine { get; }
    }
    /// <summary>
    /// A basic text control. reads input from a stream.
    /// Outputs text into another output stream.
    /// </summary>
    public class TextControl : DrawableGameComponent
    {
        private int _cursorPosition = 0;
        private List<char> _currentContent;

        public string Prompt { get; }

        private Vector2 promptSize;
        private Vector2 chrSize;
        private BasicBlinkCursor cursor;
        /// <summary>
        /// This is only used interally to sdisplay,and to limit the amount of converting from array -> String
        /// </summary>
        private string _currentLine;
        private readonly SpriteFont currentFont;
        private readonly Stream inputStream;
        private readonly Stream ouputStream;
        private Vector2 position;
        private SpriteBatch _spriteBatch;

        public string Value { get => new string(_currentContent.ToArray()); }
        public event Action<object, EventArgs> OnValueChanged;
        public TextControl(Game game, SpriteFont fnt, Stream inputStream, Stream ouputStream, Vector2 position) : base(game)
        {
            currentFont = fnt;
            this.inputStream = inputStream;
            this.ouputStream = ouputStream;
            this.position = position;
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
            _currentContent = new List<char>();
            Prompt = ">";
            promptSize = fnt.MeasureString(Prompt);
            chrSize = fnt.MeasureString("0");
            this._currentLine = "";
            // The Anchor position must take into account the prompt.
            // So this first position is actually 1.
            this.cursor = new BasicBlinkCursor(this.Game, _spriteBatch, 200.0f, fnt, Color.DarkGreen, new Vector2(this.position.X, this.position.Y), chrSize, "_");
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            cursor.Update(gameTime);
            ProcessStream();
        }

        internal void SetPosition(Vector2 newPosition)
        {
            if (position != newPosition) position = newPosition;
        }
        private void ProcessStream()
        {
            if (inputStream.Length > 0)
            {
                // Do stream things
                _currentLine = "";
                inputStream.Position = 0;
                using StreamReader sr = new StreamReader(inputStream, Encoding.UTF8, bufferSize: (int)inputStream.Length, leaveOpen: true);
                var theChars = new char[inputStream.Length];
                Span<char> sp = theChars;
                sr.Read(sp);
                sr.Close();

                // enumerate the colleciton of chars

                var charEnumerator = sp.GetEnumerator();

                while (charEnumerator.MoveNext())
                {
                    var currentChar = charEnumerator.Current;
                    // Cursors
                    if (currentChar == (char)Keys.Left)
                    {
                        ChangeCursorPosition(-1);
                    }
                    else if (currentChar == (char)Keys.Right)
                    {
                        ChangeCursorPosition(1);
                    }
                    else if (currentChar == '\b') // Backspace.
                    {
                        if (_currentContent.Count > 0 && _cursorPosition == _currentContent.Count)
                            _currentContent.RemoveAt(_currentContent.Count - 1);
                        else if (_cursorPosition > -1 && _currentContent.Count > 0)
                        {

                            _currentContent.RemoveAt(_cursorPosition - 2);
                            ChangeCursorPosition(-1);
                        }
                    }
                    else if (currentChar == (char)Keys.Escape)
                    {
                        _currentContent = new List<char>();
                    }
                    else if (currentChar == (byte)Keys.Enter)
                    {
                        if (_currentContent.Count > 0)
                        {
                            _currentContent.Add(currentChar);
                            _currentContent.InsertRange(0, this.Prompt.ToArray());
                            var outputContent = Encoding.UTF8.GetBytes(_currentContent.ToArray());
                            ouputStream.Write(outputContent, 0, outputContent.Length);
                            _currentContent = new List<char>();
                            RaiseValueChanged();
                            _cursorPosition = 0;
                        }
                    }
                    else
                    {
                        // This is all cursor position dependent
                        if (_cursorPosition == _currentContent.Count + 1)
                        {
                            _currentContent.Add(currentChar);
                        }
                        else
                        {
                            _currentContent.Insert(_cursorPosition, currentChar);
                        }
                        ChangeCursorPosition(1);

                        RaiseValueChanged();
                    }
                }
                this._currentLine = new string(_currentContent.ToArray());

                // Measure the string up to the cursor position;
                cursor.MoveCursorPosition(_cursorPosition == _currentLine.Length + 1 ? _cursorPosition - 1 : _cursorPosition);

                inputStream.SetLength(0);
            }
        }

        private void ChangeCursorPosition(int cursorChange)
        {
            _cursorPosition += cursorChange;
            // Left
            if (_cursorPosition < 0) _cursorPosition = 0;
            // Right boundary
            if (_cursorPosition > _currentContent.Count + 1)
                _cursorPosition = _currentContent.Count + 1;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            _spriteBatch.Begin();
            _spriteBatch.DrawString(currentFont, Prompt, new Vector2(position.X - promptSize.X, position.Y), Color.White);
            _spriteBatch.DrawString(currentFont, _currentLine, position, Color.White);
            this.cursor.Draw(gameTime);
            _spriteBatch.End();
        }

        protected virtual void RaiseValueChanged() => OnValueChanged?.Invoke(this, new EventArgs());


    }
}
