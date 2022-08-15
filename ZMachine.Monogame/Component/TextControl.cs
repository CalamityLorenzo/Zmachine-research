using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZMachine.Monogame.Extensions;

namespace ZMachine.Monogame.Component
{
    public class DEnnis : DrawableGameComponent
    {
        
        private readonly SpriteFont fnt;
        private TypeToStream textSource;
        private Vector2 caretSize;

        private Texture2D Cursor { get; }
        private int _cursorCharPosition;
        private Stream MemStream;
        private Vector2 startPos;
        private List<char> currentLine = new();
        private List<string> AllLines = new();

        private bool isEnterPressed;
        private bool isLeftArrowPressed;
        private bool isRightArrowPressed;
        private SpriteBatch _spriteBatch;

        public DEnnis(Game game, SpriteBatch _spriteBatch, SpriteFont fnt) : base(game)
        {
            textSource = new TypeToStream(game);
            this._spriteBatch = _spriteBatch;
            this.fnt = fnt;
            var fntWidth = fnt.MeasureString("0");
            this.caretSize = fnt.MeasureString(">");
            Cursor = _spriteBatch.CreateFilledRectTexture(new Rectangle(0, 0, (int)fntWidth.X, (int)fntWidth.Y), Color.White);
            _cursorCharPosition = 0;
            MemStream = new MemoryStream();
            this.Enabled = true;
            this.startPos = new Vector2(20, 20);
        }

        public DEnnis(Game game, SpriteBatch _spriteBatch, SpriteFont fnt, Stream writeableStream, Vector2 startPos) : this(game, _spriteBatch, fnt)
        {
            MemStream = writeableStream;
            this.startPos = startPos;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();

        }
        public override void Update(GameTime gameTime)
        {
            CheckTextBuffer();
            var kboard = Keyboard.GetState();
            CheckNonCharacterKeys(kboard);
            base.Update(gameTime);
        }

        private void CheckNonCharacterKeys(KeyboardState kboard)
        {
            if (kboard.IsKeyDown(Keys.Enter) && isEnterPressed == false)
            {
                isEnterPressed = true;
                AllLines.Add(String.Join("", currentLine.ToArray()) + "\n");
                WriteToOutputStream();
                currentLine.Clear();
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
                if (_cursorCharPosition == currentLine.Count) return;
                _cursorCharPosition += 1;
            }

            if (kboard.IsKeyUp(Keys.Enter) && isEnterPressed) isEnterPressed = false;
            if (kboard.IsKeyUp(Keys.Left) && isLeftArrowPressed) isLeftArrowPressed = false;
            if (kboard.IsKeyUp(Keys.Right) && isRightArrowPressed) isRightArrowPressed = false;
        }

        private void CheckTextBuffer()
        {
            if (textSource.TextStreamLength > 0)
            {
                var chars = textSource.GetCurrentCharacters();
                for (var x = 0; x < chars.Length; x++)
                {
                    var charA = (char)chars[x];
                    if (charA == '\b')
                    { // backspace
                        if (_cursorCharPosition != 0)
                        {
                            currentLine.RemoveAt(_cursorCharPosition - 1);
                            _cursorCharPosition -= 1;
                        }
                        else
                        {
                            if (currentLine.Count > 0)
                                currentLine.RemoveAt(_cursorCharPosition);
                        }

                    }
                    else if (charA == '\u007f')  // forward del
                    {
                        if (currentLine.Count > 0 && _cursorCharPosition < currentLine.Count)
                        {
                            currentLine.RemoveAt(_cursorCharPosition);
                        }
                    }
                    else if (charA == 27) // escape
                    {
                        currentLine = new List<char>();
                        _cursorCharPosition = 0;
                    }
                    else // Append character and move the cursor along.
                    {
                        currentLine.Insert(_cursorCharPosition, charA);
                        _cursorCharPosition += 1;
                    }
                }
            }
        }

        private void WriteToOutputStream()
        {
            // Nothing is known about the stream as the outside can manipulate it.
            // so just write, and move on.
            using StreamWriter sw = new StreamWriter(MemStream, Encoding.UTF8, bufferSize: (int)currentLine.Count, leaveOpen: true);
            sw.WriteLine(currentLine.ToArray(), 0, currentLine.Count);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            var cLine = String.Join("", currentLine);
            var cursorPoint = fnt.MeasureString(cLine.Substring(0, _cursorCharPosition));

            _spriteBatch.DrawString(fnt, ">", startPos, Color.White);
            var stringPosition = new Vector2(startPos.X + caretSize.Length(), startPos.Y);
            _spriteBatch.DrawString(fnt, cLine, stringPosition, Color.White);
            _spriteBatch.Draw(Cursor, new Vector2(stringPosition.X + cursorPoint.X, stringPosition.Y), Color.White);
            // If the cursor is over a character, we want to paint the 'reverese' colour of the character
            if (_cursorCharPosition != currentLine.Count){
                _spriteBatch.DrawString(fnt, cLine.Substring(_cursorCharPosition, 1), new Vector2(stringPosition.X + cursorPoint.X, stringPosition.Y), Color.Black);
            }
        }
    }
}
