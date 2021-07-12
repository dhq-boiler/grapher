﻿using Prism.Mvvm;
using Reactive.Bindings;
using System.Windows;

namespace boilersGraphics.Models
{
    public class Corner : BindableBase
    {
        public ReactiveProperty<int> Number { get; set; } = new ReactiveProperty<int>();

        public ReactiveProperty<double> Radius { get; set; } = new ReactiveProperty<double>();

        public ReactiveProperty<double> Angle { get; set; } = new ReactiveProperty<double>();

        public ReactiveProperty<Point> Point { get; set; } = new ReactiveProperty<Point>();

        public override bool Equals(object obj)
        {
            if (!(obj is Corner))
                return false;
            var corner = obj as Corner;
            return Number.Value.Equals(corner.Number.Value)
                && Radius.Value.Equals(corner.Radius.Value)
                && Angle.Value.Equals(corner.Angle.Value)
                && Point.Value.Equals(corner.Point.Value);
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode()
                 ^ Radius.GetHashCode()
                 ^ Angle.GetHashCode()
                 ^ Point.GetHashCode();
        }
    }
}