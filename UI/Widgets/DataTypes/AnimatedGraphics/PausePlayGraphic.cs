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
        VertexPositionColor[] vert = new VertexPositionColor[4];
        short[] ind = new short[6];

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
            //basicEffect.TextureEnabled = false;

            vert[0].Position = new Vector3(0, 0, 0);
            vert[1].Position = new Vector3(1f, 0, 0);
            vert[2].Position = new Vector3(1f, 1f, 0);
            //vert[3].Position = new Vector3(0f, 1f, 0);

            vert[0].Color = Color.White;
            vert[1].Color = Color.White;
            vert[2].Color = Color.White;
            vert[3].Color = Color.White;

            ind[0] = 0;
            ind[1] = 2;
            ind[2] = 1;
            ind[3] = 1;
            ind[4] = 2;
            ind[5] = 3;
        }

        protected override void Update(float step)
        {
            color.Update(step);
        }

        protected override void Draw(WidgetDrawArgs args, RectangleF area)
        {
            float progress = 1f; //base.progress.GetCurrent();
            Color color = Color.White; //this.color.GetCurrent();
            Vector2 area_size = area.Size;
            Texture2D current_target = (Texture2D)args.SpriteBatch.GraphicsDevice.GetRenderTargets()[0].RenderTarget;
            Vector2 target_size = current_target.Bounds.Size.ToVector2();

            Vector2 window_offset = area.Position / target_size;
            Vector2 area_offset = area_size / target_size;
            Vector2 offset = window_offset * 2;// + area_offset / 2;

            //Debug.WriteLine("offset = " + offset);
            Debug.WriteLine("area_offset = " + area_offset);

            Vector2 resize = area_size / target_size;
            //basicEffect.Projection = ;
            basicEffect.View = Matrix.CreateTranslation(offset.X, -offset.Y, 1f) * Matrix.CreateScale(resize.X, resize.Y, 1f);


            args.RestartImmediate();

            foreach (EffectPass effectPass in basicEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                args.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList, vert, 0, vert.Length, ind, 0, ind.Length / 3);
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
