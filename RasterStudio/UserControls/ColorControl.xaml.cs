using Atari.Images;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    public sealed partial class ColorControl : UserControl
    {
        public ColorControl()
        {
            this.InitializeComponent();

            this.ColorPicker.DataContext = this;

            this.PointerEntered += OnPointerEntered;
            this.PointerExited += OnPointerExited;
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 0);
        }

        private void ColorTapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(LayoutRoot);
        }

        public AtariColor AtariColor
        {
            get { return (AtariColor)GetValue(AtariColorProperty); }
            set { SetValue(AtariColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AtariColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AtariColorProperty =
            DependencyProperty.Register("AtariColor", typeof(AtariColor), typeof(ColorControl), new PropertyMetadata(AtariColor.Black, OnAtariColorChanged));

        private static void OnAtariColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorControl cc = (ColorControl)d;

            AtariColor c = (AtariColor)e.NewValue;
            
            Color rgbColor = Color.FromArgb(0xFF, (byte)(c.R * 32), (byte)(c.G * 32), (byte)(c.B * 32));
            
            cc.GridColor.Background = new SolidColorBrush(rgbColor);

            cc.AtariColorChanged?.Invoke(cc, EventArgs.Empty);


            cc.TextBlockText.Text = $"${c.Color.ToString("X4")}";
            cc.TextBlockRGB.Text = $"#{rgbColor.R.ToString("X2")}{rgbColor.G.ToString("X2")}{rgbColor.B.ToString("X2")}";
        }

        public event EventHandler AtariColorChanged;
    }
}
