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
        private readonly SpriteFont currentFont;
        private readonly Stream inputStream;

        public string Value { get; private set; }

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
            throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
