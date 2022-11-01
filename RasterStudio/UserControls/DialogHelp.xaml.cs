using RasterStudio.Models;
using SamuelBlanchard.Audio;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
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
    public sealed partial class DialogHelp : UserControl
    {
        DispatcherTimer timer;

        enum MyAudioKeys
        {
            Loop,
        }
        AudioPlayer<MyAudioKeys> audioPlayer = new AudioPlayer<MyAudioKeys>();

        public DialogHelp()
        {
            this.InitializeComponent();
            this.DialogContainer.Visibility = Visibility.Collapsed;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
        
            this.ScrollViewer.PointerPressed += OnScrollViewerPointerPressed;
            this.ScrollViewer.PointerReleased += OnScrollViewerPointerReleased;

            this.Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await audioPlayer.InitializeAsync();
            await audioPlayer.AddSoundFromApplicationAsync(MyAudioKeys.Loop, "ms-appx:///Assets/Mp3/WB.mp3");
            audioPlayer.Volume = 0.25;
        }

        private void OnScrollViewerPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            timer.Stop();
        }

        private void OnScrollViewerPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            timer.Start();
        }

        private void OnTimer(object sender, object e)
        {
            timer.Stop();

            if (this.ScrollViewer.ScrollableHeight != 0)
            {
                if (this.ScrollViewer.VerticalOffset == this.ScrollViewer.ScrollableHeight)
                {
                    this.ScrollViewer.ChangeView(null, 0, null, true);
                }
                else
                {
                    this.ScrollViewer.ChangeView(null, this.ScrollViewer.VerticalOffset + 1, null, true);
                }
            }
            
            timer.Start();
        }

        /// <summary>
        /// IsOpen
        /// </summary>

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DialogTemplate), new PropertyMetadata(false, OnIsOpenChange));

        private static void OnIsOpenChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as DialogHelp;

            bool isOpen = ((bool)e.NewValue);

            c.DialogContainer.Visibility = isOpen ? Visibility.Visible : Visibility.Collapsed;

            MainPage.Instance.OpenHelpDialog(isOpen);

            if(isOpen == true)
            {
                c.ScrollViewer.ChangeView(null, 0, null, true);
                c.audioPlayer.PlayLoop(MyAudioKeys.Loop);
                c.timer.Tick += c.OnTimer;
                c.timer.Start();
            }
            else
            {
                c.audioPlayer.Stop(MyAudioKeys.Loop);
                c.timer.Tick -= c.OnTimer;
                c.timer.Stop();
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
        }

        private async void TextBlockFacebook_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://www.facebook.com/groups/383252337204206)"));
        }

        private async void TextBlockRasterStudio_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://en.wikipedia.org/wiki/Raster_bar"));
        }

        private async void TextBlockTwitter_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://twitter.com/samoteph"));
        }
    }
}
