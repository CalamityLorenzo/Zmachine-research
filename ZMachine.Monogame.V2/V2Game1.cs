
using ZMachine.Monogame.Component;
using ZMachine.Monogame.Components.TextComponents;
using ZMachine.Monogame.Components;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Color = Microsoft.Xna.Framework.Color;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Zmachine.Library.V2.Implementation;
using Zmachine.Library.V2;
using System.Drawing;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System;

namespace ZMachine.Monogame.V2
{
    public class V2Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private StreamInputProcessor sip;
        private ScrollablePanel hostPanel;
        private TextControl tc;
        private MemoryStream input0 = new(), input1 = new(), outputScreen = new(), outputTranscript = new();
        // This is the abstraction from the keyboard input into the input streams
        private MemoryStream kboardStream = new();

        private Machine zMachine;
        private ZmachineTools zMachineTools;

        public V2Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.Window.ClientSizeChanged += Window_ClientSizeChanged;
            this.Window.AllowUserResizing = true;
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

            this.hostPanel.UpdateDisplayArea(new Rectangle(0, 0, this._graphics.PreferredBackBufferWidth, this._graphics.PreferredBackBufferHeight));
        }

        protected override void Initialize()
        {



            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var arial = Content.Load<SpriteFont>("Arial");
            var cascade = Content.Load<SpriteFont>("Cascadia");
            var cbm128 = Content.Load<SpriteFont>("cbm128");


            var filename = "Curses\\curses.z5";
            filename = "hollywoo.dat";
            using var StoryData = File.Open(filename, FileMode.Open);
            var sd = new byte[StoryData.Length];
            Span<byte> spSpan = sd;
            StoryData.Read(spSpan);

            this.sip = new(this, kboardStream);

            var screenOutput = new ZMachineScreenOutput(this, cbm128, new Color(139, 243, 236, 255), Color.Black, new Vector2(10, 10), input0, outputScreen, kboardStream);

            hostPanel = new ScrollablePanel(this, true, new Rectangle(0, 0, this._graphics.PreferredBackBufferWidth, this._graphics.PreferredBackBufferHeight));
            //     this._testContent = new TestPanelContentextt(this);
            hostPanel.AddContent(screenOutput);


            zMachine = new Machine(input0, input1, outputScreen, outputTranscript, spSpan.ToArray());
            this.zMachineTools = new ZmachineTools(zMachine);


            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //zMachine.Update();
            zMachineTools.Step();

            this.hostPanel.Update(gameTime);

            // TODO: Add your update logic here
            this.sip.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            this.hostPanel.Draw(gameTime);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}