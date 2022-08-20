using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMachine.Monogame.Components.TextComponents
{
    internal class BasicBlinkCursor : DrawableGameComponent
    {
        Texture2D? cursorTexture;
        private readonly SpriteBatch sb;
        private readonly float blinkRate;
        private readonly SpriteFont fnt;
        private readonly Color cursorColour;
        /// <summary>
        /// All actions on moving the cursor are to relative to this point.
        /// </summary>
        private Vector2 anchorPosition;
        private readonly Vector2 inputCharSize;
        private string cursor;
        private Vector2 fntSize;
        private bool shouldDisplay; // True== display, False==blink
        private double currentTimer = 0;
        private int numberOfChars = 0;

        public BasicBlinkCursor(Game game, SpriteBatch sb, float blinkRate, SpriteFont fnt, Color cursorColour,  Vector2 startPosition, Vector2 inputCharSize,  string cursor) : base(game)
        {
            this.cursorTexture = null;
            this.sb = sb;
            this.blinkRate = blinkRate;
            this.fnt = fnt;
            this.cursorColour = cursorColour;
            this.anchorPosition = startPosition;
            this.inputCharSize = inputCharSize;
            this.cursor = cursor;
            this.fntSize = fnt.MeasureString(cursor);
        }

        /// <summary>
        /// This is added to the current position, it's all relative baby!
        /// </summary>
        /// <param name="updatePosition"></param>
        public void MoveCursorPosition(int numberOfChars)
        {
            this.numberOfChars = numberOfChars;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var elaspedLastUpdate = gameTime.ElapsedGameTime.TotalMilliseconds;
            currentTimer += elaspedLastUpdate;
            if(currentTimer> blinkRate)
            {
                currentTimer= 0;
                this.shouldDisplay = !shouldDisplay;
            }


        }

        public void UpdateAnchor(Vector2 newAnchor)
        {
            if (newAnchor != anchorPosition) anchorPosition = newAnchor;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (this.shouldDisplay)
            {
                if (this.cursorTexture == null)
                    sb.DrawString(fnt, this.cursor, new Vector2(this.anchorPosition.X + (fntSize.X*numberOfChars), this.anchorPosition.Y), this.cursorColour);
            }
                

        }
    }
}
