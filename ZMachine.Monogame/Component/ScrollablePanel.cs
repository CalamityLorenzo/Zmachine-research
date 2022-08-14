using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace ZMachine.Monogame.Component
{
    public interface IScrollablePanelContent : IDrawable
    {
        void SetSpriteBatch(SpriteBatch spritebatch);
        Rectangle ContentDimensions();
        void DrawPanel(GameTime time);
        void SetVerticalOffset(float y);
    }

    public enum ScrollDirection
    {
        Unknown = 0,
        Up,
        Down,
        Left,
        Right
    }
    public class ScrollablePanel
    {
        private readonly SpriteBatch sb;
        // This is our scissor rect
        public Rectangle DisplayArea { get; private set; } = Rectangle.Empty;
        // The total area of the content to be displayed.
        // Used to calculate the scollbar button size.
        private Rectangle ContentDimensions = Rectangle.Empty;
        public Rectangle vScrollbarDimensions { get; private set; } = Rectangle.Empty;
        private Rectangle vScrollbarNubDimensons = Rectangle.Empty;

        private Vector2 previousMouse = Vector2.Zero;

        public bool VerticalScrollbar { get; }
        public IScrollablePanelContent Content { get; private set; }
        public Rectangle TotalArea { get; private set; }

        private RasterizerState rasterState;
        private Texture2D scrollbarTexture;
        private Texture2D disabledScrollbarTexture;
        private Texture2D scrollbarNubTexture;
        private bool lButtonPressed;
        private bool disableVerticalScroll;
        private int currentScrollHeight;

        public ScrollablePanel(Game game, SpriteBatch sb, bool verticalScrollbar, Rectangle startingDimensions)
        {

            this.sb = new SpriteBatch(sb.GraphicsDevice);
            VerticalScrollbar = verticalScrollbar;

            // This is the panel content area
            UpdateDisplayArea(startingDimensions);

            // Scrollbar and butotn
            scrollbarTexture = new Texture2D(this.sb.GraphicsDevice, 1, 1);
            disabledScrollbarTexture = new Texture2D(this.sb.GraphicsDevice, 1, 1);
            scrollbarNubTexture = new Texture2D(this.sb.GraphicsDevice, 1, 1);
            disabledScrollbarTexture.SetData<Color>(Enumerable.Range(0, 1).Select(a => Color.Beige).ToArray());
            scrollbarTexture.SetData<Color>(Enumerable.Range(0, 1).Select(a => Color.Gray).ToArray());
            scrollbarNubTexture.SetData<Color>(Enumerable.Range(0, 1).Select(a => Color.DarkGray).ToArray());

            // Rasterizer State (Do we actually need this?)
            rasterState = new RasterizerState();
            rasterState.MultiSampleAntiAlias = true;
            rasterState.ScissorTestEnable = true;
            rasterState.FillMode = FillMode.Solid;
            rasterState.CullMode = CullMode.CullCounterClockwiseFace;
            rasterState.DepthBias = 0;
            rasterState.SlopeScaleDepthBias = 0;
            UpdateTotalArea();
        }

        // The display area combined with the vertical scrollbar.
        private void UpdateTotalArea()
        {
            TotalArea = new Rectangle(DisplayArea.X, DisplayArea.Y, DisplayArea.Width + vScrollbarDimensions.X, DisplayArea.Height);
        }

        public void UpdateDisplayArea(Rectangle dimensions)
        {
            if (DisplayArea != dimensions)
            {
                DisplayArea = dimensions;
                FormatScrollbar();
                UpdateTotalArea();
            }
        }

        public void AddContent(IScrollablePanelContent content)
        {
            Content = content;
            Content.SetSpriteBatch(sb);
            // The full size of the content
            ContentDimensions = Content.ContentDimensions();
            FormatScrollbar();
        }
        // Set the size of the scrollbar button in relation to the content being displayed
        private void FormatScrollbar()
        {

            if (VerticalScrollbar)
            {
                var pages = (float)ContentDimensions.Height / DisplayArea.Height;
                var nubHeight = pages == 0 ? vScrollbarDimensions.Height / 1 : vScrollbarDimensions.Height / pages;
                if (nubHeight > vScrollbarDimensions.Height) nubHeight = vScrollbarDimensions.Height;
                //float factor = (float)ContentDimensions.Height >= DisplayArea.Height ? (float)ContentDimensions.Height / DisplayArea.Height : 1;
                vScrollbarDimensions = new(DisplayArea.X + DisplayArea.Width - 20, DisplayArea.Y, 22, DisplayArea.Height);
                vScrollbarNubDimensons = new(DisplayArea.X + DisplayArea.Width - 20, DisplayArea.Y, 22, (int)nubHeight);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Content != null)
            {
                Rectangle contentDimensions = Content.ContentDimensions();
                if (ContentDimensions != contentDimensions)
                {
                    // well do something!
                    ContentDimensions = contentDimensions;
                    FormatScrollbar();
                    SetVerticalScrollbarRelativePosition(this.vScrollbarDimensions.Y + this.vScrollbarDimensions.Height);
                }
                //if (contentDimensions != this.DisplayArea) UpdateDimensions(contentDimensions);
                if (contentDimensions.Height < this.DisplayArea.Height && disableVerticalScroll == false)
                    disableVerticalScroll = true;
                else if (contentDimensions.Height > this.DisplayArea.Height && disableVerticalScroll == true)
                    disableVerticalScroll = false;
                var mState = Mouse.GetState();
                MousePosition(mState);
            }

        }

        private void MousePosition(MouseState mState)
        {
            var mouseRect = new Rectangle(mState.X, mState.Y, 1, 1);
            if (mState.LeftButton == ButtonState.Pressed && !lButtonPressed)
            {
                lButtonPressed = true;
                if (vScrollbarDimensions.Intersects(mouseRect))
                {
                    ClickVerticalScrollPosition(mState.X, mState.Y);
                }
            }

            if (mState.LeftButton == ButtonState.Pressed && lButtonPressed && vScrollbarNubDimensons.Intersects(mouseRect))
                HoldVerticalScrollPosition(new Vector2(mState.X, mState.Y));

            if (mState.LeftButton == ButtonState.Released && lButtonPressed)
            {
                previousMouse = Vector2.Zero;
                lButtonPressed = false;
            }
        }

        // When you click and hold on the nub. you can move the nub, up and down
        // Appesr to scroll that way.
        private void HoldVerticalScrollPosition(Vector2 currentMousePos)
        {
            if (previousMouse == Vector2.Zero)
            {
                previousMouse = currentMousePos;
                return;
            }
            if (currentMousePos == previousMouse)
                return;
            else if (previousMouse != currentMousePos)
            {
                // work out the different and move the nubbin the same distance.
                // Move nubbin up, (so scroll down)]
                var nubPos = 0;
                if (previousMouse.Y > currentMousePos.Y)
                {
                    nubPos = -(int)(previousMouse.Y - currentMousePos.Y);
                }
                else if (previousMouse.Y < currentMousePos.Y)
                {
                    nubPos = (int)(currentMousePos.Y - previousMouse.Y);
                }
                SetVerticalScrollbarRelativePosition(nubPos);
                // Set the position.
                previousMouse = currentMousePos;
            }


        }

        // When you click the gutter onthe scroll bar how it moves.
        // we are trying to page from one section to the next.
        // If you click the nub it returns.
        internal void ClickVerticalScrollPosition(int x, int y)
        {
            // If we have clicked the nubn eject.
            if (new Rectangle(x, y, 1, 1).Intersects(vScrollbarNubDimensons)) return;
            // X and Y have beenc clicked.
            // do we move the button up or down?
            // Stepping distnace
            var pages = (float)ContentDimensions.Height / DisplayArea.Height;
            // Effectively we are asking is the positive or negative?
            var distanceToMove = DisplayArea.Height / pages;

            var newPos = 0;
            // Clicked after the halfway point of  the button, so nub scrolls down and content scroll up
            if (y >= vScrollbarNubDimensons.Y + vScrollbarNubDimensons.Height)
            {
                if (vScrollbarNubDimensons.Y + vScrollbarNubDimensons.Height < vScrollbarDimensions.Height)
                {
                    newPos = (int)distanceToMove;
                }
            }
            else // nub moves up, content scrolls down
            {
                newPos = -(int)distanceToMove;
            }
            this.SetVerticalScrollbarRelativePosition(newPos);
        }

        /// <summary>
        /// Change the position of the nub realtive to existing position.
        /// </summary>
        /// <param name="relativeNubY"></param>
        private void SetVerticalScrollbarRelativePosition(int relativeNubY)
        {
            var newPos = this.vScrollbarNubDimensons.Y + relativeNubY;
            this.SetVerticalScrollbarAbsolutePosition(newPos);
        }

        private void SetVerticalScrollbarAbsolutePosition(int NubY)
        {
            this.vScrollbarNubDimensons.Y = NubY;
            // Above the height of the gutter.
            if (this.vScrollbarNubDimensons.Y < this.vScrollbarDimensions.Y)
                this.vScrollbarNubDimensons.Y = this.vScrollbarDimensions.Y;
            // Bottom of the nub is below the gutter end.
            if (this.vScrollbarNubDimensons.Y + this.vScrollbarNubDimensons.Height > this.vScrollbarDimensions.Y + this.vScrollbarDimensions.Height)
                this.vScrollbarNubDimensons.Y = (this.vScrollbarDimensions.Y + this.vScrollbarDimensions.Height) - this.vScrollbarNubDimensons.Height;

            // Now based on that position, set the content offset.
            var actualPosition = vScrollbarNubDimensons.Y - vScrollbarDimensions.Y;
            var pages = (float)ContentDimensions.Height / DisplayArea.Height;
            this.Content.SetVerticalOffset(actualPosition * pages);
        }

        public void Draw(GameTime gameTime)
        {
            sb.Begin(SpriteSortMode.Deferred, null, null, null, rasterState, null, null);
            sb.GraphicsDevice.ScissorRectangle = DisplayArea;
            // Draw content here...Maybe?
            // this.sb.Draw(this.scrollbarTexture, this.ContentArea, null, Color.Aqua);
            if (Content != null && Content.Visible)
            {
                Content.DrawPanel(gameTime);
                // Draw scrollbar here
                if (VerticalScrollbar)
                {

                    // this.sb.Draw(this.scrollbarTexture, new Vector2(this.DisplayArea.X + this.DisplayArea.Width - 20, this.DisplayArea.Y), new Rectangle(0, 0, 20, (int)this.DisplayArea.Height), Color.White);
                    sb.Draw(disableVerticalScroll ? disabledScrollbarTexture : scrollbarTexture, new Vector2(vScrollbarDimensions.X, vScrollbarDimensions.Y), new Rectangle(0, 0, 20, DisplayArea.Height), Color.White);
                    sb.Draw(scrollbarNubTexture, new Vector2(vScrollbarNubDimensons.X, vScrollbarNubDimensons.Y), new Rectangle(0, 0, vScrollbarNubDimensons.Width, vScrollbarNubDimensons.Height), Color.White);
                }
            }
            sb.End();
        }

    }
}