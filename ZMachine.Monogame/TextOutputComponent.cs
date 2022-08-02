using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

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
                var byteBuffer = new byte[output.Length];
                output.Position = 0;
                output.Read(byteBuffer, 0, byteBuffer.Length);
                output.Flush();

                this.textStream = System.Text.Encoding.UTF8.GetString(byteBuffer);
                output.SetLength(0);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            this.batch.DrawString(this.font, this.textStream, new Vector2(20, 20), Color.White);
        }
    }
}
