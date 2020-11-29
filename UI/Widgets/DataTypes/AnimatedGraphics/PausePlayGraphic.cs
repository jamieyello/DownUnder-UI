using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DownUnder.Utilities;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace DownUnder.UI.Widgets.DataTypes.AnimatedGraphics
{
    class PausePlayGraphic : AnimatedGraphic
    {
        BasicEffect basicEffect;
        VertexPositionColor[] starting_vert;
        VertexPositionColor[] ending_vert;

        ChangingValue<Color> color = new ChangingValue<Color>(Color.White);

        float pause_gap = 0.8f;

        public PausePlayGraphic()
        {
            starting_vert = PauseVertex(pause_gap);
            ending_vert = PlayVertex();
            Progress.InterpolationSettings = InterpolationSettings.Faster;
        }

        protected override void Initialize(GraphicsDevice gd)
        {
            basicEffect = new BasicEffect(gd);
        }

        protected override void Update(float step)
        {
            color.Update(step);
        }

        protected override void Draw(WidgetDrawArgs args)
        {
            float progress = Progress.GetCurrent();

            VertexPositionColor[] vert = new VertexPositionColor[starting_vert.Length];
            for (int i = 0; i < vert.Length; i++)
            {
                vert[i] = starting_vert[i].Lerp(ending_vert[i], progress);
            }

            basicEffect.Projection = args.GetStretchedProjection();

            args.RestartImmediate();

            foreach (EffectPass effectPass in basicEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                args.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList, vert, 0, vert.Length / 3);
            }

            args.RestartDefault();

            //args.SpriteBatch.DrawCircle(new Vector2(30f) + area.Position, 25f, 6, Color.White);
            //throw new Exception();
            //args.SpriteBatch.DrawPolygon(area.Position, new Polygon(new Vector2[] { new Vector2(), new Vector2(30, 0), new Vector2(30, 30) }), Color.White);
            //args.SpriteBatch.Dr
            //args.SpriteBatch.DrawQuad(new Vector2(), new Vector2[] { new Vector2(), new Vector2(30, 0), new Vector2(30, 30), new Vector2(0, 30) }, Color.White);
            //args.SpriteBatch.DrawQuad(area.Position, new Vector2[] 
            //{ 
            //    Vector2.Lerp(left_start[0] * size, left_end[0] * size, progress),
            //    Vector2.Lerp(left_start[1] * size, left_end[1] * size, progress),
            //    Vector2.Lerp(left_start[2] * size, left_end[2] * size, progress), 
            //    Vector2.Lerp(left_start[3] * size, left_end[3] * size, progress)
            //}, color);
            //args.SpriteBatch.DrawQuad(area.Position, new Vector2[]
            //{
            //    Vector2.Lerp(right_start[0], right_end[0], progress),
            //    Vector2.Lerp(right_start[1], right_end[1], progress),
            //    Vector2.Lerp(right_start[2], right_end[2], progress),
            //    Vector2.Lerp(right_start[3], right_end[3], progress)
            //}, color);
        }

        public override object Clone()
        {
            return new PausePlayGraphic();
        }

        private static VertexPositionColor[] PauseVertex(float pause_gap, Color? color = null)
        {
            VertexPositionColor[] vert = new VertexPositionColor[12];
            float pg = pause_gap / 2f;

            // TL
            vert[0].Position = new Vector3(-1f, -1f, 0f);
            vert[1].Position = new Vector3(-1f, 1f, 0f);
            vert[2].Position = new Vector3(-pg, 1f, 0f);

            // BL
            vert[3].Position = new Vector3(-1f, -1f, 0f);
            vert[4].Position = new Vector3(-pg, 1f, 0f);
            vert[5].Position = new Vector3(-pg, -1f, 0f);

            // TR
            vert[8].Position = new Vector3(1f, -1f, 0f);
            vert[7].Position = new Vector3(1f, 1f, 0f);
            vert[6].Position = new Vector3(pg, 1f, 0f);

            // BR
            vert[11].Position = new Vector3(1f, -1f, 0f);
            vert[10].Position = new Vector3(pg, 1f, 0f);
            vert[9].Position = new Vector3(pg, -1f, 0f);

            for (int i = 0; i < vert.Length; i++)
            {
                vert[i].Color = color ?? Color.White;
            }

            return vert;
        }

        private static VertexPositionColor[] PlayVertex(Color? color = null)
        {
            VertexPositionColor[] vert = new VertexPositionColor[12];

            // TL
            vert[0].Position = new Vector3(-1f, -1f, 0f);
            vert[1].Position = new Vector3(-1f, 1f, 0f);
            vert[2].Position = new Vector3(1f, 0f, 0f);

            //// BL
            vert[3].Position = new Vector3(-1f, -1f, 0f);
            vert[4].Position = new Vector3(1f, 0f, 0f);
            vert[5].Position = new Vector3(-1f, 1f, 0f);

            //// TR
            vert[8].Position = new Vector3(1f, -1f, 0f);
            vert[7].Position = new Vector3(1f, 1f, 0f);
            vert[6].Position = new Vector3(1f, 1f, 0f);

            //// BR
            vert[11].Position = new Vector3(1f, -1f, 0f);
            vert[10].Position = new Vector3(1f, 1f, 0f);
            vert[9].Position = new Vector3(1f, -1f, 0f);

            for (int i = 0; i < vert.Length; i++)
            {
                vert[i].Color = color ?? Color.White;
            }

            return vert;
        }
    }
}
