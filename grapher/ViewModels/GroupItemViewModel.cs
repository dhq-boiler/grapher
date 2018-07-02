﻿using grapher.Helpers;
using System;
using System.Collections.Generic;
using System.Windows;

namespace grapher.ViewModels
{
    public class GroupItemViewModel : DesignerItemViewModelBase, IObservable<GroupTransformNotification>
    {
        private double _leftOld;
        private double _topOld;
        private double _widthOld;
        private double _heightOld;
        private double _lotateAngleOld;

        public GroupItemViewModel()
            : base()
        {
            Left.Subscribe(l =>
            {
                var notification = new GroupTransformNotification()
                {
                    Type = TransformType.Move,
                    LeftChange = l - _leftOld
                };
                GroupTransformObserversOnNext(notification);
                _leftOld = l;
            });
            Top.Subscribe(t =>
            {
                var notification = new GroupTransformNotification()
                {
                    Type = TransformType.Move,
                    TopChange = t - _topOld
                };
                GroupTransformObserversOnNext(notification);
                _topOld = t;
            });
            Width.Subscribe(w =>
            {
                var notification = new GroupTransformNotification()
                {
                    Type = TransformType.Resize,
                    GroupLeftTop = new Point(Left.Value, Top.Value),
                    OldWidth = _widthOld,
                    OldHeight = _heightOld,
                    WidthChange = w - _widthOld
                };
                GroupTransformObserversOnNext(notification);
                _widthOld = w;
            });
            Height.Subscribe(h =>
            {
                var notification = new GroupTransformNotification()
                {
                    Type = TransformType.Resize,
                    GroupLeftTop = new Point(Left.Value, Top.Value),
                    OldWidth = _widthOld,
                    OldHeight = _heightOld,
                    HeightChange = h - _heightOld
                };
                GroupTransformObserversOnNext(notification);
                _heightOld = h;
            });
            RotationAngle.Subscribe(a =>
            {
                var notification = new GroupTransformNotification()
                {
                    Type = TransformType.Rotate,
                    GroupLeftTop = new Point(Left.Value, Top.Value),
                    GroupCenter = new Point(Left.Value + Width.Value / 2, Top.Value + Height.Value / 2),
                    RotateAngleChange = a - _lotateAngleOld
                };
                GroupTransformObserversOnNext(notification);

                //rotate group boundary
                //Matrix.Value.RotateAt(a - _lotateAngleOld, Left.Value + Width.Value / 2, Top.Value + Height.Value / 2);

                _lotateAngleOld = a;
            });
        }

        private void GroupTransformObserversOnNext(GroupTransformNotification notification)
        {
            observers.ForEach(x => x.OnNext(notification));
        }

        public void AddGroup(SelectableDesignerItemViewModelBase viewModel)
        {
            Subscribe(viewModel);
        }

        #region IObservable<GroupTransformNotification>

        public List<IObserver<GroupTransformNotification>> observers = new List<IObserver<GroupTransformNotification>>();

        public IDisposable Subscribe(IObserver<GroupTransformNotification> observer)
        {
            observers.Add(observer);
            return new GroupItemViewModelDisposable(this, observer);
        }

        private class GroupItemViewModelDisposable : IDisposable
        {
            private GroupItemViewModel _groupItemViewModel;
            private IObserver<GroupTransformNotification> _observer;

            public GroupItemViewModelDisposable(GroupItemViewModel groupItemViewModel, IObserver<GroupTransformNotification> observer)
            {
                _groupItemViewModel = groupItemViewModel;
                _observer = observer;
            }

            public void Dispose()
            {
                _groupItemViewModel.observers.Remove(_observer);
            }
        }

        #endregion IObservable<GroupTransformNotification>
    }
}
