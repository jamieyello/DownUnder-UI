using DownUnder.UI.Widgets.Behaviors.Examples.RotatableCubeBehaviors;
using DownUnder.UI.Widgets.Behaviors.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

// https://www.i-programmer.info/projects/119-graphics-and-games/1108-getting-started-with-3d-xna.html
namespace DownUnder.UI.Widgets.Behaviors.Examples
{
    public class RotatableCube : WidgetBehavior, IBaseWidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.COSMETIC_HIGH_PERFORMANCE };

        public Type[] BaseBehaviorPreviews { get; private set; } = new Type[] { typeof(CubeRotation), typeof(SpinOnHoverOnOff) };

        public Vector3 Angle = new Vector3();

        BasicEffect basicEffect;
        VertexPositionNormalTexture[] cube;

        protected override void Initialize()
        {
            Parent.DrawingMode = Widget.DrawingModeType.use_render_target;
            if (Parent.IsGraphicsInitialized) InitializeCube(this, EventArgs.Empty);
        }

        protected override void ConnectEvents()
        {
            Parent.OnGraphicsInitialized += InitializeCube;
            Parent.OnDrawOverlayEffects += Draw;
            Parent.OnUpdate += Update;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnGraphicsInitialized -= InitializeCube;
            Parent.OnDrawOverlayEffects -= Draw;
            Parent.OnUpdate -= Update;
        }

        public override Widget EditorWidgetRepresentation()
        {
            Widget result = base.EditorWidgetRepresentation();
            result.Behaviors.GetFirst<DrawText>().Text = "Rotatable\nCube";
            return result;
        }

        public override object Clone()
        {
            RotatableCube c = new RotatableCube();
            return c;
        }

        protected void Update(object sender, EventArgs args)
        {
            Matrix projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f, Parent.Width / Parent.Height, 1f, 10f);
            basicEffect.Projection = projection;
            Matrix V = Matrix.CreateTranslation(0f, 0f, -10f);
            basicEffect.View = V;

            if (Angle.X > 2 * Math.PI) Angle.X = 0;
            if (Angle.X < 0) Angle.X = 2f * (float)Math.PI;

            if (Angle.Y > 2 * Math.PI) Angle.Y = 0;
            if (Angle.Y < 0) Angle.Y = 2f * (float)Math.PI;

            if (Angle.Z > 2 * Math.PI) Angle.Z = 0;
            if (Angle.Z < 0) Angle.Z = 2f * (float)Math.PI;

            Matrix R = Matrix.CreateRotationY(Angle.Y) * Matrix.CreateRotationX(Angle.X) * Matrix.CreateRotationZ(Angle.Z);
            Matrix T = Matrix.CreateTranslation(0.0f, 0f, 5f);
            basicEffect.World = R * T;
        }

        private void InitializeCube(object sender, EventArgs args)
        {
            basicEffect = new BasicEffect(Parent.GraphicsDevice);

            basicEffect.AmbientLightColor = Vector3.One;
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.DiffuseColor = Vector3.One;
            basicEffect.DirectionalLight0.Direction = Vector3.Normalize(Vector3.One);

            basicEffect.LightingEnabled = true;
            basicEffect.AmbientLightColor = new Vector3(0.0f, 1.0f, 0.0f);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f, Parent.Width / Parent.Height, 1f, 10f);
            basicEffect.Projection = projection;
            Matrix V = Matrix.CreateTranslation(0f, 0f, -10f);
            basicEffect.View = V;

            cube = MakeCube();
        }

        public void Draw(object sender, EventArgs args)
        {
            foreach (EffectPass pass in
           basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Parent.GraphicsDevice.DrawUserPrimitives
                  <VertexPositionNormalTexture>(
                     PrimitiveType.TriangleList, cube,
                                                0, 12);
            }
        }

        protected VertexPositionNormalTexture[] MakeCube()
        {
            VertexPositionNormalTexture[] vertexes = new VertexPositionNormalTexture[36];
            Vector2 Texcoords = new Vector2(0f, 0f);

            Vector3[] face = new Vector3[6];

            //TopLeft
            face[0] = new Vector3(-1f, 1f, 0.0f);
            //BottomLeft
            face[1] = new Vector3(-1f, -1f, 0.0f);
            //TopRight
            face[2] = new Vector3(1f, 1f, 0.0f);
            //BottomLeft
            face[3] = new Vector3(-1f, -1f, 0.0f);
            //BottomRight
            face[4] = new Vector3(1f, -1f, 0.0f);
            //TopRight
            face[5] = new Vector3(1f, 1f, 0.0f);

            //front face
            for (int i = 0; i <= 2; i++)
            {
                vertexes[i] =
                  new VertexPositionNormalTexture(
                       face[i] + Vector3.UnitZ,
                             Vector3.UnitZ, Texcoords);
                vertexes[i + 3] =
                  new VertexPositionNormalTexture(
                       face[i + 3] + Vector3.UnitZ,
                             Vector3.UnitZ, Texcoords);
            }

            //Back face
            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 6] =
                   new VertexPositionNormalTexture(
                        face[2 - i] - Vector3.UnitZ,
                            -Vector3.UnitZ, Texcoords);
                vertexes[i + 6 + 3] =
                   new VertexPositionNormalTexture(
                        face[5 - i] - Vector3.UnitZ,
                            -Vector3.UnitZ, Texcoords);
            }

            //left face
            Matrix RotY90 = Matrix.CreateRotationY(
                               -(float)Math.PI / 2f);
            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 12] =
                  new VertexPositionNormalTexture(
                     Vector3.Transform(face[i], RotY90)
                          - Vector3.UnitX,
                            -Vector3.UnitX, Texcoords);
                vertexes[i + 12 + 3] =
                  new VertexPositionNormalTexture(
                 Vector3.Transform(face[i + 3], RotY90)
                         - Vector3.UnitX,
                           -Vector3.UnitX, Texcoords);
            }

            //Right face
            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 18] =
                  new VertexPositionNormalTexture(
                   Vector3.Transform(face[2 - i], RotY90)
                    - Vector3.UnitX,
                     Vector3.UnitX, Texcoords);
                vertexes[i + 18 + 3] =
                  new VertexPositionNormalTexture(
                  Vector3.Transform(face[5 - i], RotY90)
                  - Vector3.UnitX,
                     Vector3.UnitX, Texcoords);
            }

            //Top face
            Matrix RotX90 = Matrix.CreateRotationX(
                                -(float)Math.PI / 2f);
            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 24] =
                  new VertexPositionNormalTexture(
                   Vector3.Transform(face[i], RotX90)
                    + Vector3.UnitY,
                    Vector3.UnitY, Texcoords);
                vertexes[i + 24 + 3] =
                  new VertexPositionNormalTexture(
                   Vector3.Transform(face[i + 3], RotX90)
                    + Vector3.UnitY,
                    Vector3.UnitY, Texcoords);
            }
            //Bottom face

            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 30] =
                 new VertexPositionNormalTexture(
                  Vector3.Transform(face[2 - i], RotX90)
                   - Vector3.UnitY,
                    -Vector3.UnitY, Texcoords);
                vertexes[i + 30 + 3] =
                 new VertexPositionNormalTexture(
                  Vector3.Transform(face[5 - i], RotX90)
                   - Vector3.UnitY,
                    -Vector3.UnitY, Texcoords);
            }
            return vertexes;
        }
    }
}
