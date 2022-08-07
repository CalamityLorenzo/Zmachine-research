﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZMachine.Monogame.Component
{
    internal class TextOutputComponent : DrawableGameComponent
    {
        private readonly SpriteBatch batch;
        private readonly SpriteFont font;
        private readonly Stream output;

        private List<Tuple<float, string>> history =
                                           new List<Tuple<float, string>>(); // Everything displayed until now.
        private string currentLine;                        // The line last loaded from the stream
        private string[] displayLines;                     // What gets displayed this time.
        private Vector2 currentDrawingPosition;

        private float HorizontalStart = 0;
        private float RowHeight = 0;
        public TextOutputComponent(Game game, SpriteBatch batch, SpriteFont font, Vector2 startPosition, Stream output) : base(game)
        {
            this.batch = batch;
            this.font = font;
            this.output = output;
            currentLine = "";
            this.currentDrawingPosition = startPosition;

            this.HorizontalStart = startPosition.X;
            this.RowHeight = font.MeasureString("W").Y + 2;

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
                output.SetLength(0);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var lineData in history)
            {
                var line = lineData.Item2;
                batch.DrawString(font, line, new Vector2(HorizontalStart, lineData.Item1), Color.White);
            }

            if (this.currentLine.Length > 0)
            {
                batch.DrawString(font, currentLine, currentDrawingPosition, Color.White);
            }

        }
    }
}