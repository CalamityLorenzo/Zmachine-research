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
        private int _cursorPosition;
        private List<char> _currentContent;
        private string _currentLine;
        private readonly SpriteFont currentFont;
        private readonly Stream inputStream;
        private readonly Stream ouputStream;

        public string Value { get => new String(this._currentContent.ToArray()); }
        public event Action<object, EventArgs> OnValueChanged;
        public event Action<object, LineAddedEventArgs> OnLineAdded;
        public TextControl(Game game, SpriteFont fnt, Stream inputStream, Stream ouputStream) : base(game)
        {
            this.currentFont = fnt;
            this.inputStream = inputStream;
            this.ouputStream = ouputStream;
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
                    }else if (currentChar == '\b') // Backspace.
                    {
                        this._currentContent.RemoveAt(_currentContent.Count);
                    }
                    else if (currentChar == (char)Keys.Escape)
                    {
                        this._currentContent = new List<char>();
                    }else if (currentChar == (byte)Keys.Enter)
                    {
                        var outputContent = System.Text.Encoding.UTF8.GetBytes(_currentContent.ToArray());
                        this.ouputStream.Write(outputContent, 0, outputContent.Length);
                    }
                    else
                    {
                        this._currentContent.Add(currentChar);
                        this.RaiseValueChanged();
                    }
                }

                inputStream.SetLength(0);
            }
        }

        private void ChangeCursorPosition(int cursorChange)
        {
            this._cursorPosition += cursorChange;
            if (_cursorPosition < 0) this._cursorPosition = 0;
            if (_cursorPosition > this._currentLine.Length + 1) this._cursorPosition = _currentLine.Length + 1;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected virtual void RaiseValueChanged() => OnValueChanged?.Invoke(this, new EventArgs());
        protected virtual void RaiseLineAdded(string newLine) => OnLineAdded?.Invoke(this, new LineAddedEventArgs(newLine));


    }
}
