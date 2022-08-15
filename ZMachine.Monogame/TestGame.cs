using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ZMachine.Monogame.Component;
using ZMachine.Monogame.Components;

namespace ZMachine.Monogame
{
    public class TestGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ZMachineScreenOutput screenOutput;
        private Stream outputStream;
        private StreamInputComponent sic;
        public TestGame()
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
            var arial = Content.Load<SpriteFont>("Arial");

            outputStream = new MemoryStream();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            sic = new(this, outputStream);
            screenOutput = new(this, this._spriteBatch, arial, new Color(new Vector3(113f, 202f, 197f)), Color.Black, new Vector2(10, 10), outputStream);

            // TODO: use this.Content to load your game content here
            var statusLineText = "@@STATUS_LINE@@:Hello Doctor";
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
            screenOutput.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            this._spriteBatch.Begin();
            // TODO: Add your drawing code here
            screenOutput.Draw(gameTime);
            this._spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
