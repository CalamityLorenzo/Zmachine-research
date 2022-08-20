﻿using System.Text;
using ZMachine.Monogame.Extensions;

namespace ZMachine.Monogame.Component
{
    /// <summary>
    /// This is effectively the screen envirtonment for the games.
    /// And because of that It's hella complicated.
    /// </summary>
    public class ZMachineScreenOutput : DrawableGameComponent, IScrollablePanelContent
    {
        private SpriteBatch batch;
        private readonly SpriteFont font;
        private readonly Color foreground;
        private readonly Color background;
        private readonly Stream output;

        private List<Tuple<float, string>> history =
                                           new List<Tuple<float, string>>(); // Everything displayed until now.
        private string currentLine;                        // The line last loaded from the stream
        private string[] displayLines;                     // What gets displayed this time.
        private Vector2 currentDrawingPosition;
        private Texture2D backgroundDisplay;
        private Texture2D reverseBackground;
        private float HorizontalStart = 0;
        private float RowHeight = 0;

        private Vector2 OffSet = Vector2.Zero;
        private string statusLine;

        public ZMachineScreenOutput(Game game, SpriteBatch batch, SpriteFont font, Color foreground, Color background, Vector2 startPosition, Stream output) : base(game)
        {
            this.batch = batch;
            this.font = font;
            this.foreground = foreground;
            this.background = background;
            
            
            // This is actually our input on the application output stream.
            this.output = output;
            currentLine = "";
            this.currentDrawingPosition = startPosition;

            this.HorizontalStart = startPosition.X;
            this.RowHeight = font.MeasureString("W").Y + 2;
            this.backgroundDisplay = this.batch.CreateFilledRectTexture(this.ContentDimensions(), this.background);
            this.reverseBackground = this.batch.CreateFilledRectTexture(this.ContentDimensions(), this.foreground);
        }

        public override void Update(GameTime gameTime)
        {
            // TODO : THis is all very unsatisfactory.
            if (output.Length > 0)
            {
                currentLine = "";
                output.Position = 0;
                using StreamReader sr = new StreamReader(output, Encoding.UTF8, bufferSize: (int)output.Length, leaveOpen: true);
                var theChars = new char[output.Length];
                Span<char> sp = theChars;
                sr.Read(sp);
                sr.Close();

                // That's our line of text that we are going to draw.
                // we have no knowledge whats in that text. at all.
                var streamLine = new string(sp).Replace("\0", string.Empty);
               
                // Can't think of a neater way to write to the status line with out adding machinery.
                // So instead, i'm now embedding cmds in a raw stream...Fucking genius is what your thinking...No?...
                var statusLineCmd = "@@STATUS_LINE@@:";
                if (streamLine.StartsWith(statusLineCmd))
                {
                    this.statusLine = streamLine.Substring(statusLineCmd.Length);
                }
                else // normal stream text.
                {
                    // need to discover the newlines, and split out accordingly.
                    displayLines = streamLine.Split(new char[] { '\r' });
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
                }
                output.SetLength(0);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            this.DrawPanel(gameTime);
        }

        public void SetSpriteBatch(SpriteBatch spritebatch)
        {
            this.batch = spritebatch;
        }
        // +3 =1 Status line 2 lines after prompt
        public Rectangle ContentDimensions() => new Rectangle(0, 0, 500, ((int)this.RowHeight * (this.history.Count + 3)));

        public void DrawPanel(GameTime time)
        {

            var row = (this).RowHeight;
            batch.Draw(reverseBackground,new Vector2(0,0), new Rectangle(0,0, this.ContentDimensions().Width, (int)row-(int)OffSet.Y), Color.White);
            if(this.statusLine is not null)
                batch.DrawString(font, statusLine, new Vector2(HorizontalStart, 0) - this.OffSet, background);

            row += font.MeasureString(statusLine).Y;
            
            // Colour in the background 
            foreach (var lineData in history)
            {
                var line = lineData.Item2;
                batch.DrawString(font, line, new Vector2(HorizontalStart, lineData.Item1+row) -  this.OffSet, Color.White);
            }

            if (this.currentLine.Length > 0)
            {
                var currentLineWidth = font.MeasureString(currentLine);
                batch.DrawString(font, currentLine, currentDrawingPosition - this.OffSet, Color.White);
            }

        }

        public void SetVerticalOffset(float y)
        {
            this.OffSet.Y = y;
        }
    }
}