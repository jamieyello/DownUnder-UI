using DownUnder.UI.Widgets.Behaviors.Examples;
using DownUnder.UI.Widgets.Behaviors.Examples.Draw3DCubeBehaviors;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Format.GridFormatBehaviors;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.Widgets.Behaviors.Visual.DrawTextBehaviors;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class BehaviorFinder
    {
        BehaviorManager _behaviors;

        public BehaviorFinder(BehaviorManager behaviors)
        {
            _behaviors = behaviors;
        }

        // Visual
        public DrawText DrawText => _behaviors.GetFirst<DrawText>();
        public BlurBackground BlurBackground => _behaviors.GetFirst<BlurBackground>();
        public DragOffOutline DragOffOutline => _behaviors.GetFirst<DragOffOutline>();
        public DrawCenteredImage DrawCenteredImage => _behaviors.GetFirst<DrawCenteredImage>();
        public DrawSwitchGraphic DrawSwitchGraphic => _behaviors.GetFirst<DrawSwitchGraphic>();
        public DrawOutline DrawOutline => _behaviors.GetFirst<DrawOutline>();
        public MakeMousePointer MakeMousePointer => _behaviors.GetFirst<MakeMousePointer>();
        public MouseGlow MouseGlow => _behaviors.GetFirst<MouseGlow>();
        public Neurons Neurons => _behaviors.GetFirst<Neurons>();
        public ShadingBehavior ShadingBehavior => _behaviors.GetFirst<ShadingBehavior>();

        // Functional
        public CenterContent CenterContent => _behaviors.GetFirst<CenterContent>();
        public DragAndDropSource DragAndDropSource => _behaviors.GetFirst<DragAndDropSource>();
        public PinWidget PinWidget => _behaviors.GetFirst<PinWidget>();
        public PopInOut PopInOut => _behaviors.GetFirst<PopInOut>();
        public ScrollBar ScrollBar => _behaviors.GetFirst<ScrollBar>();
        public TriggerAction TriggerAction => _behaviors.GetFirst<TriggerAction>();
        public TriggerWidgetAction TriggerWidgetAction => _behaviors.GetFirst<TriggerWidgetAction>();
        
        // Format
        public BorderFormat BorderFormat => _behaviors.GetFirst<BorderFormat>();
        public GridFormat GridFormat => _behaviors.GetFirst<GridFormat>();
        public SpacedListFormat SpacedListFormat => _behaviors.GetFirst<SpacedListFormat>();
       
        // Examples
        public Draw3DCube Draw3DCube => _behaviors.GetFirst<Draw3DCube>();
        
        // Sub-behaviors
        public DrawEditableText DrawEditableText => _behaviors.GetFirst<DrawEditableText>();
        public RepresentMember RepresentMember => _behaviors.GetFirst<RepresentMember>();
        public MemberViewer MemberViewer => _behaviors.GetFirst<MemberViewer>();
        public CubeRotation CubeRotation => _behaviors.GetFirst<CubeRotation>();
        public SpinOnHoverOnOff SpinOnHoverOnOff => _behaviors.GetFirst<SpinOnHoverOnOff>();
    }
}
