using DownUnder.Content.Utilities.Serialization;
using DownUnder.UI;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.UI.Widgets.Behaviors.Examples.Draw3DCubeBehaviors;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UIEditor;
using DownUnder.UIEditor.EditorTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace DownUnder.Widgets
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainWindow : DWindow
    {
        /// <summary> Contains all widgets relevant to this editor. (When the editor supports slots this won't be proper) </summary>
        internal EditorObjects editor_objects;

        // Because this *editor* needs to be able to edit code. A typical window won't.
        /// <summary> Path to the editor's .cs file. </summary>
        //private readonly string main_window_cs_file = "C:\\Users\\jamie\\Documents\\Visual Studio 2017\\Projects\\DownUnder\\DownUnder.UIEditor\\MainWindow.cs";

public MainWindow(DWindow parent = null) : base(parent)
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

            WindowFont = Content.Load<SpriteFont>("font");

            //XmlHelper.ToXmlFile(TestLayouts.ContainerTest(), "test.xml");
            //MainWidget = TestLayouts.PropertyTest();
            //MainWidget = TestLayouts.PropertyTest();
            
            MainWidget = EditorWidgets.UIEditor(out editor_objects);

            MainWidget.GroupBehaviors.AddPolicy(GroupBehaviorCollection.StandardPC);

            //XmlHelper.ToXmlFile(new Widget(), "test.xml");
            //Widget read = XmlHelper.FromXmlFile<Widget>("test.xml");

            // TODO: use this.Content to load your game content here
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
        }

        public void Slot_Layout_OnClick(object sender, EventArgs e)
        {
            Debug.WriteLine($"Successful click event");
        }

        public void SetProject(Widget widget)
        {
            Widget container = editor_objects.project.ParentWidget;
            editor_objects.project.Delete();
            container.Add(widget);
            widget.DesignerObjects.IsEditModeEnabled = true;
        }

        public void SetObjectViewer(object obj)
        {
            //editor_objects.property_grid_container
        }
    }
}