using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMachine.Monogame.Components
{
    public class TextControl : DrawableGameComponent
    {
        private int _cursorPosition;
        private List<char> _currentContent;
        private string _currentLine;
        private readonly SpriteFont currentFont;
        private readonly Stream inputStream;

        public string Value { get; private set; }
        public event Action<object, EventArgs> OnValueChanged;
        public TextControl(Game game, SpriteFont fnt, Stream inputStream) : base(game)
        {
            this.currentFont = fnt;
            this.inputStream = inputStream;
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
            if(this.inputStream.Length> 0)
            {
                // Do stream things
                _currentLine = "";
                inputStream.Position = 0;
                using StreamReader sr = new StreamReader(inputStream, Encoding.UTF8, bufferSize: (int)inputStream.Length, leaveOpen: true);
                var theChars = new char[inputStream.Length];
                Span<char> sp = theChars;
                sr.Read(sp);
                sr.Close();
                // That's our line of text that we are going to draw.
                // we have no knowledge whats in that text. at all.
                var streamLine = new string(sp).Replace("\0", string.Empty);
                var displayLines = streamLine.Split(new char[] { '\r' });
                for (var x = 0; x < displayLines.Length; ++x)
                {
                    // These all had new lines, and so go to the history table.
                    if (x < displayLines.Length - 1)
                    {
                        this.history.Add(Tuple.Create(currentDrawingPosition.Y, displayLines[x]));
                        this.currentDrawingPosition.Y += RowHeight;
                    }
                    // must be the last entry (cos otherwise there would be a blank entry after)
                    if (x == displayLines.Length - 1 && displayLines[x].Length > 0)
                    {
                        this.currentLine = displayLines[x];
                    }
                }
                inputStream
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected virtual void RaiseValueChanged()=> OnValueChanged?.Invoke(this, new EventArgs());

    }
}
