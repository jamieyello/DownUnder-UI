using DownUnder;
using DownUnder.UI;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Linq;

namespace DownUnder.UIEditor
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private DWindow dWindow;
        public string[] Args;

        public Game1(string[] args)
        {
            _graphics = new GraphicsDeviceManager(this);
            Args = args;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.IsBorderless = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //dWindow = new DesktopWindow(_graphics, this, new OSInterface(), null, GroupBehaviorCollection.PlasmaDesktop);

            dWindow = new DesktopWindow(_graphics, this, new OSInterface(), null, GroupBehaviorCollection.PlasmaDesktop);
            dWindow.DisplayWidget = Widgets.EditorWidgets.UIEditor(Args);

            //dWindow.DisplayWidget = TestWidgets.ExplorerTest();
            //dWindow.DisplayWidget = TestWidgets.BrokenStuff();
            //dWindow.DisplayWidget = TestWidgets.BrokenFixedSize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            dWindow.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            dWindow.Draw(_spriteBatch);

            base.Draw(gameTime);
        }
    }
}