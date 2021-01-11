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
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;
using static DownUnder.UI.Widgets.Behaviors.Visual.DrawText;
using static DownUnder.UI.Widgets.DataTypes.GeneralVisualSettings;

namespace DownUnder.UI.Widgets
{
    public static class CommonWidgets
    {
        public static Widget Button(string text, RectangleF? area = null)
        {
            Widget result = new Widget(area)
                .WithAddedBehavior(new MakeMousePointer())
                .WithAddedBehavior(new DrawText { Text = text, SideSpacing = 8f, ConstrainAreaToText = true, XTextPositioning = XTextPositioningPolicy.center, YTextPositioning = DrawText.YTextPositioningPolicy.center });
            result.VisualSettings.VisualRole = VisualRoleType.button;
            result.Name = text;
            result.Behaviors.GroupBehaviors.AcceptancePolicy.DisallowedIDs.Add(DownUnderBehaviorIDs.SCROLL_FUNCTION);

            return result;
        }

        public static Widget ImageButton(string asset_name, float scaling = 1f, RectangleF? area = null)
        {
            Widget result = new Widget(area)
                .WithAddedBehavior(new MakeMousePointer())
                .WithAddedBehavior(new DrawCenteredImage(asset_name, scaling));
            result.VisualSettings.VisualRole = VisualRoleType.button;
            result.Behaviors.GroupBehaviors.AcceptancePolicy.DisallowedIDs.Add(DownUnderBehaviorIDs.SCROLL_FUNCTION);

            return result;
        }

        public static Widget Label(string text, RectangleF? area = null, XTextPositioningPolicy x_positioning = XTextPositioningPolicy.left, YTextPositioningPolicy y_positioning = YTextPositioningPolicy.center)
        {
            Widget result = new Widget(area);
            result.VisualSettings.DrawBackground = false;
            result.VisualSettings.DrawOutline = false;
            result.Behaviors.Add(new DrawText { Text = text, XTextPositioning = x_positioning, YTextPositioning = y_positioning });
            result.Behaviors.GroupBehaviors.AcceptancePolicy.DisallowedIDs.Add(DownUnderBehaviorIDs.SCROLL_FUNCTION);

            return result;
        }

        public static Widget SingleLineTextEntry(string text = "", XTextPositioningPolicy x_positioning = XTextPositioningPolicy.left, YTextPositioningPolicy y_positioning = YTextPositioningPolicy.center, float? side_spacing = null, bool constrain_area_to_text = false, RectangleF? area = null)
        {
            Widget result = new Widget(area);
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

        public static Widget PropertyGrid(object obj, float row_height = 30f)
        {
            return new Widget().WithAddedBehavior(new Behaviors.Format.GridFormatBehaviors.MemberViewer(obj, row_height));
        }

        public static Widget FileBar(AutoDictionary<string, AutoDictionary<string, DropDownEntry>> entries)
        {
            Widget file_bar = new Widget
            {
                Height = 40f,
            };

            file_bar.VisualSettings.ChangeColorOnMouseOver = false;
            file_bar.VisualSettings.VisualRole = VisualRoleType.header_widget;
            file_bar.FitToContentArea = true;

            //file_bar.Behaviors.Add(new ShadingBehavior() { UseWidgetOutlineColor = true });
            file_bar.Behaviors.Add(new SpacedListFormat() { ListSpacing = 0f });

            foreach (var entry in entries)
            {
                Widget w_entry = Button(entry.Key, new RectangleF(0, 0, 50, 40));
                w_entry.Behaviors.GetFirst<DrawText>().SideSpacing = 25f;
                w_entry.VisualSettings.DrawOutline = false;
                w_entry.VisualSettings.DrawBackground = false;
                
                WidgetList items = new WidgetList();
                foreach (var item in entry.Value)
                {
                    Widget new_entry = Button(item.Key);
                    new_entry.VisualSettings.DrawOutline = false;
                    new_entry.VisualSettings.DrawBackground = false;
                    new_entry.Behaviors.GetFirst<DrawText>().XTextPositioning = XTextPositioningPolicy.left;
                    if (item.Value.ClickAction != null) new_entry.Behaviors.Add(new TriggerWidgetAction(nameof(Widget.OnClick), (WidgetAction)item.Value.ClickAction.InitialClone()));
                    items.Add(new_entry);
                }

                Widget dropdown = DropDown(items);

                w_entry.Behaviors.Add(
                    new TriggerWidgetAction(
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
            dropdown.VisualSettings.VisualRole = VisualRoleType.pop_up;
            dropdown.Behaviors.GroupBehaviors.AcceptancePolicy += GroupBehaviorAcceptancePolicy.NoUserScrolling;
            dropdown.MinimumSize = new Point2(1f, 1f);
            dropdown.AddRange(widgets);
            dropdown.Behaviors.Add(new GridFormat(1, widgets.Count()));
            if (pop_in_out_behavior == null) dropdown.Behaviors.Add(new PopInOut(RectanglePart.Uniform(0.95f, Directions2D.DLR), RectanglePart.Uniform(0f, Directions2D.D, 1f)) { OpeningMotion = InterpolationSettings.Fast, ClosingMotion = InterpolationSettings.Faster });
            else dropdown.Behaviors.Add(pop_in_out_behavior);


            return dropdown;
        }

        public static Widget EntryField(string label_text, out Widget text_box, Point2? location = null, Point2? size = null, float ratio = 0.5f)
        {
            if (size == null) size = new Point2(250, 40);
            if (location == null) location = new Point2();
            Widget result = new Widget(location.Value, size.Value);
            result.VisualSettings.VisualRole = GeneralVisualSettings.VisualRoleType.background;
            result.VisualSettings.DrawBackground = false;
            result.VisualSettings.DrawOutline = false;
            Widget label = Label(label_text);
            text_box = SingleLineTextEntry();

            label.Area = new RectangleF(0, 0, size.Value.X * ratio, size.Value.Y);
            text_box.Area = new RectangleF(size.Value.X * ratio, 0, size.Value.X * (1f - ratio), size.Value.Y);

            result.Add(label);
            result.Add(text_box);

            return result;
        }
    }
}
