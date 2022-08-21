 using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMachine.Monogame.Component;
using ZMachine.Monogame.Components;
using ZMachine.Monogame.Components.TextComponents;

namespace ZMachine.Monogame
{
    public class TestGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Stream outputStream, inputStream;
        private ZMachineScreenOutput screenOutput;
        private StreamInputProcessor sic;
        private ScrollablePanel hostPanel;
        private TextControl tc;
        private bool tp =true;


        public TestGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            bool viewUpdate = false;
            if (this._graphics.PreferredBackBufferWidth != _graphics.GraphicsDevice.Viewport.Width)
            {
                this._graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.Viewport.Width;
                viewUpdate = true;
            }

            if (this._graphics.PreferredBackBufferHeight != _graphics.GraphicsDevice.Viewport.Height)
            {
                this._graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.Viewport.Height;
                viewUpdate = true;
            }

            if (viewUpdate)
                this._graphics.ApplyChanges();

            this.hostPanel.UpdateDisplayArea(new Rectangle(0,0, this._graphics.PreferredBackBufferWidth, this._graphics.PreferredBackBufferHeight));
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var arial = Content.Load<SpriteFont>("Arial");
            var cascade = Content.Load<SpriteFont>("Cascadia");
            var cbm128 = Content.Load<SpriteFont>("cbm128");

            outputStream = new MemoryStream();
            inputStream = new MemoryStream();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            sic = new(this, inputStream);
            //tc = new(this, cascade, inputStream, outputStream, new Vector2(10, 60));
            screenOutput = new(this,  cbm128, new Color(139, 243, 236,255), Color.Black, new Vector2(10, 10), inputStream, outputStream, new MemoryStream());

            hostPanel = new ScrollablePanel(this, true, new(0, 0, this._graphics.PreferredBackBufferWidth, this._graphics.PreferredBackBufferHeight));
            hostPanel.AddContent(screenOutput);
            // TODO: use this.Content to load your game content here
            var statusLineText = "@@STATUS_LINE@@:West of house";
            var byteArr = Encoding.UTF8.GetBytes(statusLineText);
            Span<byte> bytes = new Span<byte>(byteArr);
            //Convert.TryFromBase64String(statusLineText, bytes, out var byteWRite);
            outputStream.Write(bytes);

            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            sic.Update(gameTime);
            //tc.Update(gameTime);
            //screenOutput.Update(gameTime);
            hostPanel.Update(gameTime);
            if (tp == true)
            {
                var ff = new Span<byte>(Encoding.UTF8.GetBytes(TestContent.data[0]));
                outputStream.Write(ff);
                tp = false;
            }

            DoTheKeyboard(Keyboard.GetState());

            base.Update(gameTime);
        }

        private void DoTheKeyboard(KeyboardState kstate)
        {
            var keys = kstate.GetPressedKeys();
            var testData = TestContent.data;
            if (keys.Contains(Keys.LeftControl))
            {
                foreach(var key in keys)
                {
                    var keyCode = (int)key;
                    if (keyCode >= 48 && keyCode <= 57)
                    {
                        var idx = (int)key - 48;
                        if(testData.Count>= idx && idx < testData.Count  )
                        {
                            var byteArr = Encoding.UTF8.GetBytes(testData[idx]);
                            Span<byte> bytes = new Span<byte>(byteArr);
                            //Convert.TryFromBase64String(statusLineText, bytes, out var byteWRite);
                            outputStream.Write(bytes);
                        }
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            Matrix view = Matrix.Identity;


            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);

            // testShader.Parameters["crt_lottes"].SetValue(view * projection);

            this._spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //testShader.CurrentTechnique.Passes[0].Apply();
            // TODO: Add your drawing code here
            //tc.Draw(gameTime);
            //screenOutput.Draw(gameTime);
            hostPanel.Draw(gameTime);
            this._spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
