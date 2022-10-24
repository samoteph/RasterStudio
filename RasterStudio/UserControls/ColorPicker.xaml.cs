using Atari.Images;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.VoiceCommands;
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
using Windows.UI.Xaml.Shapes;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace RasterStudio.UserControls
{
    public sealed partial class ColorPicker : UserControl
    {
        private RGBColorControl selectedRGBColorControl;
        private RGBColorControl selectedBlueColorControl;

        public ColorPicker()
        {
            this.InitializeComponent();

            // Square Colors (R et B)

            for (int i = 0; i < 8; i++)
            {
                this.GridSquareColors.RowDefinitions.Add(new RowDefinition());
                this.GridSquareColors.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var rectangle = new RGBColorControl();

                    // Le premier rectangle est selectionnée
                    if(selectedRGBColorControl == null)
                    {
                        selectedRGBColorControl = rectangle;
                        rectangle.IsSelected = true;
                    }

                    // selection
                    rectangle.IsSelectedChanged += (s, e) =>
                    {
                        var rgbColorControl = s as RGBColorControl;

                        this.SelectRGBColorControl(rgbColorControl);
                    };

                    var r = (byte)(x * 32);
                    var g = (byte)(y * 32);
                    var b = (byte)0;

                    rectangle.Color = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, r, g, b));
                    
                    Grid.SetColumn(rectangle, x);
                    Grid.SetRow(rectangle, y);

                    this.GridSquareColors.Children.Add(rectangle);
                }
            }

            // Colors B

            for (int i = 0; i < 8; i++)
            {
                this.GridBlueColors.RowDefinitions.Add(new RowDefinition());
                var rectangle = new RGBColorControl();

                var r = (byte)0;
                var g = (byte)0;
                var b = (byte)(i*32);

                rectangle.Color = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, r, g, b));

                // Le premier rectangle est selectionnée
                if (selectedBlueColorControl == null)
                {
                    selectedBlueColorControl = rectangle;
                    rectangle.IsSelected = true;
                }

                // Selection

                rectangle.IsSelectedChanged += (s, e) =>
                {
                    var rgbColorControl = s as RGBColorControl;

                    this.SelectBlueColorControl(rgbColorControl);
                };

                Grid.SetRow(rectangle, i);

                this.GridBlueColors.Children.Add(rectangle);
            }

            // Couleur selectionnée

            this.GridColor.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF,0,0,0));

            this.TextBoxR3.Text = "0";
            this.TextBoxG3.Text = "0";
            this.TextBoxB3.Text = "0";

            this.TextBoxR3.GotFocus += TextBoxColorComponent_GotFocus;
            this.TextBoxG3.GotFocus += TextBoxColorComponent_GotFocus;
            this.TextBoxB3.GotFocus += TextBoxColorComponent_GotFocus;
        }

        private void TextBoxColorComponent_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;

            tb.SelectAll();
        }

        private void SelectBlueColorControl(RGBColorControl rgbColorControl)
        {
            if (rgbColorControl.IsSelected == true)
            {
                if (selectedBlueColorControl != null)
                {
                    selectedBlueColorControl.IsSelected = false;
                }

                selectedBlueColorControl = rgbColorControl;

                this.SliderB.Value = Grid.GetRow(rgbColorControl);

                this.RefreshRGBColor();
            }
        }

        private void SelectRGBColorControl(RGBColorControl rgbColorControl)
        {
            if (rgbColorControl.IsSelected == true)
            {
                if (selectedRGBColorControl != null)
                {
                    selectedRGBColorControl.IsSelected = false;
                }

                selectedRGBColorControl = rgbColorControl;
                this.SetAtariColor(rgbColorControl.Color);

                this.SliderR.Value = Grid.GetColumn(rgbColorControl);
                this.SliderG.Value = Grid.GetRow(rgbColorControl);
            }
        }

        private void SetAtariColor(Brush brush)
        {
            this.GridColor.Background = brush;

            var c = ((SolidColorBrush)brush).Color;
            this.AtariColor = new AtariColor(c.R / 32, c.G / 32, c.B / 32);

            var c3 = $"${this.AtariColor.Color.ToString("X4")}";
            var c8 = $"#{c.R.ToString("X2")}{c.G.ToString("X2")}{c.B.ToString("X2")}";

            this.TextBlockText.Text = c3 + " " + c8;
        }

        private void RefreshRGBColor()
        {
            foreach (RGBColorControl color in GridSquareColors.Children)
            {
                var x = Grid.GetColumn(color);
                var y = Grid.GetRow(color);

                var z = Grid.GetRow(selectedBlueColorControl);

                var r = (byte)(x * 32);
                var g = (byte)(y * 32);
                var b = (byte)(z * 32);

                var c = Windows.UI.Color.FromArgb(0xFF, r, g, b);
                ((SolidColorBrush)color.Color).Color = c;
            }

            this.SetAtariColor(this.selectedRGBColorControl.Color);
        }

        private void SetColor(int r3, int g3, int b3)
        {
            foreach(RGBColorControl color in GridSquareColors.Children)
            {
                var x = Grid.GetColumn(color);
                var y = Grid.GetRow(color);

                if(x == r3 && y == g3)
                {
                    color.IsSelected = true;
                                        
                    break;
                }
            }

            foreach (RGBColorControl color in GridBlueColors.Children)
            {
                var z = Grid.GetRow(color);

                if(z == b3)
                {
                    color.IsSelected = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Atari Color
        /// </summary>

        public AtariColor AtariColor
        {
            get { return (AtariColor)GetValue(AtariColorProperty); }
            set { SetValue(AtariColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AtariColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AtariColorProperty =
            DependencyProperty.Register("AtariColor", typeof(AtariColor), typeof(ColorPicker), new PropertyMetadata(AtariColor.Black ,OnAtariColorChanged));

        private static void OnAtariColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ColorPicker;

            var c = (AtariColor)e.NewValue;
            control.SetColor(c.R, c.G, c.B);

            control.AtariColorChanged?.Invoke(control, EventArgs.Empty);
        }

        public event EventHandler AtariColorChanged;

        /// <summary>
        /// Changement des valeurs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void SliderR_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var r = (int)e.NewValue;
            var g = Grid.GetRow(this.selectedRGBColorControl);
            var b = Grid.GetRow(this.selectedBlueColorControl);

            this.TextBoxR3.Text = r.ToString();

            this.SetColor(r, g, b);
        }

        private void SliderG_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var r = Grid.GetColumn(this.selectedRGBColorControl);
            var g = (int)e.NewValue;
            var b = Grid.GetRow(this.selectedBlueColorControl);

            this.TextBoxG3.Text = g.ToString();

            this.SetColor(r, g, b);
        }

        private void SliderB_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var r = Grid.GetColumn(this.selectedRGBColorControl);
            var g = Grid.GetRow(this.selectedRGBColorControl);
            var b = (int)e.NewValue;

            this.TextBoxB3.Text = b.ToString();

            this.SetColor(r, g, b);
        }

        private void TextBoxR3_LostFocus(object sender, RoutedEventArgs e)
        {
            this.ValidateTextBoxValue(sender as TextBox, this.SliderR);
        }

        private void TextBoxG3_LostFocus(object sender, RoutedEventArgs e)
        {
            this.ValidateTextBoxValue(sender as TextBox, this.SliderG);
        }

        private void TextBoxB3_LostFocus(object sender, RoutedEventArgs e)
        {
            this.ValidateTextBoxValue(sender as TextBox, this.SliderB);
        }

        private void ValidateTextBoxValue(TextBox textbox, Slider slider)
        {
            if (int.TryParse(textbox.Text, out int colorComponent))
            {
                if (colorComponent < 0)
                {
                    colorComponent = 0;
                }
                else if (colorComponent > 7)
                {
                    colorComponent = 7;
                }

                slider.Value = colorComponent;
            }
            else
            {
                textbox.Text = slider.Value.ToString();
            }
        }
    }
}
