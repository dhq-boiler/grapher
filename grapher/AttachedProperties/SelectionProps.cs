﻿using grapher.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace grapher.AttachedProperties
{
    public static class SelectionProps
    {
        #region EnabledForSelection

        public static readonly DependencyProperty EnabledForSelectionProperty =
            DependencyProperty.RegisterAttached("EnabledForSelection", typeof(bool), typeof(SelectionProps),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnEnabledForSelectionChanged)));

        public static bool GetEnabledForSelection(DependencyObject d)
        {
            return (bool)d.GetValue(EnabledForSelectionProperty);
        }

        public static void SetEnabledForSelection(DependencyObject d, bool value)
        {
            d.SetValue(EnabledForSelectionProperty, value);
        }

        private static void OnEnabledForSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)d;
            if ((bool)e.NewValue)
            {
                fe.PreviewMouseDown += Fe_PreviewMouseDown;
            }
            else
            {
                fe.PreviewMouseDown -= Fe_PreviewMouseDown;
            }
        }

        #endregion

        private static void Fe_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectableDesignerItemViewModelBase selectableDesignerItemViewModelBase =
                (SelectableDesignerItemViewModelBase)((FrameworkElement)sender).DataContext;

            if (selectableDesignerItemViewModelBase != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                {
                    if ((Keyboard.Modifiers & (ModifierKeys.Shift)) != ModifierKeys.None)
                    {
                        selectableDesignerItemViewModelBase.IsSelected = !selectableDesignerItemViewModelBase.IsSelected;
                    }

                    if ((Keyboard.Modifiers & (ModifierKeys.Control)) != ModifierKeys.None)
                    {
                        selectableDesignerItemViewModelBase.IsSelected = !selectableDesignerItemViewModelBase.IsSelected;
                    }
                }
                else
                {
                    foreach (SelectableDesignerItemViewModelBase item in selectableDesignerItemViewModelBase.Parent.SelectedItems)
                        item.IsSelected = false;

                    selectableDesignerItemViewModelBase.Parent.SelectedItems.Clear();
                    selectableDesignerItemViewModelBase.Parent.EdgeColors.Clear();
                    selectableDesignerItemViewModelBase.IsSelected = true;

                    Color color = Colors.Transparent;
                    if (selectableDesignerItemViewModelBase is DesignerItemViewModelBase)
                    {
                        color = (selectableDesignerItemViewModelBase as DesignerItemViewModelBase).EdgeColor;
                    }
                    else if (selectableDesignerItemViewModelBase is ConnectorBaseViewModel)
                    {
                        color = (selectableDesignerItemViewModelBase as ConnectorBaseViewModel).EdgeColor;
                    }
                    selectableDesignerItemViewModelBase.Parent.EdgeColors.Add(color);
                }
            }
        }
    }
}
