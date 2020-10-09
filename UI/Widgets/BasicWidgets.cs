﻿using DownUnder.UI.Widgets.Actions;
using DownUnder.UI.Widgets.Actions.Functional;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.DataTypes.OverlayWidgetLocations;
using DownUnder.Utilities;
using DownUnder.Utility;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;
using static DownUnder.UI.Widgets.Widget;

namespace DownUnder.UI.Widgets
{
    public static class BasicWidgets
    {
        public static Widget GenericButton(string text)
        {
            return new Widget().WithAddedBehavior(new DrawText { Text = text, SideSpacing = 8f, ConstrainAreaToText = true });
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
                ChangeColorOnMouseOver = false,
                Height = 30,
                WidgetRole = WidgetRoleType.header_widget
            };

            file_bar.Behaviors.Add(new ShadingBehavior() { UseWidgetOutlineColor = true });
            file_bar.Behaviors.Add(new SpacedListFormat() { ListSpacing = 5f });

            foreach (var entry in entries)
            {
                Widget w_entry = new Widget().WithAddedBehavior(new DrawText() { Text = entry.Key });
                w_entry.DrawOutline = false;
                w_entry.DrawBackground = false;

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