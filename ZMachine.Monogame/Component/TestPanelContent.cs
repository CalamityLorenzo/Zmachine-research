using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using ZMachine.Monogame.Extensions;

namespace ZMachine.Monogame.Component
{
    internal class TestPanelContent : DrawableGameComponent, IScrollablePanelContent
    {
        private SpriteBatch _sp;
        private Rectangle ContentRect;
        private Vector2 offSet;
        private Texture2D[] textures;
        private Texture2D largeText;
        private int sectionHeight;

        public TestPanelContent(Game game) : base(game)
        {
            this._sp = new SpriteBatch(game.GraphicsDevice);

            var textRect = new Rectangle(0, 0, 1, 1);
            this.textures = Enumerable.Empty<Texture2D>().ToArray();

            this.largeText =Texture2D.FromFile(this.GraphicsDevice, "Content\\cupcake.jpg");

            //this.textures = new[]
            //{
            //    _sp.CreateFilledRectTexture(textRect, Color.Brown),
            //    _sp.CreateFilledRectTexture(textRect, Color.Peru),
            //    _sp.CreateFilledRectTexture(textRect, Color.PaleGoldenrod),
            //    _sp.CreateFilledRectTexture(textRect, Color.Azure),
            //    _sp.CreateFilledRectTexture(textRect, Color.Red),
            //    _sp.CreateFilledRectTexture(textRect, Color.Plum)

            //  };

            this.sectionHeight = 200;

            this.ContentRect = new Rectangle(0, 0, 500, sectionHeight * textures.Length);
            this.offSet = new Vector2(0, 0);
        }


        public Rectangle ContentDimensions() => this.largeText.Bounds;

        public void SetVerticalOffset(float y)
        {
            this.offSet.Y = y;
        }

        public void DrawPanel(GameTime time)
        {
            var pos = new Rectangle(0, 0 + (int)offSet.Y, largeText.Width, largeText.Height);
            _sp.Draw(this.largeText, largeText.Bounds, pos, Color.White);
            //for (var x = 0; x < this.textures.Length; ++x)
            //{
            //    var t = textures[x];
            //    _sp.Draw(t, pos, Color.White);
            //    pos = new Rectangle(0, pos.Y + sectionHeight, 500, sectionHeight);
            //}
        }

        public void SetSpriteBatch(SpriteBatch spritebatch)
        {
            this._sp = spritebatch;
        }
    }
}
