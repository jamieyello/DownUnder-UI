using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.DataTypes.AnimatedGraphics
{
    public abstract class AnimatedGraphic : ICloneable
    {
        protected readonly ChangingValue<float> progress = new ChangingValue<float>(0f);
        public bool IsInitialized { get; private set; }

        public void InitializeExternal(GraphicsDevice gd)
        {
            Initialize(gd);
            IsInitialized = true;
        }

        internal void UpdateExternal(float step)
        {
            progress.Update(step);
            Update(step);
        }

        internal void DrawExternal(WidgetDrawArgs args)
        {
            Draw(args);
        }

        protected abstract void Initialize(GraphicsDevice gd);
        protected abstract void Update(float step);
        protected abstract void Draw(WidgetDrawArgs args);

        public void SetStateStart()
        {
            progress.SetTargetValue(0f);
        }

        public void SetStateEnd()
        {
            progress.SetTargetValue(1f);
        }

        public abstract object Clone();
    }
}
