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
        public ChangingValue<float> Progress { get; private set; } = new ChangingValue<float>(0f);
        public bool IsInitialized { get; private set; }

        public void InitializeExternal(GraphicsDevice gd)
        {
            Initialize(gd);
            IsInitialized = true;
        }

        internal void UpdateExternal(float step)
        {
            Progress.Update(step);
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
            Progress.SetTargetValue(0f);
        }

        public void SetStateEnd()
        {
            Progress.SetTargetValue(1f);
        }

        public abstract object Clone();
    }
}
