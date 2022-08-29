
using ZMachine.Monogame.Component;
using ZMachine.Monogame.Components.TextComponents;
using ZMachine.Monogame.Components;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Color = Microsoft.Xna.Framework.Color;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Zmachine.Library.V2.Implementation;

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

        public V2Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            var filename = "Curses\\curses.z5";
            filename = "hollywoo.dat";
            using var StoryData = File.Open(filename, FileMode.Open);
            var sd = new byte[StoryData.Length];
            Span<byte> spSpan = sd;
            StoryData.Read(spSpan);

            // TODO: Add your initialization logic here
            zMachine = new Machine(input0, input1, outputScreen, outputTranscript, spSpan.ToArray());

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            zMachine.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}