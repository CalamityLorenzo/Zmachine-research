using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using ZMachine.Monogame.Component;

namespace ZMachine.Monogame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont arial;
        private TextOutputComponent textOutput;
        private TestPanelContent _testContent;

        internal TypeToStream TypeToStream { get; private set; }

        private MemoryStream input0 = new(), input1 = new(), outputScreen = new(), outputTranscript = new();
        private ZMachineGamee machineGame;

        private ScrollablePanel scrollPanel;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += Window_ClientSizeChanged;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        private void Window_ClientSizeChanged(object? sender, EventArgs e)
        {
            if (Window.ClientBounds.Width != this._graphics.PreferredBackBufferWidth)
            {
               // this.ScaleFactor.Update(Window.ClientBounds.Width);
            }

            this._graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            this._graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

            this._graphics.ApplyChanges();

        }

        protected override void LoadContent()
        {
            this.arial = Content.Load<SpriteFont>("Arial");
            var filename = "Curses\\curses.z5";
            var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;
            this.machineGame = new ZMachineGamee(input0, input1, outputScreen, outputTranscript, fileStream);

            scrollPanel = new ScrollablePanel(this, this._spriteBatch, true, new Rectangle(40, 20, 500, 300));
            this.textOutput = new TextOutputComponent(this, _spriteBatch, arial, new Vector2(40,20), outputScreen);
            this._testContent = new TestPanelContent(this);
            scrollPanel.AddContent(_testContent);
            // TODO: use this.Content to load your game content here

            this.TypeToStream = new TypeToStream(this, this.input0);
            
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

            //this.textOutput.Update(gameTime);

            this._testContent.Update(gameTime);

            this.scrollPanel.Update(gameTime);
           
            // TODO: Add your update logic here
            this.TypeToStream.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            // TODO: Add your drawing code here
            // this.textOutput.Draw(gameTime);
            this.scrollPanel.Draw(gameTime);

            _spriteBatch.End();

        }
    }
}