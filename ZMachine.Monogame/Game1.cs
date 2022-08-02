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
                0xb2, 19,141,42,234,3,45,40,28,40,215,120,28,26,105,42,224,26,105,3,45,40,18,57,141,103,192,33,170,43,10,1,55,83,147,41,37,200,165,
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