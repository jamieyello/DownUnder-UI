﻿using DownUnder.UI.Widgets.Interfaces;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DownUnder.UI.Widgets.Behaviors {
    /// <summary> A <see cref="WidgetBehavior"/> acts as a plugin for a <see cref="Widget"/>. Adds additional behaviors to the <see cref="Widget"/>'s <see cref="EventHandler"/>s. </summary>
    public abstract class WidgetBehavior : INeedsWidgetParent, ICloneable {
        Widget _parent_backing;

        public abstract string[] BehaviorIDs { get; protected set; }
        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;

        public bool HasParent => Parent != null;

        public Widget Parent {
            get => _parent_backing;
            set {
                if (_parent_backing != null) {
                    if (_parent_backing == value) return;
                    throw new Exception($"{nameof(WidgetBehavior)}s cannot be reused. Use {nameof(Clone)} to create a copy first.");
                }
                _parent_backing = value;
                Initialize();
                ConnectEvents();
                OnConnect?.Invoke(this, EventArgs.Empty);
            }
        }

        internal void Disconnect()
        {
            DisconnectEvents();
            OnDisconnect?.Invoke(this, EventArgs.Empty);
        }

        protected abstract void Initialize();
        protected abstract void ConnectEvents();
        protected abstract void DisconnectEvents();
        public abstract object Clone();

        /// <summary> Set a tag that can only be read by this <see cref="WidgetBehavior"/>. </summary>
        public void SetTag(string key, string value) => SetTag(Parent, key, value);
        /// <summary> Set a tag that can only be read by this <see cref="WidgetBehavior"/>. </summary>
        public void SetTag(Widget widget, string key, string value) => widget.BehaviorTags[GetType()][key] = value;

        public string GetTag(string key) => GetTag(Parent, key);
        public string GetTag(Widget widget, string key) => widget.BehaviorTags[GetType()][key];

        public virtual Widget EditorWidgetRepresentation()
        {
            return new Widget()
            {
                Size = new Point2(100, 100)
            }.WithAddedBehavior(new DrawText() 
            { 
                Text = GetType().Name,
                TextPositioning = DrawText.TextPositioningPolicy.center
            });
        }
    }
}
