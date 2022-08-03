using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Text;

namespace ZMachine.Monogame
{
    internal class TextOutputComponent : DrawableGameComponent
    {
        private readonly SpriteBatch batch;
        private readonly SpriteFont font;
        private readonly Stream output;
        private string textStream;

        public TextOutputComponent(Game game, SpriteBatch batch, SpriteFont font, Stream output ) : base(game)
        {
            this.batch = batch;
            this.font = font;
            this.output = output;
            this.textStream = "";
        }
        public override void Update(GameTime gameTime)
        {
            if (this.output.Length > 0)
            {
                this.output.Position = 0;

                using StreamReader sr = new StreamReader(this.output, Encoding.UTF8, bufferSize: (int)this.output.Length, leaveOpen:true);
                //var theChars = new char[this.output.Length];
                //Span<Char> sp = theChars;
                //sr.ReadBlock(sp);
                //sr.Close();
                //output.SetLength(0);  
                //this.textStream = new string(sp);
                this.textStream = sr.ReadLine();
                sr.Close();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            this.batch.DrawString(this.font, this.textStream, new Vector2(20, 20), Color.White);
        }
    }
}
