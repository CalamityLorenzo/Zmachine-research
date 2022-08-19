using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZMachine.Monogame.Components
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
        private int _cursorPosition = 1;
        private List<char> _currentContent;
        private string _currentLine;
        private readonly SpriteFont currentFont;
        private readonly Stream inputStream;
        private readonly Stream ouputStream;
        private Vector2 position;
        private SpriteBatch _spriteBatch;

        public string Value { get => new String(this._currentContent.ToArray()); }
        public event Action<object, EventArgs> OnValueChanged;
        public TextControl(Game game, SpriteFont fnt, Stream inputStream, Stream ouputStream, Vector2 position) : base(game)
        {
            this.currentFont = fnt;
            this.inputStream = inputStream;
            this.ouputStream = ouputStream;
            this.position = position;
            this._spriteBatch = new SpriteBatch(game.GraphicsDevice);
            this._currentContent = new List<char>();
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
            this.ProcessStream();
        }

        private void SetPosition(Vector2 newPosition) => this.position = newPosition;
        private void ProcessStream()
        {
            if (this.inputStream.Length > 0)
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
                        this.ChangeCursorPosition(-1);
                    }
                    else if (currentChar == (char)Keys.Right)
                    {
                        this.ChangeCursorPosition(1);
                    }
                    else if (currentChar == '\b') // Backspace.
                    {
                        if (this._currentContent.Count > 0 && _cursorPosition == _currentContent.Count)
                            this._currentContent.RemoveAt(_currentContent.Count - 1);
                        else if (_cursorPosition > -1 && this._currentContent.Count>0)
                        {
                            
                           this._currentContent.RemoveAt(_cursorPosition-2);
                            ChangeCursorPosition(-1);
                        }
                    }
                    else if (currentChar == (char)Keys.Escape)
                    {
                        this._currentContent = new List<char>();
                    }
                    else if (currentChar == (byte)Keys.Enter)
                    {
                        _currentContent.Add(currentChar);
                        var outputContent = System.Text.Encoding.UTF8.GetBytes(_currentContent.ToArray());
                        this.ouputStream.Write(outputContent, 0, outputContent.Length);
                        this._currentContent = new List<char>();
                        this.RaiseValueChanged();
                        _cursorPosition = 0;
                    }
                    else
                    {
                        // This is all cursor position dependent
                        if (_cursorPosition == _currentContent.Count+1)
                        {
                            this._currentContent.Add(currentChar);
                        }
                        else
                        {
                            this._currentContent.Insert(_cursorPosition, currentChar);
                        }
                        this.ChangeCursorPosition(1);

                        this.RaiseValueChanged();
                    }
                }

                inputStream.SetLength(0);
            }
        }

        private void ChangeCursorPosition(int cursorChange)
        {
            this._cursorPosition += cursorChange;
            // Left
            if (_cursorPosition < 0) this._cursorPosition = 0;
            // Right boundary
            if (_cursorPosition > this._currentContent.Count + 1) this._cursorPosition = _currentLine.Length + 1;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            this._spriteBatch.Begin();
            this._spriteBatch.DrawString(this.currentFont, new String(this._currentContent.ToArray()), this.position, Color.White);
            this._spriteBatch.End();
        }

        protected virtual void RaiseValueChanged() => OnValueChanged?.Invoke(this, new EventArgs());


    }
}
