using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.Widgets.DataTypes.OverlayWidgetLocations;
using DownUnder.UI.Widgets.Interfaces;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors
{
    /// <summary> A <see cref="WidgetBehavior"/> acts as a plugin for a <see cref="Widget"/>. Adds additional behaviors to the <see cref="Widget"/>'s <see cref="EventHandler"/>s. </summary>
    [KnownType(typeof(ScrollBar)), DataContract] public abstract class WidgetBehavior : IIsWidgetChild, ICloneable {
        Widget _parent_backing;

        public abstract string[] BehaviorIDs { get; protected set; }

        public bool HasParent => Parent != null;

        public bool IsSubBehavior => GetType().GetInterface("ISubWidgetBehavior`1") != null;

        public ISubWidgetBehavior<WidgetBehavior> AsSubBehavior => (ISubWidgetBehavior<WidgetBehavior>)this;

        public Type BaseBehaviorType => GetType().GetInterface("ISubWidgetBehavior`1").GetGenericArguments()[0];

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
            }
        }

        internal void Disconnect()
        {
            DisconnectEvents();
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
            Widget result;
            if (IsSubBehavior)
            {
                result = ((WidgetBehavior)Activator.CreateInstance(BaseBehaviorType)).EditorWidgetRepresentation();
                result.Behaviors.GetFirst<DrawText>().Text = EditorDisplayText();
                result.Behaviors.RemoveType(typeof(TriggerAction));
                result.Behaviors.GetFirst<DragAndDropSource>().DragObject = this;
            }
            else
            {
                result = new Widget()
                {
                    Size = new Point2(100, 100)
                };

                DrawText text = new DrawText()
                {
                    TextPositioning = DrawText.TextPositioningPolicy.center
                };
                text.Text = EditorDisplayText();

                result.Behaviors.Add(text);
                result.Behaviors.Add(new DragAndDropSource() { DragObject = this });
                result.Behaviors.Add(new DragOffOutline());
            }

            if (this is IEditorDisplaySubBehaviors b_this)
            {
                result.Behaviors.Add(new Behaviors.Functional.TriggerAction(
                    nameof(Widget.OnDoubleClick),
                    new Actions.Functional.AddMainWidget()
                    {
                        LocationOptions = new CoverParentOverlay(1),
                        Widget = new Widget()
                        //Widget = BehaviorDisplay(b_this.BaseBehaviorPreviews).WithAddedBehavior(
                        //    new PopInOut() 
                        //    { 
                        //        CloseOnClickOff = true,
                        //    })
                    }
                    ));
            }

            return result;
        }

        private string EditorDisplayText()
        {
            string type_name = GetType().Name;
            StringBuilder name = new StringBuilder();
            name.Append(type_name[0]);
            for (int i = 1; i < type_name.Length; i++)
            {
                if (char.IsUpper(type_name[i])) name.Append('\n');
                name.Append(type_name[i]);
            }
            return name.ToString();
        }

        public static Widget BehaviorDisplay(IEnumerable<Type> behaviors)
        {
            Widget behaviors_list = new Widget().WithAddedBehavior(new SpacedListFormat());
            behaviors_list.ChangeColorOnMouseOver = false;

            foreach (Type type in behaviors)
            {
                if (Activator.CreateInstance(type) is WidgetBehavior behavior) behaviors_list.Add(behavior.EditorWidgetRepresentation());
                else throw new Exception($"A given {nameof(Type)} was not a {nameof(WidgetBehavior)}.");
            }

            return behaviors_list;
        }

        /// <summary> Creates a <see cref="Widget"/> and adds this <see cref="WidgetBehavior"/>. </summary>
        public Widget CreateWidget()
        {
            return new Widget().WithAddedBehavior(this);
        }
    }
}
