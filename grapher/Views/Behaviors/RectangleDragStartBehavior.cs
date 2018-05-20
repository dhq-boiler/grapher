﻿using System.Windows;

namespace grapher.Views.Behaviors
{
    class RectangleDragStartBehavior : AbstractDragStartBehavior
    {
        protected override AbstractDragAdorner CreateDragAdorner(UIElement owner, UIElement adornedElement, double opacity, Point dragPos)
        {
            return new RectangleDragAdorner(owner, adornedElement, opacity, dragPos);
        }
    }
}
