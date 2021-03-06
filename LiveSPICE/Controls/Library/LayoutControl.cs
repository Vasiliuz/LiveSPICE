﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Reflection;

namespace LiveSPICE
{
    /// <summary>
    /// Control that displays a circuit symbol layout.
    /// </summary>
    public class LayoutControl : Control, INotifyPropertyChanged
    {
        static LayoutControl() { DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutControl), new FrameworkPropertyMetadata(typeof(LayoutControl))); }

        private bool showText = true;
        public bool ShowText { get { return showText; } set { showText = value; InvalidateVisual(); NotifyChanged("ShowText"); } }

        private Circuit.SymbolLayout layout = null;
        public Circuit.SymbolLayout Layout { get { return layout; } set { layout = value; InvalidateVisual(); NotifyChanged("Layout"); } }
        
        protected override Size MeasureOverride(Size constraint)
        {
            if (layout == null)
                return base.MeasureOverride(constraint);
            return new Size(
                Math.Min(layout.Width, constraint.Width),
                Math.Min(layout.Height, constraint.Height));
        }
        
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (layout == null)
                base.OnRender(drawingContext);

            Circuit.Coord center = (layout.LowerBound + layout.UpperBound) / 2;
            double scale = Math.Min(Math.Min(ActualWidth / layout.Width, ActualHeight / layout.Height), 1.0);
            
            Matrix transform = new Matrix();
            transform.Translate(-center.x, -center.y);
            transform.Scale(scale, -scale);
            transform.Translate(ActualWidth / 2, ActualHeight / 2);

            SymbolControl.DrawLayout(layout, drawingContext, transform, ShowText ? FontFamily : null, FontWeight, FontSize);
        }

        public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register(
            "Layout",
            typeof(Circuit.SymbolLayout),
            typeof(LayoutControl),
            new PropertyMetadata(default(Circuit.SymbolLayout), OnComponentPropertyChanged));

        private static void OnComponentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LayoutControl target = d as LayoutControl;
            target.Layout = (Circuit.SymbolLayout)e.NewValue;
        }

        // INotifyPropertyChanged.
        protected void NotifyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
