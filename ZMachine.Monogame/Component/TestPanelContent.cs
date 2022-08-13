﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZMachine.Monogame.Extensions;

namespace ZMachine.Monogame.Component
{
    internal class TestPanelContent : DrawableGameComponent, IScrollablePanelContent
    {
        private SpriteBatch _sp;
        private Rectangle ContentRect;
        private Vector2 offSet;
        private Texture2D[] textures;
        public TestPanelContent(Game game) : base(game)
        {
            this._sp = new SpriteBatch(game.GraphicsDevice);

            var textRect = new Rectangle(0, 0, 1, 1);
            this.textures = new[]
            {
              _sp.CreateFilledRectTexture(textRect, Color.Brown),
            //_sp.CreateFilledRectTexture(textRect, Color.Peru),
            //_sp.CreateFilledRectTexture(textRect, Color.PaleGoldenrod),
            _sp.CreateFilledRectTexture(textRect, Color.Azure),
            //_sp.CreateFilledRectTexture(textRect, Color.Red),
            //_sp.CreateFilledRectTexture(textRect, Color.Plum)

              };

            this.ContentRect = new Rectangle(0, 0, 500, 300 * textures.Length);
            this.offSet = new Vector2(0, 0);
        }


        public Rectangle ContentDimensions() => this.ContentRect;

        public void DrawPanel(GameTime time)
        {
            var height = 200;
            var pos = new Rectangle(0, 0 + (int)offSet.Y, 500, height);

            for (var x = 0; x < this.textures.Length; ++x)
            {
                var t = textures[x];
                _sp.Draw(t, pos, Color.White);
                pos = new Rectangle(0, pos.Y + height, 500, height);
            }
        }

        public void SetSpriteBatch(SpriteBatch spritebatch)
        {
            this._sp = spritebatch;
        }
    }
}
