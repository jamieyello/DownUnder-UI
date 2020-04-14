using DownUnder.UI;
using DownUnder.UI.Widgets.BaseWidgets;
using DownUnder.UIEditor;
using DownUnder.UIEditor.Editor_Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using MonoGame.Extended;

namespace DownUnder.Widgets
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainWindow : DWindow
    {
        // shader testing --

        Texture2D surge;
        Effect effect;

        // shader testing --
        
        /// <summary> Contains all widgets relevant to this editor. (When the editor supports slots this won't be proper) </summary>
        private EditorObjects editor_objects;

        // Because this *editor* needs to be able to edit code. A typical window won't.
        /// <summary> Path to the editor's .cs file. </summary>
        private readonly string main_window_cs_file = "C:\\Users\\jamie\\Documents\\Visual Studio 2017\\Projects\\DownUnder\\DownUnder.UIEditor\\MainWindow.cs";

        public MainWindow(Layout layout = null, DWindow parent = null) : base(parent)
        {
            Content.RootDirectory = "Content";
        }
        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            LoadDWindow();

            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsManager.GraphicsDevice);
            SpriteFont = Content.Load<SpriteFont>("font");
            BackgroundEffect = Content.Load<Effect>("shader");

            Layout = new UIEditorLayout(this, out editor_objects);
            //Layout = EditorTools.TestLayout2(this);

            //Utility.Serialization.CSCreator.SerializeToCS(Layout, "UIEditor", "DownUnder.UI", " ForceUpdateOwnershipHierarchy");

            // TODO: use this.Content to load your game content here

            //shader testing---
            surge = Content.Load<Texture2D>("tests");
            effect = Content.Load<Effect>("shader");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            // TODO: Add your update logic here

            UpdateDWindow(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here
            
            DrawDWindow(gameTime);

            //effect.Parameters["Size"].SetValue(new Vector2(200, 100));
            //effect.CurrentTechnique.Passes[0].Apply();
            //SpriteBatch.FillRectangle(new RectangleF(0, 0, 200, 100), Color.Moccasin);
            
        }

        public void Slot_Layout_OnClick(object sender, EventArgs e)
        {
            Debug.WriteLine($"Successful click event");
        }
    }
}