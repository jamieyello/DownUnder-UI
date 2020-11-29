using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
        VertexPositionColor[] vert;

        Vector2[] left_start;
        Vector2[] right_start;
        Vector2[] left_end;
        Vector2[] right_end;

        ChangingValue<Color> color= new ChangingValue<Color>(Color.White);
        
        public PausePlayGraphic()
        {
            left_start = new Vector2[] { new Vector2(0f, 0f), new Vector2(0.4f, 0f), new Vector2(0.4f, 1f), new Vector2(0f, 1f) };
            right_start = new Vector2[4];
            left_end = new Vector2[4];
            right_end = new Vector2[4];
        }

        protected override void Initialize(GraphicsDevice gd)
        {
            basicEffect = new BasicEffect(gd);

            vert = new VertexPositionColor[12];

            vert[2].Position = new Vector3(0.1f, 0.1f, 0);
            vert[1].Position = new Vector3(0.6f, 0.1f, 0);
            vert[0].Position = new Vector3(0.1f, 0.6f, 0);

            vert[3].Position = new Vector3(-0.1f, 0.1f, 0);
            vert[4].Position = new Vector3(-0.6f, 0.1f, 0);
            vert[5].Position = new Vector3(-0.1f, 0.6f, 0);

            vert[6].Position = new Vector3(0.1f, -0.1f, 0);
            vert[7].Position = new Vector3(0.6f, -0.1f, 0);
            vert[8].Position = new Vector3(0.1f, -0.6f, 0);

            vert[11].Position = new Vector3(-0.1f, -0.1f, 0);
            vert[10].Position = new Vector3(-0.6f, -0.1f, 0);
            vert[9].Position = new Vector3(-0.1f, -0.6f, 0);

            for (int i = 0; i < vert.Length; i++)
            {
                vert[i].Color = Color.White;
            }
        }

        protected override void Update(float step)
        {
            color.Update(step);
        }

        protected override void Draw(WidgetDrawArgs args)
        {
            float progress = 1f; //base.progress.GetCurrent();
            Color color = Color.White; //this.color.GetCurrent();

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
    }
}
