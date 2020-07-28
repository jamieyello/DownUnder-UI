using DownUnder.UI.Widgets.Behaviors.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    public class GlimmerBase : WidgetBehavior, IBaseWidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.COSMETIC_HIGH_PERFORMANCE };
        private static GroupBehaviorPolicy DefaultChild => new GroupBehaviorPolicy() { Behavior = new GlimmerChild(), InheritancePolicy = GroupBehaviorPolicy.BehaviorInheritancePolicy.apply_to_compatible_children };
        Effect crystal_effect;

        private Model model;

        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);


        public GroupBehaviorPolicy SubWidgetBehavior { get; set; } = DefaultChild;

        protected override void Initialize()
        {
            Parent.DrawingMode = Widget.DrawingModeType.use_render_target;
            Parent.GroupBehaviors.AddPolicy(SubWidgetBehavior);
        }

        protected override void ConnectEvents()
        {
            Parent.OnGraphicsInitialized += InitializeEffect;
            Parent.OnDrawOverlayEffects += Draw;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnGraphicsInitialized -= InitializeEffect;
            Parent.OnDrawOverlayEffects -= Draw;
        }

        public override object Clone()
        {
            GlimmerBase c = new GlimmerBase();
            c.SubWidgetBehavior = (GroupBehaviorPolicy)SubWidgetBehavior.Clone();
            c.crystal_effect = crystal_effect.Clone();
            return c;
        }

        private void InitializeEffect(object sender, EventArgs args)
        {
            crystal_effect = Parent.ParentWindow.EffectCollection.CrystalEffect.Clone();
            //model = Parent.ParentWindow.Content.Load<Model>("ModelTest/Helicopter");

            model = new Model(Par)
        }

        public void Draw(object sender, EventArgs args)
        {
            DrawModel(model, world, view, projection);
            crystal_effect.CurrentTechnique.Passes[0].Apply();
            Parent.SpriteBatch.FillRectangle(Parent.DrawingAreaUnscrolled, Color.Transparent);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }

        private Model TestModel()
        {

        }
    }
}
