using RasterStudio.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
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
    public sealed partial class RasterSlider : UserControl
    {
        private bool isPressed = false;
        private double heightCanvas;
        private int originalLine = 0;
        private double startDistanceY;

        public RasterSlider()
        {
            this.InitializeComponent();

            this.PointerMoved += this.RasterSlider_PointerMoved;
            this.PointerReleased += this.RasterSlider_PointerReleased;
        }

        public RasterThumb SelectedRasterThumb
        {
            get { return (RasterThumb)GetValue(SelectedRasterThumbProperty); }
            set { SetValue(SelectedRasterThumbProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedRasterThumb.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedRasterThumbProperty =
            DependencyProperty.Register("SelectedRasterThumb", typeof(RasterThumb), typeof(RasterSlider), new PropertyMetadata(null, OnSelectedRasterThumbChanged));

        private static void OnSelectedRasterThumbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rasterSlider = d as RasterSlider;

            var oldRasterThumb = e.OldValue as RasterThumb;

            if(oldRasterThumb != null)
            {
                oldRasterThumb.IsSelected = false;
            }

            if (rasterSlider.SelectedRasterThumb != null)
            {
                rasterSlider.SelectedRasterThumb.IsSelected = true;
                rasterSlider.RasterThumbSelected?.Invoke(rasterSlider, EventArgs.Empty);
            }
        }

        public event EventHandler RasterThumbSelected;

        // Deplacement du thumb
        private void RasterThumbControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.heightCanvas = this.RasterThumbContainer.ActualHeight;
            var rasterThumbControl = sender as RasterThumbControl;

            // ancien thumb
            if (this.SelectedRasterThumb != null)
            {
                this.SelectedRasterThumb.IsSelected = false;
            }

            // nouveau thumb
            this.SelectedRasterThumb = rasterThumbControl.RasterThumb;

            this.SelectedRasterThumb.IsSelected = true;
            originalLine = this.SelectedRasterThumb.Line;

            isPressed = true;

            var pointY = e.GetCurrentPoint(this.RasterThumbContainer).Position.Y;
            var originalPointY = Canvas.GetTop(rasterThumbControl);

            // Cette distance doit être ajouté au move pour que le position initial du pointer ne devienne pas une line tout de suite
            this.startDistanceY = originalPointY - pointY;

            this.RasterThumbPointerPressed?.Invoke(this, EventArgs.Empty);

            this.CapturePointer(e.Pointer);
        }


        // Deplacement du thumb à partir du slider
        private void RasterSlider_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isPressed)
            {
                if (this.SelectedRasterThumb != null)
                {
                    var point = e.GetCurrentPoint(this.RasterThumbContainer);

                    var y = point.Position.Y + this.startDistanceY;

                    if (y >= 0 && y < heightCanvas)
                    {
                        this.SelectedRasterThumb.Line = (int)(MainPage.Instance.Project.Image.Height * y / heightCanvas);
                    }
                }
            }
        }

        // Release du thumb à partir du slider
        private void RasterSlider_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.ReleasePointerCaptures();
            isPressed = false;

            RasterThumbPointerReleased?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler RasterThumbPointerReleased;
        public event EventHandler RasterThumbPointerPressed;

        /// <summary>
        /// Ajout d'un control
        /// </summary>
        /// <param name="raster"></param>

        private void AddRasterThumbControl(RasterThumb rasterThumb)
        {
            var rasterThumbControl = new RasterThumbControl();

            this.RasterThumbContainer.Children.Add(rasterThumbControl);

            rasterThumbControl.RasterThumb = rasterThumb;

            rasterThumbControl.PointerPressed += RasterThumbControl_PointerPressed;

        }

        /// <summary>
        /// RasterThumbs
        /// </summary>

        public ObservableCollection<RasterThumb> RasterThumbs
        {
            get;
            private set;
        } 
        = new ObservableCollection<RasterThumb>();

        /// <summary>
        /// Changement dans la collection de Thumbs et modification dans le Canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRastersThumbChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AddRasterThumbControl(e.NewItems[0] as RasterThumb);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.RemoveRasterThumbControl(e.OldItems[0] as RasterThumb);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.ClearRasterThumbControls();                    
                    break;
            }
        }

        private RasterThumbControl FindRasterThumbControl(RasterThumb rasterThumb)
        {
            foreach(RasterThumbControl rasterThumbControl in this.RasterThumbContainer.Children)
            {
                if(rasterThumbControl.RasterThumb == rasterThumb)
                {
                    return rasterThumbControl;
                }
            }

            return null;
        }

        private void RemoveRasterThumbControl(RasterThumb rasterThumb)
        {
            // il faudrait mettre RasterThumb dans RasterThumbControl
            // pour pouvoir faire une association en RasterThumb et RasterThumbControl et également faire de la notification quand line ou la couleur change

            var thumbControl = this.FindRasterThumbControl(rasterThumb);

            this.RemoveRasterThumbControl(thumbControl);
        }

        private void RemoveRasterThumbControl(RasterThumbControl thumbControl)
        {
            thumbControl.PointerPressed -= RasterThumbControl_PointerPressed;
            this.RasterThumbContainer.Children.Remove(thumbControl);
        }

        private void ClearRasterThumbControls()
        {
            foreach(RasterThumbControl rasterThumbControl in this.RasterThumbContainer.Children)
            {
                rasterThumbControl.PointerPressed -= RasterThumbControl_PointerPressed;
            }
            
            this.RasterThumbContainer.Children.Clear();
        }

        /// <summary>
        /// Fixé le raster en cours et notamment les RasterThumbs en cours
        /// </summary>
        /// <param name="selectedRaster"></param>

        public void SetRaster(AtariRaster selectedRaster)
        {
            if(this.RasterThumbs != null)
            {
                this.RasterThumbs.CollectionChanged -= OnRastersThumbChanged;
            }

            this.RasterThumbs = selectedRaster.Thumbs;

            if (this.RasterThumbs != null)
            {
                this.RasterThumbs.CollectionChanged += OnRastersThumbChanged;
            }

            this.ClearRasterThumbControls();
            
            foreach(var rasterThumb in this.RasterThumbs)
            {
                this.AddRasterThumbControl(rasterThumb);
            }
        }

        /// <summary>
        /// Changement de taille
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RasterThumbContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (RasterThumbControl rasterThumbControl in this.RasterThumbContainer.Children)
            {
                rasterThumbControl.RefreshPositionInCanvas(e.NewSize.Height);
            }
        }
    }
}
