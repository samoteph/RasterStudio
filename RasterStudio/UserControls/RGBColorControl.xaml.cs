using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RasterStudio.UserControls
{
    public sealed partial class RGBColorControl : UserControl
    {
        private static bool isPressed = false;

        public RGBColorControl()
        {
            this.InitializeComponent();

            this.BorderRGB.PointerPressed += OnPressed;
            this.BorderRGB.PointerReleased += OnReleased;
            this.BorderRGB.PointerEntered += OnEntered;
        }

        private void OnEntered(object sender, PointerRoutedEventArgs e)
        {
            if(isPressed)
            {
                this.IsSelected = true;
            }
        }

        private void OnPressed(object sender, PointerRoutedEventArgs e)
        {
            this.IsSelected = true;
            isPressed = true;
        }

        private void OnReleased(object sender, PointerRoutedEventArgs e)
        {
            isPressed = false;
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(RGBColorControl), new PropertyMetadata(false, OnIsSelectedChange));

        private static void OnIsSelectedChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RGBColorControl;
            bool isSelected = (bool)e.NewValue;

            if (isSelected)
            {
                control.BorderRGB.BorderThickness = new Thickness(1);
            }
            else
            {
                control.BorderRGB.BorderThickness = new Thickness(0);
            }

            control.IsSelectedChanged?.Invoke(control, EventArgs.Empty);
        }

        public event EventHandler IsSelectedChanged;



        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(RGBColorControl), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnColorChange));

        private static void OnColorChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RGBColorControl;
            Brush color = (Brush)e.NewValue;

            control.BorderRGB.Background = color;
        }
    }
}
