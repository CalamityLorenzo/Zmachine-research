using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace ZMachine.Monogame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont arial;
        private TextOutputComponent textOutput;
        private MemoryStream input0 = new(), input1 = new(), outputScreen = new(), outputTranscript = new();
        private ZMachineGamee machineGame;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            this.arial = Content.Load<SpriteFont>("Arial");
            var filename = "Curses\\curses.z5";
            var fileStream = File.Open(filename, FileMode.Open);
            fileStream.Position = 0;
            this.machineGame = new ZMachineGamee(input0, input1, outputScreen, outputTranscript, fileStream);
            this.textOutput = new TextOutputComponent(this, _spriteBatch, arial, outputScreen);
            // TODO: use this.Content to load your game content here

            this.machineGame.LoadCustomMemory(new byte[]
            {
                0x8f, 00,1, // call_1n x x (4)
                0xb0,       // return true
                // Routine start
                3,
                0xb2, 18,42,103,0,25,41,3,20,73,64,79,82,29,87,224,165,  //Print a big fat string.
                0xbb,
                0xb2, 17,83,101,87,1,110,95,25,2,122,72,234,220,189,    // More groovy strings
                0xb0,       // return true
                // routine end.

            });
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            machineGame.Update();

            this.textOutput.Update(gameTime);
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            // TODO: Add your drawing code here
            this.textOutput.Draw(gameTime);
            _spriteBatch.End();

        }
    }
}