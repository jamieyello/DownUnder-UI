using DownUnder.UI.Widgets.Behaviors;
using DownUnder.UI.Widgets.Behaviors.Examples;
using DownUnder.UI.Widgets.Behaviors.Examples.Draw3DCubeBehaviors;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Format.GridFormatBehaviors;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.Widgets.Behaviors.Visual.DrawTextBehaviors;

namespace DownUnder.UI.Widgets.Behaviors {
    public sealed class BehaviorFinder {
        readonly BehaviorManager _behaviors;

        public BehaviorFinder(BehaviorManager behaviors) =>
            _behaviors = behaviors;

        // Visual
        public DrawText DrawText => _behaviors.Get<DrawText>();
        public BlurBackground BlurBackground => _behaviors.Get<BlurBackground>();
        public DragOffOutline DragOffOutline => _behaviors.Get<DragOffOutline>();
        public DrawCenteredImage DrawCenteredImage => _behaviors.Get<DrawCenteredImage>();
        public DrawSwitchGraphic DrawSwitchGraphic => _behaviors.Get<DrawSwitchGraphic>();
        public DrawOutline DrawOutline => _behaviors.Get<DrawOutline>();
        public MakeMousePointer MakeMousePointer => _behaviors.Get<MakeMousePointer>();
        public MouseGlow MouseGlow => _behaviors.Get<MouseGlow>();
        public Neurons Neurons => _behaviors.Get<Neurons>();
        public ShadingBehavior ShadingBehavior => _behaviors.Get<ShadingBehavior>();

        // Functional
        public CenterContent CenterContent => _behaviors.Get<CenterContent>();
        public DragAndDropSource DragAndDropSource => _behaviors.Get<DragAndDropSource>();
        public PinWidget PinWidget => _behaviors.Get<PinWidget>();
        public PopInOut PopInOut => _behaviors.Get<PopInOut>();
        public ScrollBar ScrollBar => _behaviors.Get<ScrollBar>();
        public TriggerAction TriggerAction => _behaviors.Get<TriggerAction>();
        public TriggerWidgetAction TriggerWidgetAction => _behaviors.Get<TriggerWidgetAction>();

        // Format
        public BorderFormat BorderFormat => _behaviors.Get<BorderFormat>();
        public GridFormat GridFormat => _behaviors.Get<GridFormat>();
        public SpacedListFormat SpacedListFormat => _behaviors.Get<SpacedListFormat>();

        // Examples
        public Draw3DCube Draw3DCube => _behaviors.Get<Draw3DCube>();

        // Sub-behaviors
        public DrawEditableText DrawEditableText => _behaviors.Get<DrawEditableText>();
        public RepresentMember RepresentMember => _behaviors.Get<RepresentMember>();
        public MemberViewer MemberViewer => _behaviors.Get<MemberViewer>();
        public CubeRotation CubeRotation => _behaviors.Get<CubeRotation>();
        public SpinOnHoverOnOff SpinOnHoverOnOff => _behaviors.Get<SpinOnHoverOnOff>();
    }
}
