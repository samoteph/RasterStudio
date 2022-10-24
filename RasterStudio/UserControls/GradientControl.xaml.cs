using Atari.Images;
using System;
using System.Collections.Generic;
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
    public sealed partial class GradientControl : UserControl
    {
        public GradientControl()
        {
            this.InitializeComponent();

            this.PointerEntered += GradientControl_PointerEntered;
            this.PointerExited += GradientControl_PointerExited;
        }

        private void GradientControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }

        private void GradientControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 0);
        }

        public string Text
        {
            get
            {
                return this.TextBlockText.Text;
            }

            set
            {
                this.TextBlockText.Text = value;
            }
        }


        public AtariColor ColorStart
        {
            get
            {
                return this.colorStart;
            }

            set
            {
                if(this.colorStart.Color != value.Color)
                {
                    colorStart = value;
                    this.RefreshBackground();
                }
            }
        }

        private AtariColor colorStart;

        public AtariColor ColorStop
        {
            get
            {
                return this.colorStop;
            }

            set
            {
                if(this.colorStop.Color != value.Color)
                {
                    colorStop = value;
                    this.RefreshBackground();
                }
            }
        }

        private AtariColor colorStop;

        /// <summary>
        /// Refresh background
        /// </summary>

        private void RefreshBackground()
        {
            var c1 = Color.FromArgb(0xFF, (byte)(this.colorStart.R * 32), (byte)(this.colorStart.G * 32), (byte)(this.colorStart.B * 32));
            var c2 = Color.FromArgb(0xFF, (byte)(this.colorStop.R * 32), (byte)(this.colorStop.G * 32), (byte)(this.colorStop.B * 32));

            var gradient = new LinearGradientBrush();

            gradient.StartPoint = new Point(0, 0);
            gradient.EndPoint = new Point(1, 0);

            gradient.GradientStops.Add(new GradientStop() { Color = c1, Offset = 0 });
            gradient.GradientStops.Add(new GradientStop() { Color = c2, Offset = 1 });

            this.GridColor.Background = gradient;
        }
    }
}
