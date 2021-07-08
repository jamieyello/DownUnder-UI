using System;
using DownUnder.UI.Widgets.Behaviors.Examples.Draw3DCubeBehaviors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// https://www.i-programmer.info/projects/119-graphics-and-games/1108-getting-started-with-3d-xna.html
namespace DownUnder.UI.Widgets.Behaviors.Examples {
    public sealed class Draw3DCube : WidgetBehavior, IEditorDisplaySubBehaviors {
        public Vector3 Angle;
        BasicEffect basicEffect;
        VertexPositionNormalTexture[] cube;

        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.COSMETIC_HIGH_PERFORMANCE };
        public Type[] BaseBehaviorPreviews { get; } = { typeof(CubeRotation), typeof(SpinOnHoverOnOff) };

        protected override void Initialize() {
            Parent.DrawingMode = Widget.DrawingModeType.use_render_target;
            if (Parent.IsGraphicsInitialized)
                InitializeCube(this, EventArgs.Empty);
        }

        protected override void ConnectEvents() {
            Parent.OnGraphicsInitialized += InitializeCube;
            Parent.OnDrawOverlay += Draw;
            Parent.OnUpdate += Update;
        }

        protected override void DisconnectEvents() {
            Parent.OnGraphicsInitialized -= InitializeCube;
            Parent.OnDrawOverlay -= Draw;
            Parent.OnUpdate -= Update;
        }

        public override object Clone() =>
            new Draw3DCube();

        void Update(object sender, EventArgs args) {
            var projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f, Parent.Width / Parent.Height, 1f, 10f);
            basicEffect.Projection = projection;
            var V = Matrix.CreateTranslation(0f, 0f, -10f);
            basicEffect.View = V;

            if (Angle.X > 2 * Math.PI) Angle.X = 0;
            if (Angle.X < 0) Angle.X = 2f * (float)Math.PI;

            if (Angle.Y > 2 * Math.PI) Angle.Y = 0;
            if (Angle.Y < 0) Angle.Y = 2f * (float)Math.PI;

            if (Angle.Z > 2 * Math.PI) Angle.Z = 0;
            if (Angle.Z < 0) Angle.Z = 2f * (float)Math.PI;

            var R = Matrix.CreateRotationY(Angle.Y) * Matrix.CreateRotationX(Angle.X) * Matrix.CreateRotationZ(Angle.Z);
            var T = Matrix.CreateTranslation(0.0f, 0f, 5f);
            basicEffect.World = R * T;
        }

        void InitializeCube(object sender, EventArgs args) {
            basicEffect = new BasicEffect(Parent.GraphicsDevice);

            basicEffect.AmbientLightColor = Vector3.One;
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.DiffuseColor = Vector3.One;
            basicEffect.DirectionalLight0.Direction = Vector3.Normalize(Vector3.One);

            basicEffect.LightingEnabled = true;
            basicEffect.AmbientLightColor = new Vector3(0.0f, 1.0f, 0.0f);

            var projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f, Parent.Width / Parent.Height, 1f, 10f);
            basicEffect.Projection = projection;
            var V = Matrix.CreateTranslation(0f, 0f, -10f);
            basicEffect.View = V;

            cube = MakeCube();
        }

        public void Draw(object sender, EventArgs args) {
            foreach (var pass in basicEffect.CurrentTechnique.Passes) {
                pass.Apply();
                Parent.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, cube, 0, 12);
            }
        }

        static VertexPositionNormalTexture[] MakeCube() {
            var vertexes = new VertexPositionNormalTexture[36];
            var Texcoords = new Vector2(0f, 0f);

            var face = new Vector3[6] {
                new Vector3(-1f, 1f, 0.0f),  // TopLeft
                new Vector3(-1f, -1f, 0.0f), // BottomLeft
                new Vector3(1f, 1f, 0.0f),   // TopRight
                new Vector3(-1f, -1f, 0.0f), // BottomLeft
                new Vector3(1f, -1f, 0.0f),  // BottomRight
                new Vector3(1f, 1f, 0.0f)    // TopRight
            };

            // front face
            for (var i = 0; i <= 2; i++) {
                vertexes[i] = new VertexPositionNormalTexture(
                    face[i] + Vector3.UnitZ,
                    Vector3.UnitZ,
                    Texcoords
                );

                vertexes[i + 3] = new VertexPositionNormalTexture(
                    face[i + 3] + Vector3.UnitZ,
                    Vector3.UnitZ,
                    Texcoords
                );
            }

            // Back face
            for (var i = 0; i <= 2; i++) {
                vertexes[i + 6] = new VertexPositionNormalTexture(
                    face[2 - i] - Vector3.UnitZ,
                    -Vector3.UnitZ,
                    Texcoords
                );

                vertexes[i + 6 + 3] = new VertexPositionNormalTexture(
                    face[5 - i] - Vector3.UnitZ,
                    -Vector3.UnitZ,
                    Texcoords
                );
            }

            // left face
            var RotY90 = Matrix.CreateRotationY(-(float)Math.PI / 2f);

            for (var i = 0; i <= 2; i++) {
                vertexes[i + 12] = new VertexPositionNormalTexture(
                    Vector3.Transform(face[i], RotY90) - Vector3.UnitX,
                    -Vector3.UnitX,
                    Texcoords
                );

                vertexes[i + 12 + 3] = new VertexPositionNormalTexture(
                    Vector3.Transform(face[i + 3], RotY90) - Vector3.UnitX,
                    -Vector3.UnitX,
                    Texcoords
                );
            }

            // Right face
            for (var i = 0; i <= 2; i++) {
                vertexes[i + 18] = new VertexPositionNormalTexture(
                    Vector3.Transform(face[2 - i], RotY90) - Vector3.UnitX,
                    Vector3.UnitX,
                    Texcoords
                );

                vertexes[i + 18 + 3] = new VertexPositionNormalTexture(
                    Vector3.Transform(face[5 - i], RotY90) - Vector3.UnitX,
                    Vector3.UnitX,
                    Texcoords
                );
            }

            // Top face
            var RotX90 = Matrix.CreateRotationX(-(float)Math.PI / 2f);

            for (var i = 0; i <= 2; i++) {
                vertexes[i + 24] = new VertexPositionNormalTexture(
                    Vector3.Transform(face[i], RotX90) + Vector3.UnitY,
                    Vector3.UnitY,
                    Texcoords
                );

                vertexes[i + 24 + 3] = new VertexPositionNormalTexture(
                    Vector3.Transform(face[i + 3], RotX90) + Vector3.UnitY,
                    Vector3.UnitY,
                    Texcoords
                );
            }

            // Bottom face
            for (var i = 0; i <= 2; i++) {
                vertexes[i + 30] = new VertexPositionNormalTexture(
                    Vector3.Transform(face[2 - i], RotX90) - Vector3.UnitY,
                    -Vector3.UnitY,
                    Texcoords
                );

                vertexes[i + 30 + 3] = new VertexPositionNormalTexture(
                    Vector3.Transform(face[5 - i], RotX90) - Vector3.UnitY,
                    -Vector3.UnitY,
                    Texcoords
                );
            }

            return vertexes;
        }
    }
}