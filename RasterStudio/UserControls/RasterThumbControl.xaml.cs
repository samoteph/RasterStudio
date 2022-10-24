using RasterStudio.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed partial class RasterThumbControl : UserControl
    {
        private RasterThumb rasterThumb;
        private double heightCanvas;

        public RasterThumbControl()
        {
            this.InitializeComponent();
            this.Loaded += RasterThumbControl_Loaded;
        }

        private void RasterThumbControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.heightCanvas = (this.Parent as Canvas).ActualHeight;

            this.DisplayRasterThumbLine(rasterThumb);
        }

        public RasterThumb RasterThumb 
        { 
            get
            {
                return this.rasterThumb;
            }

            set
            {
                if(rasterThumb != value)
                {
                    if(this.rasterThumb != null)
                    {
                        this.rasterThumb.PropertyChanged -= OnRasterThumbPropertyChanged;
                    }

                    this.rasterThumb = value;

                    if (value != null)
                    {
                        this.rasterThumb.PropertyChanged += OnRasterThumbPropertyChanged;

                        this.InitializeDisplayRasterThumb(this.rasterThumb);
                    }

                    if (value.IsEdge)
                    {
                        this.Tooltip.Visibility = Visibility.Visible;
                        this.Tooltip.Content = "Edge thumb that bounds rasters. All thumbs outside the two edges are not considered in exportation operation.";
                    }
                    else
                    {
                        this.Tooltip.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void InitializeDisplayRasterThumb(RasterThumb rasterThumb)
        {
            this.DisplayRasterThumbLine(rasterThumb);
            this.DisplayRasterThumbColor(rasterThumb);
            this.DisplayRasterThumbIsSelected(rasterThumb);
            this.DisplayRasterThumbIsEdge(rasterThumb);
        }

        private void DisplayRasterThumbIsEdge(RasterThumb rasterThumb)
        {
            var colorWhite = new SolidColorBrush(Colors.White);

            if (rasterThumb.IsEdge)
            {
                var colorBlue = new SolidColorBrush(Color.FromArgb(0xFF,0x40, 0x87, 0xCE));
                this.TextBlockLine.Foreground = colorWhite;
                this.GridLine.Background = colorBlue;
            }
            else
            {
                var colorBlack = new SolidColorBrush(Colors.Black);

                this.TextBlockLine.Foreground = colorBlack;
                this.GridLine.Background = colorWhite;
            }
        }

        private void DisplayRasterThumbIsSelected(RasterThumb rasterThumb)
        {
            if (rasterThumb.IsSelected)
            {
                Canvas.SetZIndex(this, 1);
                this.FontIconBackground.Opacity = 1;
                this.RectangleBackground.Opacity = 1;
                this.GridLine.Opacity = 1;
            }
            else
            {
                Canvas.SetZIndex(this, 0);
                this.FontIconBackground.Opacity = 0.35;
                this.RectangleBackground.Opacity = 0.35;
                this.GridLine.Opacity = 0.35;
            }
        }

        private void DisplayRasterThumbLine(RasterThumb rasterThumb)
        {
            this.TextBlockLine.Text = ((int)rasterThumb.Line).ToString();
            Canvas.SetTop(this, (this.RasterThumb.Line * this.heightCanvas) / 200);
        }

        private void DisplayRasterThumbColor(RasterThumb rasterThumb)
        {
            var color = Color.FromArgb(0xFF, (byte)(rasterThumb.Color.R * 32), (byte)(rasterThumb.Color.G * 32), (byte)(rasterThumb.Color.B * 32));
            this.RectangleColor.Background = new SolidColorBrush(color);
        }

        private void OnRasterThumbPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var rasterThumb = (RasterThumb)sender;

            switch(e.PropertyName)
            {
                case nameof(RasterThumb.IsSelected):
                    this.DisplayRasterThumbIsSelected(rasterThumb);
                    break;

                case nameof(RasterThumb.Line):
                    this.DisplayRasterThumbLine(rasterThumb);                    
                    break;

                case nameof(RasterThumb.Color):
                    this.DisplayRasterThumbColor(rasterThumb);
                    break;
            }
        }

        public void RefreshPositionInCanvas(double heightCanvas)
        {
            this.heightCanvas = heightCanvas;
            Canvas.SetTop(this, (this.RasterThumb.Line * this.heightCanvas) / 200);
        }
    }
}
