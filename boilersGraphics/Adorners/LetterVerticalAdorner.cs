﻿using boilersGraphics.Controls;
using boilersGraphics.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace boilersGraphics.Adorners
{
    public class LetterVerticalAdorner : Adorner
    {
        private Point? _startPoint;
        private Point? _endPoint;
        private Pen _rectanglePen;

        private DesignerCanvas _designerCanvas;

        public LetterVerticalAdorner(DesignerCanvas designerCanvas, Point? dragStartPoint)
            : base(designerCanvas)
        {
            _designerCanvas = designerCanvas;
            _startPoint = dragStartPoint;
            var brush = new SolidColorBrush(Colors.Black);
            brush.Opacity = 0.5;
            _rectanglePen = new Pen(brush, 1);
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.IsMouseCaptured)
                    this.CaptureMouse();

                _endPoint = e.GetPosition(this);
                this.InvalidateVisual();
            }
            else
            {
                if (this.IsMouseCaptured) this.ReleaseMouseCapture();
            }

            e.Handled = true;
        }
        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            // release mouse capture
            if (this.IsMouseCaptured) this.ReleaseMouseCapture();

            // remove this adorner from adorner layer
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_designerCanvas);
            if (adornerLayer != null)
                adornerLayer.Remove(this);

            if (_startPoint.HasValue && _endPoint.HasValue)
            {
                LetterVerticalDesignerItemViewModel itemBase = new LetterVerticalDesignerItemViewModel();
                itemBase.Owner = (AdornedElement as DesignerCanvas).DataContext as IDiagramViewModel;
                itemBase.Left.Value = Math.Max(0, _startPoint.Value.X);
                itemBase.Top.Value = Math.Max(0, _startPoint.Value.Y);
                itemBase.Width.Value = Math.Abs(_endPoint.Value.X - _startPoint.Value.X);
                itemBase.Height.Value = Math.Abs(_endPoint.Value.Y - _startPoint.Value.Y);
                itemBase.IsSelected = true;
                itemBase.Owner.DeselectAll();
                itemBase.ZIndex.Value = itemBase.Owner.Items.Count;
                ((AdornedElement as DesignerCanvas).DataContext as IDiagramViewModel).AddItemCommand.Execute(itemBase);

                _startPoint = null;
                _endPoint = null;
            }

            e.Handled = true;
        }
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

            if (_startPoint.HasValue && _endPoint.HasValue)
                dc.DrawRectangle(Brushes.Transparent, _rectanglePen, new Rect(_startPoint.Value, _endPoint.Value));
        }
    }
}
