﻿using boilersGraphics.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boilersGraphics.ViewModels
{
    public class NEllipseViewModel : DesignerItemViewModelBase
    {
        public NEllipseViewModel()
            : base()
        {
            Init();
        }

        public NEllipseViewModel(double left, double top, double width, double height)
            : base()
        {
            Init();
            Left.Value = left;
            Top.Value = top;
            Width.Value = width;
            Height.Value = height;
        }

        public NEllipseViewModel(double left, double top, double width, double height, double angleInDegrees)
            : this(left, top, width, height)
        {
            RotationAngle.Value = angleInDegrees;
            Matrix.Value.RotateAt(angleInDegrees, 0, 0);
        }

        public NEllipseViewModel(int id, IDiagramViewModel parent, double left, double top)
            : base(id, parent, left, top)
        {
            Init();
        }

        private void Init()
        {
            this.ShowConnectors = false;
        }

        #region IClonable

        public override object Clone()
        {
            var clone = new NEllipseViewModel();
            clone.Owner = Owner;
            clone.Left.Value = Left.Value;
            clone.Top.Value = Top.Value;
            clone.Width.Value = Width.Value;
            clone.Height.Value = Height.Value;
            clone.EdgeColor = EdgeColor;
            clone.FillColor = FillColor;
            clone.EdgeThickness = EdgeThickness;
            clone.Matrix.Value = Matrix.Value;
            clone.RotationAngle.Value = RotationAngle.Value;
            clone.PathGeometry.Value = GeometryCreator.CreateEllipse(clone);
            return clone;
        }

        #endregion //IClonable
    }
}
