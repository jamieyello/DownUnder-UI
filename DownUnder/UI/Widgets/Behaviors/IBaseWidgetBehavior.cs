using System;

namespace DownUnder.UI.Widgets.Behaviors
{
    /// <summary> A <see cref="WidgetBehavior"/> that has <see cref="ISubWidgetBehavior"/>s that are modify this <see cref="WidgetBehavior"/>. </summary>
    public interface IBaseWidgetBehavior
    {
        Type[] BaseBehaviorPreviews { get; }
    }
}
