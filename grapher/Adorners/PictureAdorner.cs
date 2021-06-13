﻿using grapher.Controls;
using grapher.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace grapher.Adorners
{
    public class PictureAdorner : Adorner
    {
        private Point? _startPoint;
        private Point? _endPoint;
        private string _filename;

        private DesignerCanvas _designerCanvas;

        public PictureAdorner(DesignerCanvas designerCanvas, Point? dragStartPoint, string filename)
            : base(designerCanvas)
        {
            _designerCanvas = designerCanvas;
            _startPoint = dragStartPoint;
            _filename = filename;
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
                PictureDesignerItemViewModel itemBase = new PictureDesignerItemViewModel();
                itemBase.FileName = _filename;
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

            var brush = new ImageBrush(new BitmapImage(new Uri(_filename)));
            brush.Opacity = 0.5;

            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

            if (_startPoint.HasValue && _endPoint.HasValue)
                dc.DrawRectangle(brush, null, new Rect(_startPoint.Value, _endPoint.Value));
        }
    }
}