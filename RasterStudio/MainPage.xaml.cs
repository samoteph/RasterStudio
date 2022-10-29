using Atari.Images;
using RasterStudio.Models;
using RasterStudio.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using static System.Net.Mime.MediaTypeNames;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RasterStudio
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Instance
        {
            get;
            private set;
        }

        Project project = new Project();

        public Project Project
        {
            get
            {
                return this.project;
            }
        }

        public MainPage()
        {
            Instance = this;

            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;

            this.PaletteControl.ColorSelected += OnColorSelected;

            this.RasterSlider.RasterThumbPointerPressed += OnRasterThumbPointerPressed;
            this.RasterSlider.RasterThumbPointerReleased += OnRasterThumbPointerReleased;

            this.RasterSlider.RasterThumbSelected += RasterSlider_RasterThumbSelected;
            this.RasterControl.RasterThumbSelected += RasterControl_RasterThumbSelected;

            this.AtariImageControl.PaletteControl = this.PaletteControl;
            this.AtariImageControl.DisplayBlankScreen = false;
            this.AtariImageControl.DisplayAllRasters = true;
            this.AtariImageControl.Project = this.project;
        }

        public RasterThumb SelectedRasterThumb
        {
            get
            {
                return selectedRasterThumb;
            }

            set
            {
                if(selectedRasterThumb != value)
                {
                    if(this.selectedRasterThumb != null)
                    {
                        selectedRasterThumb.PropertyChanged -= OnSelectedRasterThumbPropertyChanged;
                    }

                    selectedRasterThumb = value;
                
                    if(selectedRasterThumb != null)
                    {
                        selectedRasterThumb.PropertyChanged += OnSelectedRasterThumbPropertyChanged;
                    }
                }
            }
        }

        private RasterThumb selectedRasterThumb;

        private void OnSelectedRasterThumbPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(RasterThumb.Color):
                    this.AtariImageControl.DrawRasterOnScreen();
                    break;
                case nameof(RasterThumb.Line):

                    // Changement de ligne

                    if (this.selectedRasterThumb != null)
                    {
                        var line = this.selectedRasterThumb.Line;

                        this.AtariImageControl.DrawRasterOnScreen();
                        this.AtariImageControl.DrawCursorLineOnScreen(line);
                    }
                    break;
                case nameof(RasterThumb.EasingMode):
                case nameof(RasterThumb.EasingFunction):
                    this.AtariImageControl.DrawRasterOnScreen();
                    break;
            }
        }

        /// <summary>
        /// Selection d'un RasterThumb
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void RasterControl_RasterThumbSelected(object sender, EventArgs e)
        {
            this.RasterSlider.SelectedRasterThumb = this.RasterControl.SelectedRasterThumb;
            this.SelectedRasterThumb = this.RasterControl.SelectedRasterThumb;
        }

        private void RasterSlider_RasterThumbSelected(object sender, EventArgs e)
        {
            this.RasterControl.SelectedRasterThumb = this.RasterSlider.SelectedRasterThumb;
            this.SelectedRasterThumb = this.RasterSlider.SelectedRasterThumb;
        }

        /// <summary>
        ///  On commence un appuie sur un RasterThumb
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void OnRasterThumbPointerPressed(object sender, EventArgs e)
        {
            // On nettoie l'ecran pour ne plus afficher le cursor
            AtariImageControl.DrawCursorLineOnScreen(this.SelectedRasterThumb.Line);
        }


        /// <summary>
        ///  On relache le RasterThumb
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void OnRasterThumbPointerReleased(object sender, EventArgs e)
        {
            // On nettoie l'ecran pour ne plus afficher le cursor
            AtariImageControl.DrawRasterOnScreen();
        }

        /// <summary>
        /// Raster selectionné correspond à un celui d'un couleur
        /// </summary>

        public AtariRaster SelectedRaster
        {
            get;
            private set;
        }

        /// <summary>
        /// Couleur selectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void OnColorSelected(object sender, AtariColorChangedArgs args)
        {
            this.project.SelectedRasterIndex = args.NewColorIndex;
            this.SelectRaster(args.NewColorIndex);
        }
        
        /// <summary>
        /// Chargement de l'application terminée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadEmptyProjectAsync();
        }

        public async Task LoadEmptyProjectAsync()
        {
            // Lecture de l'image
            var uriSource = new Uri("ms-appx:///Assets/ScreenShots/MBIDL.png");
            // Chargement du fichier
            var file = await StorageFile.GetFileFromApplicationUriAsync(uriSource);

            await LoadImageAsync(file);
        }

        public async Task<bool> LoadImageAsync(StorageFile file)
        {
            try
            {
                await this.AtariImageControl.LoadImageAsync(file);

                // démarrage des Rasters
                this.InitializeRasters(this.project.Image);

                this.SetTitleBar();

                this.NavigationView.SelectedItem = this.NavigationView.MenuItems[0];

                return true;
            }
            catch(Exception ex)
            {
                MessageDialog dialog = new MessageDialog($"An error occured during the loading of the image!\n\n{ex.Message}", "Loading image");
                await dialog.ShowAsync();
            }

            return false;
        }

        private void InitializeRasters(AtariImage image)
        {
            this.project.Rasters = new AtariRaster[this.project.Image.Palette.Length];

            for (int i = 0; i < this.project.Rasters.Length; i++)
            {
                var raster = new AtariRaster();

                raster.Initialize(this.project.Image, i);

                this.project.Rasters[i] = raster;
            }

            this.SelectRaster(0);
        }

        private void InitializePalette(AtariImage image, int selectedColorIndex)
        {
            this.PaletteControl.Palette = this.project.Image.Palette;

            // Fixe les etoiles de PaletteControl selon les informatons de Rasters
            for (int colorIndex = 0; colorIndex < image.Palette.Length; colorIndex++)
            {
                this.PaletteControl.SetHaveRasterThumbDefined(colorIndex, this.AreRasterThumbModified(colorIndex));
            }

            this.PaletteControl.SelectedColorIndex = selectedColorIndex;
        }

        private void SelectRaster(int selectedColorIndex)
        {
            if(this.project.Rasters == null)
            {
                return;
            }
            
            if(this.SelectedRaster != null )
            {
                this.SelectedRaster.Thumbs.CollectionChanged -= OnThumbsChanged;
            }

            this.SelectedRaster = this.project.Rasters[selectedColorIndex];
            
            this.RasterSlider.SetRaster(this.SelectedRaster);
            this.RasterControl.SetRaster(this.SelectedRaster);

            if (this.SelectedRaster != null)
            {
                this.SelectedRaster.Thumbs.CollectionChanged += OnThumbsChanged;
            }

            this.InitializePalette(this.project.Image, selectedColorIndex);

            this.AtariImageControl.DrawRasterOnScreen();
        }

        /// <summary>
        /// les thumbs du rasters sont-ils modifiées (position nombre et couleur)
        /// </summary>
        /// <param name="colorIndex"></param>
        /// <returns></returns>

        public bool AreRasterThumbModified(int colorIndex)
        {
            var thumbs = this.project.Rasters[colorIndex].Thumbs;

            if ( thumbs.Count > 2 )
            {
                return true;
            }

            var color = this.PaletteControl.Palette[colorIndex].Color;

            if (thumbs[0].Color.Color != color || thumbs[1].Color.Color != color)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// les couleurs du rasters sont-elles modifiées (on fait pas le test sur les 200 couleurs)
        /// </summary>
        /// <param name="colorIndex"></param>
        /// <returns></returns>

        public bool IsRasterColorModified(int colorIndex)
        {
            var color = this.PaletteControl.Palette[colorIndex].Color;
            var thumbs = this.project.Rasters[colorIndex].Thumbs;

            if (thumbs.Count > 2)
            {
                foreach(var thumb in thumbs)
                {
                    if(thumb.Color.Color != color)
                    {
                        return true;
                    }
                }

                return false;
            }

            if (thumbs[0].Color.Color != color || thumbs[1].Color.Color != color)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Changement dans la collection de Thumbs d'un AtariRaster
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void OnThumbsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var colorIndex = this.PaletteControl.SelectedColorIndex;

            this.PaletteControl.SetHaveRasterThumbDefined(colorIndex, this.AreRasterThumbModified(colorIndex));

            this.AtariImageControl.DrawRasterOnScreen();
        }

        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            this.ContentDialogNewProject.ProjectAction = ProjectActions.New;
            this.ContentDialogNewProject.IsOpen = true;
        }

        private void MenuItemModify_Click(object sender, RoutedEventArgs e)
        {
            this.ContentDialogNewProject.ProjectAction = ProjectActions.Modify;
            this.ContentDialogNewProject.IsOpen = true;
        }

        private async void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await this.project.SaveProjectAsync();
                this.SetTitleBar();
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog($"An error occured during the saving of the projet!\n\n{ex.Message}", "Saving project");
                await dialog.ShowAsync();
            }
        }

        private async void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await this.project.LoadProjectAsync();

                this.SetTitleBar();

                this.SelectRaster(this.project.SelectedRasterIndex);
            }
            catch(Exception ex)
            {
                MessageDialog dialog = new MessageDialog($"An error occured during the loading of the projet!\n\n{ex.Message}", "Loading project");
                await dialog.ShowAsync();
            }
        }

        public void SetTitleBar()
        {
            ApplicationView appView = ApplicationView.GetForCurrentView();
            appView.Title = this.project.Title?.ToUpper();
        }

        /// <summary>
        /// Affichage de tous les rasters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void MenuItemAllRasters_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem menuItem = sender as ToggleMenuFlyoutItem;

            this.AtariImageControl.DisplayAllRasters = menuItem.IsChecked;

            this.AtariImageControl.DrawRasterOnScreen();
        }

        /// <summary>
        /// Affichage du Blank Screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void MenuItemBlankScreen_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem menuItem = sender as ToggleMenuFlyoutItem;

            this.AtariImageControl.DisplayBlankScreen = menuItem.IsChecked;

            this.AtariImageControl.DrawRasterOnScreen();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true)
            {
                this.AtariImageControl.Visibility = Visibility.Collapsed;
                this.ExportControl.Visibility = Visibility.Collapsed;
                this.SettingsControl.Visibility = Visibility.Visible;
            }
            else
            {
                var item = args.SelectedItem as NavigationViewItem;

                if (item != null)
                {
                    switch (item.Tag)
                    {
                        case "Image":

                            this.AtariImageControl.Visibility = Visibility.Visible;
                            this.ExportControl.Visibility = Visibility.Collapsed;
                            this.SettingsControl.Visibility = Visibility.Collapsed;
                            break;
                        case "Export":

                            this.ExportControl.UpdatePreview();

                            this.AtariImageControl.Visibility = Visibility.Collapsed;
                            this.ExportControl.Visibility = Visibility.Visible;
                            this.SettingsControl.Visibility = Visibility.Collapsed;

                            break;

                        case "Settings":

                            this.AtariImageControl.Visibility = Visibility.Collapsed;
                            this.ExportControl.Visibility = Visibility.Collapsed;
                            this.SettingsControl.Visibility = Visibility.Visible;

                            break;
                    }
                }
            }
        }

        public void OpenProjectDialog(bool isOpen)
        {
            this.ContentApplication.IsEnabled = !isOpen;
            this.ContentDialogNewProject.IsOpen = isOpen;
        }

        /// <summary>
        /// Fermeture de l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            await ApplicationView.GetForCurrentView().TryConsolidateAsync();
        }
    }
}
