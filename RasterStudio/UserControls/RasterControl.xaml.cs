using Atari.Images;
using Microsoft.Graphics.Canvas;
using RasterStudio.Models;
using RasterStudio.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public sealed partial class RasterControl : UserControl
    {
        public RasterControl()
        {
            this.InitializeComponent();

            this.Loaded += RasterControl_Loaded;
        }

        private void RasterControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.ListBoxRasterThumbs.Focus(FocusState.Programmatic);
            this.ComboBoxEasingFunction.ItemsSource = Enum.GetValues(typeof(EasingFunction));
            this.ComboBoxEasingMode.ItemsSource = Enum.GetValues(typeof(EasingMode));
        }

        public ObservableCollection<RasterThumb> RasterThumbs
        {
            get;
            private set;
        } = null;

        /// <summary>
        /// Fixé le raster en cours et notamment les RasterThumbs en cours
        /// </summary>
        /// <param name="selectedRaster"></param>

        public void SetRaster(AtariRaster selectedRaster)
        {
            this.RasterThumbs = selectedRaster.Thumbs;

            this.ListBoxRasterThumbs.ItemsSource = this.RasterThumbs;
            this.ListBoxRasterThumbs.SelectedIndex = 0;
        }

        public RasterThumb SelectedRasterThumb
        {
            get { return (RasterThumb)GetValue(SelectedRasterThumbProperty); }
            set { SetValue(SelectedRasterThumbProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedRasterThumb.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedRasterThumbProperty =
            DependencyProperty.Register("SelectedRasterThumb", typeof(RasterThumb), typeof(RasterControl), new PropertyMetadata(null, OnSelectedRasterThumbChanged));


        private static void OnSelectedRasterThumbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rasterControl = d as RasterControl;

            rasterControl.SelectRasterThumb(e.OldValue as RasterThumb, e.NewValue as RasterThumb);
        }

        private void SelectRasterThumb(RasterThumb oldRasterThumb, RasterThumb newRasterThumb)
        {
            bool canDelete = true;

            if (oldRasterThumb != null)
            {
                oldRasterThumb.PropertyChanged -= this.OnRasterThumbPropertyChanged;
            }

            if (newRasterThumb != null)
            {
                if (newRasterThumb.IsEdge)
                {
                    canDelete = false;
                }

                newRasterThumb.PropertyChanged += OnRasterThumbPropertyChanged;
            }

            this.ButtonDeleteRasterThumb.IsEnabled = canDelete;
            this.RasterThumbSelected?.Invoke(this, EventArgs.Empty);
        }

        private void OnRasterThumbPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RasterThumb newRasterThumb = sender as RasterThumb;

            switch(e.PropertyName)
            {
                case nameof(RasterThumb.Line):


                    //Debug.WriteLine($"Start");

                    bool isMoved = false;

                    for (int i = 0; i < this.RasterThumbs.Count; i++)
                    {
                        var thumb = this.RasterThumbs[i];

                        //Debug.WriteLine($"thumb.Line={thumb.Line} newRasterThumb Line={newRasterThumb.Line}");

                        if (thumb.Line > newRasterThumb.Line)
                        {
                            var oldIndex = this.RasterThumbs.IndexOf(newRasterThumb);

                            int newIndex = 0;

                            if(oldIndex > i)
                            {
                                newIndex = Math.Max(0, i);
                            }
                            else
                            {
                                newIndex = Math.Max(0, i - 1);
                            }

                            // Le Edge du bas qui reviens en position initial ne fonctionne pas 

                            if (oldIndex != newIndex)
                            {
                                //Debug.WriteLine($"Move from={oldIndex} to={newIndex} (i={i})");

                                this.RasterThumbs.Move(oldIndex, newIndex);
                                this.SelectedRasterThumb = newRasterThumb;
                            }

                            isMoved = true;

                            break;
                        }
                    }

                    if(isMoved == false)
                    {
                        //Debug.WriteLine($"Move END");
                        var oldIndex = this.RasterThumbs.IndexOf(newRasterThumb);
                        var newIndex = this.RasterThumbs.Count - 1;

                        // Le Edge du bas qui revient en position initial ne fonctionne pas 

                        if (oldIndex != newIndex)
                        {
                            //Debug.WriteLine($"Move END from={oldIndex} to={newIndex}");

                            this.RasterThumbs.Move(oldIndex, newIndex);
                            this.SelectedRasterThumb = newRasterThumb;
                        }
                    }

                    //Debug.WriteLine($"Stop");

                    break;
            }
        }

        public event EventHandler RasterThumbSelected;

        /// <summary>
        /// KeyDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                this.ListBoxRasterThumbs.Focus(FocusState.Programmatic);
            }
        }

        private void TextBox_LosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            string lineString = this.TextBoxLine.Text;
        
            if(int.TryParse(lineString, out int line))
            {
                if(line < 0 )
                {
                    line = 0;
                }
                else if(line > 199)
                {
                    line = 199;
                }

                if (this.SelectedRasterThumb != null)
                {
                    this.SelectedRasterThumb.Line = line;
                }
            }
            else
            {
                args.Cancel = true;
            }
        }

        private void ButtonAddRasterThumb_Click(object sender, RoutedEventArgs e)
        {

            var newRasterThumb = new RasterThumb();            
            newRasterThumb.Fill(100, this.SelectedRasterThumb.Color, EasingFunction.Linear, false);

            bool isAdded = false;

            for(int i = 0; i < this.RasterThumbs.Count; i++)
            {
                if (this.RasterThumbs[i].Line > newRasterThumb.Line)
                {
                    this.RasterThumbs.Insert(i, newRasterThumb);
                    isAdded = true;
                    break;
                }
            }

            if(isAdded == false)
            {
                this.RasterThumbs.Add(newRasterThumb);
            }

            this.ListBoxRasterThumbs.SelectedItem = newRasterThumb;
        }

        private void ButtonRemoveRasterThumb_Click(object sender, RoutedEventArgs e)
        {
            this.RasterThumbs.Remove(this.SelectedRasterThumb);
            this.SelectedRasterThumb = this.RasterThumbs[0];
        }
    }
}
