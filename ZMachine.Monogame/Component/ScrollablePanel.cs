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
                float factor = (float)ContentDimensions.Height >= DisplayArea.Height ? (float)ContentDimensions.Height / DisplayArea.Height : 1;
                vScrollbarDimensions = new(DisplayArea.X + DisplayArea.Width - 20, DisplayArea.Y, 22, DisplayArea.Height);
                vScrollbarNubDimensons = new(DisplayArea.X + DisplayArea.Width - 20, DisplayArea.Y, 22, 22);
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
                }
                //if (contentDimensions != this.DisplayArea) UpdateDimensions(contentDimensions);
                if (contentDimensions.Height < this.DisplayArea.Height && disableVerticalScroll == false)
                    disableVerticalScroll = true;
                else if (contentDimensions.Height > this.DisplayArea.Height && disableVerticalScroll == true)
                    disableVerticalScroll = false;
            }

            var mState = Mouse.GetState();

            MousePosition(mState);

        }

        private void MousePosition(MouseState mState)
        {
            var mouseRect = new Rectangle(mState.X, mState.Y, 1, 1);
            if (mState.LeftButton == ButtonState.Pressed && !lButtonPressed)
            {
                lButtonPressed = true;
                if (vScrollbarDimensions.Intersects(mouseRect))
                {
                    var direction = UpdateVerticalScrollPosition(mState.X, mState.Y);
                    // up = head to the negatives
                    // down increate the vertical
                    var dimensions = this.Content.ContentDimensions();
                    var factor = (float)dimensions.Height / dimensions.Height;
                    //this.Content.Off  

                }

            }

            if (mState.LeftButton == ButtonState.Pressed && lButtonPressed && vScrollbarNubDimensons.Intersects(mouseRect))
                ScrollVerticalScrollBar(new Vector2(mState.X, mState.Y));



            if (mState.LeftButton == ButtonState.Released && lButtonPressed)
            {
                previousMouse = Vector2.Zero;
                lButtonPressed = false;
            }
        }

        private void ScrollVerticalScrollBar(Vector2 currentMousePos)
        {
            if (previousMouse == Vector2.Zero)
            {
                previousMouse = currentMousePos;
                Debug.WriteLine("WOT");
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

                this.vScrollbarNubDimensons.Y += nubPos;
                Debug.WriteLine(vScrollbarNubDimensons.Y);


                if (this.vScrollbarNubDimensons.Y < this.vScrollbarDimensions.Y)
                    this.vScrollbarNubDimensons.Y = 0;
                if (this.vScrollbarNubDimensons.Y + this.vScrollbarNubDimensons.Height > this.vScrollbarDimensions.Y + this.vScrollbarDimensions.Height)
                    this.vScrollbarNubDimensons.Y = (this.vScrollbarDimensions.Y + this.vScrollbarNubDimensons.Height) - this.vScrollbarNubDimensons.Height;
                previousMouse = currentMousePos;
            }


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

        internal ScrollDirection UpdateVerticalScrollPosition(int x, int y)
        {
            // If we have clicked the nubn eject.
            if (new Rectangle(x, y, 1, 1).Intersects(vScrollbarNubDimensons)) return ScrollDirection.Unknown;
            // X and Y have beenc lieced.
            // do we move the button up or down?
            ScrollDirection sd = ScrollDirection.Unknown;
            // Stepping distnace
            var pages = (float)ContentDimensions.Height / DisplayArea.Height;
            var distanceToMove = DisplayArea.Height / pages;
            // remove the screen offset.
            var screenOffSet = vScrollbarDimensions.Y;
            float newContentYPos = 0;


            // Clicked after the halfway point of  the button, so nub scrolls down and content scroll up
            if (y >= vScrollbarNubDimensons.Y + vScrollbarNubDimensons.Height)
            {
                if (vScrollbarNubDimensons.Y + vScrollbarNubDimensons.Height == vScrollbarDimensions.Height) return ScrollDirection.Unknown;

                // Is the total height of the nub still in bounds?cva
                if (vScrollbarNubDimensons.Y + vScrollbarNubDimensons.Height < vScrollbarDimensions.Height)
                {
                    //vScrollbarNubDimensons.Y += (int)distance;
                    var newPos = vScrollbarNubDimensons.Y + (int)distanceToMove;
                    newContentYPos = (newPos - screenOffSet) * pages;

                    // if the height of the nub is outside the bounds of the gutter move itback in.
                    if (newPos + vScrollbarNubDimensons.Height > vScrollbarDimensions.Height)
                        vScrollbarNubDimensons.Y = (vScrollbarDimensions.Y + vScrollbarDimensions.Height) - vScrollbarNubDimensons.Height;
                    else
                        vScrollbarNubDimensons.Y = newPos;
                    sd = ScrollDirection.Up;
                }
            }
            else // nub moves up, content scrolls down
            {
                var newPos = vScrollbarNubDimensons.Y - (int)distanceToMove;
                newContentYPos = (newPos - screenOffSet) * pages;
                //newContentYPos = newPos * factor;
                if (newPos < vScrollbarDimensions.Y) newPos = vScrollbarDimensions.Y;
                vScrollbarNubDimensons.Y = newPos;
                sd = ScrollDirection.Down;

            }
            this.Content.SetVerticalOffset(newContentYPos);
            return sd;
        }
    }
}