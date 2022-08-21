using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using ZMachine.Monogame.Component;
using ZMachine.Monogame.Components.TextComponents;
using ZMachine.Monogame.Components;

namespace ZMachine.Monogame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont arial;
        private TextControl textControl;
        private TestPanelContent _testContent;
        private StreamInputProcessor sip;
        private ScrollablePanel hostPanel;
        private TextControl tc;

        private ZMachineGamee machineGame;

        private MemoryStream input0 = new(), input1 = new(), outputScreen = new(), outputTranscript = new();
        // This is the abstraction from the keyboard input into the input streams
        private MemoryStream kboardStream = new();


        public Game1()
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
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            var arial = Content.Load<SpriteFont>("Arial");
            var cascade = Content.Load<SpriteFont>("Cascadia");
            var cbm128 = Content.Load<SpriteFont>("cbm128");


            var filename = "Curses\\curses.z5";
            var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;

            this.sip = new(this, kboardStream);

            this.machineGame = new ZMachineGamee(input0, input1, outputScreen, outputTranscript, fileStream);
            var screenOutput = new ZMachineScreenOutput(this, cbm128, new Color(139, 243, 236, 255), Color.Black, new Vector2(10, 10), input0, outputScreen, kboardStream);

            hostPanel = new ScrollablePanel(this, true, new Rectangle(0, 0, this._graphics.PreferredBackBufferWidth-80, this._graphics.PreferredBackBufferHeight));
            //     this._testContent = new TestPanelContentextt(this);
            hostPanel.AddContent(screenOutput);
            // TODO: use this.Content to load your game content here
            var customProg = new byte[]
            {
                // Routine start
                0x8f, 00,1, // call_1n x x (4)
                0xb0,       // return true
                // Routine end\
                // Routine start
                3,
                0xb2, 18,42,103,0,25,41,3,20,73,64,79,82,29,87,96,180,148,229,  //Print a big fat string.
                0xbb,
                0xb2, 17,83,101,87,1,110,95,25,2,122,72,234,92,189,148,229,    // More groovy strings
                0xe4, 15, 0x5d, 0xd5, 0x5e, 0x4e, 0xff,
                0xb2, 17,52,79,32,122,154,3,45,58,112,3,45,42,234,3,13,83,81,36,7,40,18,82,234,2,139,3,45,27,37,212,167,
                0xb2, 18,70,120,234,20,229,28,153,53,87,40,8,83,81,36,7,40,18,82,234,2,139,3,45,59,5,84,167,0,0,0,0,0,0,58,120,101,70,36,166,15,197,24,64,23,165,24,36,20,197,12,166,11,197,156,165,
                0xbb,
                0xbb,
                0x8c, 255,119,
                0xb0,       // return true
                // routine end.
            };

            this.machineGame.LoadCustomMemory(customProg);
        }
  
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            machineGame.Update();
            
            this.hostPanel.Update(gameTime);
           
            // TODO: Add your update logic here
            this.sip.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //   GraphicsDevice.Clear(Color.CornflowerBlue);
            this.GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            // TODO: Add your drawing code here
            this.hostPanel.Draw(gameTime);
            
            _spriteBatch.End();

        }
    }
}