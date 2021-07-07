﻿using boilersGraphics.Extensions;
using boilersGraphics.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace boilersGraphics.Controls
{
    public class ResizeThumb : SnapPoint
    {
        private Dictionary<Point, Adorner> _adorners;

        public ResizeThumb()
        {
            _adorners = new Dictionary<Point, Adorner>();
            base.DragDelta += new DragDeltaEventHandler(ResizeThumb_DragDelta);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            (App.Current.MainWindow.DataContext as MainWindowViewModel).CurrentOperation.Value = "リサイズ";
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            (App.Current.MainWindow.DataContext as MainWindowViewModel).CurrentOperation.Value = "";
            (App.Current.MainWindow.DataContext as MainWindowViewModel).Details.Value = "";
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = this.DataContext as DesignerItemViewModelBase;

            if (designerItem != null && designerItem.IsSelected)
            {
                double minLeft, minTop, minDeltaHorizontal, minDeltaVertical;
                double dragDeltaVertical, dragDeltaHorizontal;

                // only resize DesignerItems
                var selectedDesignerItems = from item in designerItem.Owner.SelectedItems
                                            where item is DesignerItemViewModelBase
                                            select item;

                CalculateDragLimits(selectedDesignerItems, out minLeft, out minTop,
                                    out minDeltaHorizontal, out minDeltaVertical);

                var mainWindowVM = (App.Current.MainWindow.DataContext as MainWindowViewModel);
                var designerCanvas = App.Current.MainWindow.GetChildOfType<DesignerCanvas>();
                var correspondingViews = designerCanvas.GetCorrespondingViews<ResizeThumb>(this.DataContext);
                var diagramVM = mainWindowVM.DiagramViewModel;
                var snapPoints = diagramVM.GetSnapPoints(new List<SnapPoint>(correspondingViews));
                Point? snapped = null;
                
                foreach (var item in selectedDesignerItems)
                {
                    if (item is DesignerItemViewModelBase)
                    {
                        var viewModel = item as DesignerItemViewModelBase;
                        if (viewModel is PictureDesignerItemViewModel &&
                            ((Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) == KeyStates.Down ||
                             (Keyboard.GetKeyStates(Key.RightShift) & KeyStates.Down) == KeyStates.Down))
                        {
                            var picViewModel = viewModel as PictureDesignerItemViewModel;
                            if (base.VerticalAlignment == VerticalAlignment.Top && base.HorizontalAlignment == HorizontalAlignment.Left)
                            {
                                double left = picViewModel.Left.Value;
                                dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                                picViewModel.Left.Value = left + dragDeltaHorizontal;
                                picViewModel.Width.Value = picViewModel.Width.Value - dragDeltaHorizontal;
                                picViewModel.Height.Value = (picViewModel.Width.Value / picViewModel.FileWidth) * picViewModel.FileHeight;
                                picViewModel.Top.Value = picViewModel.Bottom.Value - picViewModel.Height.Value;
                            }
                            else if (base.VerticalAlignment == VerticalAlignment.Top && base.HorizontalAlignment == HorizontalAlignment.Right)
                            {
                                double top = picViewModel.Top.Value;
                                dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                                picViewModel.Top.Value = top + dragDeltaVertical;
                                picViewModel.Height.Value = picViewModel.Height.Value - dragDeltaVertical;
                                picViewModel.Width.Value = (picViewModel.Height.Value / picViewModel.FileHeight) * picViewModel.FileWidth;
                            }
                            else if (base.VerticalAlignment == VerticalAlignment.Bottom && base.HorizontalAlignment == HorizontalAlignment.Left)
                            {
                                double left = picViewModel.Left.Value;
                                dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                                picViewModel.Left.Value = left + dragDeltaHorizontal;
                                picViewModel.Width.Value = picViewModel.Width.Value - dragDeltaHorizontal;
                                picViewModel.Height.Value = (picViewModel.Width.Value / picViewModel.FileWidth) * picViewModel.FileHeight;
                            }
                            else if (base.VerticalAlignment == VerticalAlignment.Bottom && base.HorizontalAlignment == HorizontalAlignment.Right)
                            {
                                dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                                picViewModel.Height.Value = picViewModel.Height.Value - dragDeltaVertical;
                                picViewModel.Width.Value = (picViewModel.Height.Value / picViewModel.FileHeight) * picViewModel.FileWidth;
                            }
                        }
                        else if (viewModel is NEllipseViewModel &&
                            ((Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) == KeyStates.Down ||
                             (Keyboard.GetKeyStates(Key.RightShift) & KeyStates.Down) == KeyStates.Down))
                        {
                            var ellipse = viewModel as NEllipseViewModel;
                            if (base.VerticalAlignment == VerticalAlignment.Top && base.HorizontalAlignment == HorizontalAlignment.Left)
                            {
                                double left = ellipse.Left.Value;
                                dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                                ellipse.Left.Value = left + dragDeltaHorizontal;
                                ellipse.Width.Value = ellipse.Width.Value - dragDeltaHorizontal;
                                ellipse.Height.Value = ellipse.Width.Value - dragDeltaHorizontal;
                                ellipse.Top.Value = ellipse.Bottom.Value - ellipse.Height.Value;
                            }
                            else if (base.VerticalAlignment == VerticalAlignment.Top && base.HorizontalAlignment == HorizontalAlignment.Right)
                            {
                                double top = ellipse.Top.Value;
                                dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                                ellipse.Top.Value = top + dragDeltaVertical;
                                ellipse.Height.Value = ellipse.Height.Value - dragDeltaVertical;
                                ellipse.Width.Value = ellipse.Height.Value - dragDeltaVertical;
                            }
                            else if (base.VerticalAlignment == VerticalAlignment.Bottom && base.HorizontalAlignment == HorizontalAlignment.Left)
                            {
                                double left = ellipse.Left.Value;
                                dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                                ellipse.Left.Value = left + dragDeltaHorizontal;
                                ellipse.Width.Value = ellipse.Width.Value - dragDeltaHorizontal;
                                ellipse.Height.Value = ellipse.Width.Value - dragDeltaHorizontal;
                            }
                            else if (base.VerticalAlignment == VerticalAlignment.Bottom && base.HorizontalAlignment == HorizontalAlignment.Right)
                            {
                                dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                                ellipse.Height.Value = ellipse.Height.Value - dragDeltaVertical;
                                ellipse.Width.Value = ellipse.Height.Value - dragDeltaVertical;
                            }
                        }
                        else
                        {
                            Rect rect = new Rect(viewModel.Left.Value, viewModel.Top.Value, viewModel.Width.Value, viewModel.Height.Value);
                            dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                            dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                            Sum(ref rect, dragDeltaHorizontal, dragDeltaVertical, base.HorizontalAlignment, base.VerticalAlignment);

                            foreach (var snapPoint in snapPoints)
                            {
                                var p = GetPosition(rect, base.VerticalAlignment, base.HorizontalAlignment);
                                if (p.X > snapPoint.X - mainWindowVM.SnapPower.Value
                                 && p.X < snapPoint.X + mainWindowVM.SnapPower.Value
                                 && p.Y > snapPoint.Y - mainWindowVM.SnapPower.Value
                                 && p.Y < snapPoint.Y + mainWindowVM.SnapPower.Value)
                                {
                                    //スナップする座標を一時変数へ保存
                                    snapped = snapPoint;
                                }
                            }

                            //スナップした場合
                            if (snapped != null)
                            {
                                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(designerCanvas);
                                RemoveFromAdornerLayerAndDictionary(snapped, adornerLayer);

                                //ドラッグ終了座標を一時変数で上書きしてスナップ
                                SetRect(ref rect, snapped.Value, base.VerticalAlignment, base.HorizontalAlignment);

                                viewModel.Left.Value = rect.X;
                                viewModel.Top.Value = rect.Y;
                                viewModel.Width.Value = rect.Width;
                                viewModel.Height.Value = rect.Height;

                                if (adornerLayer != null)
                                {
                                    Trace.WriteLine($"Snap={snapped.Value}");
                                    if (!_adorners.ContainsKey(snapped.Value))
                                    {
                                        var adorner = new Adorners.SnapPointAdorner(designerCanvas, snapped.Value);
                                        if (adorner != null)
                                        {
                                            adornerLayer.Add(adorner);

                                            //ディクショナリに記憶する
                                            _adorners.Add(snapped.Value, adorner);
                                        }
                                    }
                                }
                            }
                            else //スナップしなかった場合
                            {
                                RemoveAllAdornerFromAdornerLayerAndDictionary(designerCanvas);

                                switch (base.VerticalAlignment)
                                {
                                    case VerticalAlignment.Bottom:
                                        dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                                        viewModel.Height.Value = viewModel.Height.Value - dragDeltaVertical;
                                        break;
                                    case VerticalAlignment.Top:
                                        double top = viewModel.Top.Value;
                                        dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                                        viewModel.Top.Value = top + dragDeltaVertical;
                                        viewModel.Height.Value = viewModel.Height.Value - dragDeltaVertical;
                                        break;
                                    default:
                                        break;
                                }

                                switch (base.HorizontalAlignment)
                                {
                                    case HorizontalAlignment.Left:
                                        double left = viewModel.Left.Value;
                                        dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                                        viewModel.Left.Value = left + dragDeltaHorizontal;
                                        viewModel.Width.Value = viewModel.Width.Value - dragDeltaHorizontal;
                                        break;
                                    case HorizontalAlignment.Right:
                                        dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                                        viewModel.Width.Value = viewModel.Width.Value - dragDeltaHorizontal;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        (App.Current.MainWindow.DataContext as MainWindowViewModel).Details.Value = $"(w, h) = ({viewModel.Width.Value}, {viewModel.Height.Value})";
                    }
                }
                e.Handled = true;
            }
        }

        private void DebugPrint(string windowName, Rect rect, Point? value = null)
        {
            var designerCanvas = App.Current.MainWindow.GetChildOfType<DesignerCanvas>();
            var rtb = new RenderTargetBitmap((int)designerCanvas.ActualWidth, (int)designerCanvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(designerCanvas);
                context.DrawRectangle(brush, null, new Rect(new Point(), new Size(designerCanvas.Width, designerCanvas.Height)));

                context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Blue, 1), rect);

                context.DrawText(new FormattedText($"({rect.X}, {rect.Y})", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("メイリオ"), 12, Brushes.Blue, VisualTreeHelper.GetDpi(designerCanvas).PixelsPerDip), new Point(rect.X + 10, rect.Y + 10));
                context.DrawText(new FormattedText(rect.Height.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("メイリオ"), 12, Brushes.Blue, VisualTreeHelper.GetDpi(designerCanvas).PixelsPerDip), new Point(rect.X, rect.Y + rect.Height / 2));

                if (value != null)
                    context.DrawEllipse(Brushes.Transparent, new Pen(Brushes.Red, 1), value.Value, 2, 2);
            }

            rtb.Render(visual);

            //OpenCvSharp.Cv2.ImShow()するためには src_depth != CV_16F && src_depth != CV_32S である必要があるから、予めBgr24に変換しておく
            FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
            newFormatedBitmapSource.BeginInit();
            newFormatedBitmapSource.Source = rtb;
            newFormatedBitmapSource.DestinationFormat = PixelFormats.Bgr24;
            newFormatedBitmapSource.EndInit();

            var mat = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat(newFormatedBitmapSource);
            OpenCvSharp.Cv2.ImShow(windowName, mat);
        }

        private void Sum(ref Rect rect, double dragDeltaHorizontal, double dragDeltaVertical, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    switch (horizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            rect.X += dragDeltaHorizontal;
                            rect.Y += dragDeltaVertical;
                            return;
                        case HorizontalAlignment.Center:
                            rect.Y += dragDeltaVertical;
                            return;
                        case HorizontalAlignment.Right:
                            rect.Width += dragDeltaHorizontal;
                            rect.Y += dragDeltaVertical;
                            return;
                    }
                    break;
                case VerticalAlignment.Center:
                    switch (horizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            rect.X += dragDeltaHorizontal;
                            return;
                        case HorizontalAlignment.Center:
                            return;
                        case HorizontalAlignment.Right:
                            rect.Width += dragDeltaHorizontal;
                            return;
                    }
                    break;
                case VerticalAlignment.Bottom:
                    switch (horizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            rect.X += dragDeltaHorizontal;
                            Trace.WriteLine($"rect.Y(a)={rect.Y}");
                            rect.Height += dragDeltaVertical;
                            Trace.WriteLine($"rect.Y(b)={rect.Y}");
                            return;
                        case HorizontalAlignment.Center:
                            rect.Height += dragDeltaVertical;
                            return;
                        case HorizontalAlignment.Right:
                            rect.Width += dragDeltaHorizontal;
                            rect.Height += dragDeltaVertical;
                            return;
                    }
                    break;
            }
            throw new Exception("alignment conbination is wrong");
        }

        private void SetRect(ref Rect rect, Point snapPoint, VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
        {
            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    switch (horizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            rect.X = snapPoint.X;
                            rect.Y = snapPoint.Y;
                            return;
                        case HorizontalAlignment.Center:
                            rect.X = snapPoint.X - rect.Width / 2;
                            rect.Y = snapPoint.Y;
                            return;
                        case HorizontalAlignment.Right:
                            rect.Width = snapPoint.X - rect.X;
                            rect.Y = snapPoint.Y;
                            return;
                    }
                    break;
                case VerticalAlignment.Center:
                    switch (horizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            rect.X = snapPoint.X;
                            rect.Y = snapPoint.Y - rect.Height / 2;
                            return;
                        case HorizontalAlignment.Center:
                            rect.X = snapPoint.X - rect.Width / 2;
                            rect.Y = snapPoint.Y - rect.Height / 2;
                            return;
                        case HorizontalAlignment.Right:
                            rect.Width = snapPoint.X - rect.X;
                            rect.Y = snapPoint.Y - rect.Height / 2;
                            return;
                    }
                    break;
                case VerticalAlignment.Bottom:
                    switch (horizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            rect.X = snapPoint.X;
                            rect.Height = snapPoint.Y - rect.Top;
                            return;
                        case HorizontalAlignment.Center:
                            rect.X = snapPoint.X - rect.Width / 2;
                            rect.Height = snapPoint.Y - rect.Top;
                            return;
                        case HorizontalAlignment.Right:
                            rect.Width = snapPoint.X - rect.X;
                            rect.Height = snapPoint.Y - rect.Top;
                            return;
                    }
                    break;
            }
            throw new Exception("alignment conbination is wrong");
        }

        private Point GetPosition(Rect rect, VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
        {
            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    switch (horizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            return new Point(rect.X, rect.Top);
                        case HorizontalAlignment.Center:
                            return new Point(rect.X + rect.Width / 2, rect.Top);
                        case HorizontalAlignment.Right:
                            return new Point(rect.X + rect.Width, rect.Top);
                    }
                    break;
                case VerticalAlignment.Center:
                    switch (horizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            return new Point(rect.X, rect.Top + rect.Height / 2);
                        case HorizontalAlignment.Center:
                            return new Point(rect.X + rect.Width / 2, rect.Top + rect.Height / 2);
                        case HorizontalAlignment.Right:
                            return new Point(rect.X + rect.Width, rect.Top + rect.Height / 2);
                    }
                    break;
                case VerticalAlignment.Bottom:
                    switch (horizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            return new Point(rect.X, rect.Top + rect.Height);
                        case HorizontalAlignment.Center:
                            return new Point(rect.X + rect.Width / 2, rect.Top + rect.Height);
                        case HorizontalAlignment.Right:
                            return new Point(rect.X + rect.Width, rect.Top + rect.Height);
                    }
                    break;
            }
            throw new Exception("alignment conbination is wrong");
        }

        private static void CalculateDragLimits(IEnumerable<SelectableDesignerItemViewModelBase> selectedDesignerItems, out double minLeft, out double minTop, out double minDeltaHorizontal, out double minDeltaVertical)
        {
            minLeft = double.MaxValue;
            minTop = double.MaxValue;
            minDeltaHorizontal = double.MaxValue;
            minDeltaVertical = double.MaxValue;

            // drag limits are set by these parameters: canvas top, canvas left, minHeight, minWidth
            // calculate min value for each parameter for each item
            foreach (var item in selectedDesignerItems)
            {
                if (item is DesignerItemViewModelBase)
                {
                    var viewModel = item as DesignerItemViewModelBase;
                    double left = viewModel.Left.Value;
                    double top = viewModel.Top.Value;

                    minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                    minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);

                    minDeltaVertical = Math.Min(minDeltaVertical, viewModel.Height.Value - viewModel.MinHeight);
                    minDeltaHorizontal = Math.Min(minDeltaHorizontal, viewModel.Width.Value - viewModel.MinWidth);
                }
                else if (item is ConnectorBaseViewModel)
                {
                    var viewModel = item as ConnectorBaseViewModel;
                    double left = Math.Min(viewModel.Points[0].X, viewModel.Points[1].X);
                    double top = Math.Min(viewModel.Points[0].Y, viewModel.Points[1].Y);

                    double width = Math.Max(viewModel.Points[0].X, viewModel.Points[1].X) - Math.Min(viewModel.Points[0].X, viewModel.Points[1].X);
                    double height = Math.Max(viewModel.Points[0].Y, viewModel.Points[1].Y) - Math.Min(viewModel.Points[0].Y, viewModel.Points[1].Y);

                    minDeltaVertical = Math.Min(minDeltaVertical, height);
                    minDeltaHorizontal = Math.Min(minDeltaHorizontal, width);
                }
            }
        }

        private void RemoveAllAdornerFromAdornerLayerAndDictionary(DesignerCanvas designerCanvas)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(designerCanvas);
            var removes = _adorners.ToList();

            removes.ForEach(x =>
            {
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(x.Value);
                }
                _adorners.Remove(x.Key);
            });
        }

        private void RemoveFromAdornerLayerAndDictionary(Point? snapped, AdornerLayer adornerLayer)
        {
            var removes = _adorners.Where(x => x.Key != snapped)
                                                       .ToList();
            removes.ForEach(x =>
            {
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(x.Value);
                }
                _adorners.Remove(x.Key);
            });
        }

        public override string ToString()
        {
            return base.ToString() + $" Margin={Margin}";
        }
    }
}
