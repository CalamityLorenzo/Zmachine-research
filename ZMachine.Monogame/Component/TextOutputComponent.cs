using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Text;

namespace ZMachine.Monogame.Component
{
    internal class TextOutputComponent : DrawableGameComponent
    {
        private readonly SpriteBatch batch;
        private readonly SpriteFont font;
        private readonly Stream output;
        private string textStream;

        public TextOutputComponent(Game game, SpriteBatch batch, SpriteFont font, Stream output) : base(game)
        {
            this.batch = batch;
            this.font = font;
            this.output = output;
            textStream = "";
        }
        public override void Update(GameTime gameTime)
        {

            // TODO : THis is all very unsatisfactory.
            if (output.Length > 0)
            {
                output.Position = 0;
                using StreamReader sr = new StreamReader(output, Encoding.UTF8, bufferSize: (int)output.Length, leaveOpen: true);
                var theChars = new char[output.Length];
                Span<char> sp = theChars;
                sr.ReadBlock(sp);
                sr.Close();
                textStream += new string(sp).Replace("\0", string.Empty);
                output.SetLength(0);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            batch.DrawString(font, textStream, new Vector2(20, 20), Color.White);
        }
    }
}
