using DownUnder.UI.Widgets.Actions;
using DownUnder.UI.Widgets.Actions.Functional;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.Widgets.Behaviors.Visual.DrawTextBehaviors;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.DataTypes.OverlayWidgetLocations;
using DownUnder.Utilities;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;
using static DownUnder.UI.Widgets.Behaviors.Visual.DrawText;
using static DownUnder.UI.Widgets.DataTypes.GeneralVisualSettings;

namespace DownUnder.UI.Widgets
{
    public static class BasicWidgets
    {
        public static Widget Button(string text)
        {
            Widget result = new Widget()
                .WithAddedBehavior(new MakeMousePointer())
                .WithAddedBehavior(new DrawText { Text = text, SideSpacing = 8f, ConstrainAreaToText = true, XTextPositioning = XTextPositioningPolicy.center, YTextPositioning = DrawText.YTextPositioningPolicy.center });
            result.VisualSettings.VisualRole = VisualRoleType.button;

            return result;
        }

        public static Widget ImageButton(string asset_name, float scaling = 1f)
        {
            Widget result = new Widget()
                .WithAddedBehavior(new MakeMousePointer())
                .WithAddedBehavior(new DrawCenteredImage(asset_name, scaling));
            result.VisualSettings.VisualRole = VisualRoleType.button;

            return result;
        }

        public static Widget Label(string text)
        {
            Widget result = new Widget();
            result.VisualSettings.DrawBackground = false;
            result.VisualSettings.DrawOutline = false;
            result.Behaviors.Add(new DrawText { Text = text, YTextPositioning = YTextPositioningPolicy.center });
            return result;
        }

        public static Widget SingleLineTextEntry(string text = "", XTextPositioningPolicy x_positioning = XTextPositioningPolicy.left, YTextPositioningPolicy y_positioning = YTextPositioningPolicy.top, float? side_spacing = null, bool constrain_area_to_text = false)
        {
            Widget result = new Widget { };
            result.VisualSettings.VisualRole = VisualRoleType.text_edit_widget;
            result.Behaviors.Add(new DrawEditableText { });
            var draw_text = result.Behaviors.GetFirst<DrawText>();
            draw_text.Text = text;
            draw_text.XTextPositioning = x_positioning;
            draw_text.YTextPositioning = y_positioning;
            draw_text.ConstrainAreaToText = constrain_area_to_text;
            if (side_spacing != null) draw_text.SideSpacing = side_spacing.Value;
            return result;
        }

        public static Widget PropertyGrid(object obj)
        {
            Widget property_edit_widget = new Widget() { SnappingPolicy = DiagonalDirections2D.TL_TR_BL_BR };
            property_edit_widget.Behaviors.Add(new Behaviors.Format.GridFormatBehaviors.MemberViewer(obj));
            return property_edit_widget;
        }

        public static Widget FileBar(AutoDictionary<string, AutoDictionary<string, DropDownEntry>> entries)
        {
            Widget file_bar = new Widget
            {
                Height = 30,
            };

            file_bar.VisualSettings.ChangeColorOnMouseOver = false;
            file_bar.VisualSettings.VisualRole = VisualRoleType.header_widget;

            file_bar.Behaviors.Add(new ShadingBehavior() { UseWidgetOutlineColor = true });
            file_bar.Behaviors.Add(new SpacedListFormat() { ListSpacing = 5f });

            foreach (var entry in entries)
            {
                Widget w_entry = new Widget().WithAddedBehavior(new DrawText() { Text = entry.Key });
                w_entry.VisualSettings.DrawOutline = false;
                w_entry.VisualSettings.DrawBackground = false;

                WidgetList items = new WidgetList();
                foreach (var item in entry.Value)
                {
                    Widget new_entry = new Widget()
                        .WithAddedBehavior(new DrawText() { Text = item.Key });
                    if (item.Value.ClickAction != null) new_entry.Behaviors.Add(new TriggerAction(nameof(Widget.OnClick), (WidgetAction)item.Value.ClickAction.InitialClone()));
                    items.Add(new_entry);
                }

                Widget dropdown = DropDown(items);

                w_entry.Behaviors.Add(
                    new TriggerAction(
                        nameof(Widget.OnClick), 
                        new AddMainWidget(dropdown) 
                        { 
                            LocationOptions = new SideOfParent() { ParentSide = Direction2D.down, ParentUp = 1 } 
                        }));

                file_bar.Add(w_entry);
            }

            return file_bar;
        }

        public static Widget DropDown(IEnumerable<string> items, PopInOut pop_in_out_behavior = null)
        {
            WidgetList widgets = new WidgetList();
            foreach (string item in items) widgets.Add(new Widget().WithAddedBehavior(new DrawText() { Text = item, ConstrainAreaToText = true }));
            return DropDown(widgets, pop_in_out_behavior);
        }

        public static Widget DropDown(IEnumerable<Widget> widgets, PopInOut pop_in_out_behavior = null)
        {
            Widget dropdown = new Widget();
            dropdown.Behaviors.GroupBehaviors.AcceptancePolicy += GroupBehaviorAcceptancePolicy.NoUserScrolling;
            dropdown.MinimumSize = new Point2(1f, 1f);
            dropdown.AddRange(widgets);
            dropdown.Behaviors.Add(new GridFormat(1, widgets.Count()));
            if (pop_in_out_behavior == null) dropdown.Behaviors.Add(new PopInOut(RectanglePart.Uniform(0.95f, Directions2D.DLR), RectanglePart.Uniform(0f, Directions2D.D, 1f)) { ClosingMotion = InterpolationSettings.Faster });
            else dropdown.Behaviors.Add(pop_in_out_behavior);

            return dropdown;
        }


    }
}
