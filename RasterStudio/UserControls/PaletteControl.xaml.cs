using Atari.Images;
using RasterStudio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class PaletteControl : UserControl
    {
        private AtariPalette palette = null;
        
        public PaletteControl()
        {
            this.InitializeComponent();
        }

        public AtariColor SelectedColor
        {
            get
            {
                return this.palette[this.selectedColorIndex];
            }
        }

        public int SelectedColorIndex
        {
            get
            {
                return this.selectedColorIndex;
            }

            set
            {
                if (this.selectedColorIndex != value)
                {
                    var index = this.selectedColorIndex == -1 ? 0 : this.selectedColorIndex; 
                    
                    var colorControl = this.GridPalette.Children[index] as ColorSelectorControl;

                    colorControl.IsSelected = false;

                    int oldColorIndex = index;
                    int newColorIndex = value;

                    this.selectedColorIndex = value;

                    colorControl = this.GridPalette.Children[value] as ColorSelectorControl;
                    colorControl.IsSelected = true;

                    this.ColorSelected?.Invoke(this, new AtariColorChangedArgs(oldColorIndex, newColorIndex));
                }
            }
        }

        private int selectedColorIndex = -1;

        public AtariPalette Palette
        {
            get
            {
                return this.palette;
            }

            set
            {
                if (this.palette != value)
                {
                    this.palette = value;

                    this.selectedColorIndex = -1;

                    this.CreateColorControls(value);
                }
            }
        }

        public void SetHaveRasterThumbDefined(int colorIndex, bool haveRasterThumbDefined)
        {
            ColorSelectorControl selector = GridPalette.Children[colorIndex] as ColorSelectorControl;
            selector.HaveRasterThumbDefined = haveRasterThumbDefined;
        }

        private void CreateColorControls(AtariPalette value)
        {
            this.GridPalette.Children.Clear();
            this.GridPalette.RowDefinitions.Clear();

            if (value != null)
            {
                for (int i = 0; i < palette.Length; i++)
                {
                    this.GridPalette.RowDefinitions.Add(
                        new RowDefinition()
                        {
                            Height = new GridLength(1, GridUnitType.Star)
                        }
                    );

                    var colorControl = new ColorSelectorControl(value) { ColorIndex = i };

                    colorControl.Tapped += OnColorControlTapped;

                    Grid.SetRow(colorControl, i);

                    this.GridPalette.Children.Add(colorControl);
                }

                this.SelectedColorIndex = 0;
            }
        }

        private void OnColorControlTapped(object sender, TappedRoutedEventArgs e)
        {
            var colorControl = sender as ColorSelectorControl;

            this.SelectedColorIndex = colorControl.ColorIndex;
        }

        public event EventHandler<AtariColorChangedArgs> ColorSelected;

    }

    public class AtariColorChangedArgs : EventArgs
    {
        public AtariColorChangedArgs(int oldColorIndex, int newColorIndex)
        {
            OldColorIndex = oldColorIndex;
            NewColorIndex = newColorIndex;
        }

        public int OldColorIndex
        {
            get;
            private set;
        }

        public int NewColorIndex
        {
            get;
            private set;
        }
    }
}
