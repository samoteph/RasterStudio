using Atari.Images;
using System;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RasterStudio.UserControls
{
    public sealed partial class ColorSelectorControl : UserControl
    {
        private AtariPalette palette;

        public ColorSelectorControl(AtariPalette palette)
        {
            this.InitializeComponent();

            this.palette = palette;
        }

        public AtariColor Color
        {
            get;
            private set;
        }

        public int ColorIndex
        {
            get
            {
                return this.colorIndex;
            }

            set
            {
                if (this.colorIndex != value)
                {
                    this.colorIndex = value;
                    this.Color = this.palette[value];
                    var rgbColor = Windows.UI.Color.FromArgb(0xFF,
                        (byte)(this.Color.R * 32),
                        (byte)(this.Color.G * 32),
                        (byte)(this.Color.B * 32));

                    this.GridColor.Background = new SolidColorBrush(rgbColor);
                    this.TextBlockRGB.Text = $"#{rgbColor.R.ToString("X2")}{rgbColor.G.ToString("X2")}{rgbColor.B.ToString("X2")}";
                    this.TextBlockAtari.Text = $"$0{this.Color.R}{this.Color.G}{this.Color.B}";
                    this.TextBlockIndex.Text = $"{value}";
                }
            }
        }

        private int colorIndex = -1;


        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ColorSelectorControl), new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorControl = d as ColorSelectorControl;

            colorControl.Glyph.Visibility = ((bool)e.NewValue) == true ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool HaveRasterThumbDefined
        {
            get { return (bool)GetValue(HaveRasterThumbDefinedProperty); }
            set { SetValue(HaveRasterThumbDefinedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HaveRasterThumbDefined.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HaveRasterThumbDefinedProperty =
            DependencyProperty.Register("HaveRasterThumbDefined", typeof(bool), typeof(ColorSelectorControl), new PropertyMetadata(false, OnHaveRasterThumbDefined));

        private static void OnHaveRasterThumbDefined(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cc = d as ColorSelectorControl;

            cc.GlyphRasterThumbDefined.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
