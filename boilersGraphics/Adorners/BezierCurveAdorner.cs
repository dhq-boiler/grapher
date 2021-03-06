﻿using boilersGraphics.Controls;
using boilersGraphics.Extensions;
using boilersGraphics.Helpers;
using boilersGraphics.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace boilersGraphics.Adorners
{
    class BezierCurveAdorner : Adorner
    {
        private DesignerCanvas _designerCanvas;
        private Point? _startPoint;
        private Point? _endPoint;
        private Pen _bezierCurvePen;
        private SnapAction _snapAction;

        public BezierCurveAdorner(DesignerCanvas designerCanvas, Point? dragStartPoint)
            : base(designerCanvas)
        {
            _designerCanvas = designerCanvas;
            _startPoint = dragStartPoint;
            var parent = (AdornedElement as DesignerCanvas).DataContext as IDiagramViewModel;
            var brush = new SolidColorBrush(parent.EdgeColors.First());
            brush.Opacity = 0.5;
            _bezierCurvePen = new Pen(brush, parent.EdgeThickness.Value.Value);
            _snapAction = new SnapAction();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.IsMouseCaptured)
                    this.CaptureMouse();

                //ドラッグ終了座標を更新
                _endPoint = e.GetPosition(this);
                var currentPosition = _endPoint.Value;
                _snapAction.OnMouseMove(ref currentPosition);
                _endPoint = currentPosition;

                (App.Current.MainWindow.DataContext as MainWindowViewModel).DiagramViewModel.CurrentPoint = currentPosition;
                (App.Current.MainWindow.DataContext as MainWindowViewModel).Details.Value = $"({_startPoint.Value.X}, {_startPoint.Value.Y}) - ({_endPoint.Value.X}, {_endPoint.Value.Y}) (w, h) = ({_endPoint.Value.X - _startPoint.Value.X}, {_endPoint.Value.Y - _startPoint.Value.Y})";

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

            _snapAction.OnMouseUp(this);

            if (_startPoint.HasValue && _endPoint.HasValue)
            {
                var points = new List<Point>();
                points.Add(_startPoint.Value);
                points.Add(_endPoint.Value);
                var item = new BezierCurveViewModel(_startPoint.Value, _endPoint.Value, BezierCurve.Evaluate(0.25, points), BezierCurve.Evaluate(0.75, points));
                item.Owner = (AdornedElement as DesignerCanvas).DataContext as IDiagramViewModel;
                item.EdgeColor = item.Owner.EdgeColors.First();
                item.EdgeThickness = item.Owner.EdgeThickness.Value.Value;
                item.ZIndex.Value = item.Owner.Items.Count;
                item.IsSelected = true;
                item.PathGeometry.Value = GeometryCreator.CreateBezierCurve(item);
                item.Owner.DeselectAll();
                ((AdornedElement as DesignerCanvas).DataContext as IDiagramViewModel).AddItemCommand.Execute(item);

                _startPoint = null;
                _endPoint = null;
            }

            (App.Current.MainWindow.DataContext as MainWindowViewModel).CurrentOperation.Value = "";
            (App.Current.MainWindow.DataContext as MainWindowViewModel).Details.Value = "";

            e.Handled = true;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

            if (_startPoint.HasValue && _endPoint.HasValue)
            {
                var diff = _endPoint.Value - _startPoint.Value;
                var points = new List<Point>();
                points.Add(_startPoint.Value);
                points.Add(_endPoint.Value);
                var width = points.Select(x => x.X).Max() - points.Select(x => x.X).Min();
                var height = points.Select(x => x.Y).Max() - points.Select(x => x.Y).Min();
                var geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;
                using (StreamGeometryContext ctx = geometry.Open())
                {
                    ctx.BeginFigure(_startPoint.Value, true, false);
                    ctx.BezierTo(BezierCurve.Evaluate(0.25, points), BezierCurve.Evaluate(0.75, points), _endPoint.Value, true, false);
                }
                dc.DrawGeometry(Brushes.Transparent, _bezierCurvePen, geometry);
            }
        }
    }
}