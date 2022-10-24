using RasterStudio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class DialogProject : UserControl
    {
        private StorageFile imageFile;

        public DialogProject()
        {
            this.InitializeComponent();
            this.DialogContainer.Visibility = Visibility.Collapsed;
        }

        public ProjectActions ProjectAction
        {
            get { return (ProjectActions)GetValue(ProjectActionProperty); }
            set { SetValue(ProjectActionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProjectAction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProjectActionProperty =
            DependencyProperty.Register("ProjectAction", typeof(ProjectActions), typeof(DialogProject), new PropertyMetadata(ProjectActions.New, OnProjectActionChange));

        private static void OnProjectActionChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as DialogProject;

            switch((ProjectActions)e.NewValue)
            {
                case ProjectActions.New:
                    c.ButtonAddImage.IsEnabled = true;
                    c.ComboBoxPalette.IsEnabled = true;
                    c.TextBlockProjectTitle.Text = "New project";
                    c.ButtonOK.Content = "Create";
                    break;
                
                case ProjectActions.Modify:
                    c.ButtonAddImage.IsEnabled = false;
                    c.ComboBoxPalette.IsEnabled = false;
                    c.TextBlockProjectTitle.Text = "Modify projet";
                    c.ButtonOK.Content = "Modify";
                    break;
            }
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
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DialogProject), new PropertyMetadata(false, OnIsOpenChange));

        private static void OnIsOpenChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as DialogProject;

            bool isOpen = ((bool)e.NewValue);

            if(isOpen == true)
            {
                c.Initialize();
            }

            c.DialogContainer.Visibility = isOpen ? Visibility.Visible : Visibility.Collapsed;

            MainPage.Instance.OpenProjectDialog(isOpen);

        }

        private void Initialize()
        {
            switch(this.ProjectAction)
            {
                case ProjectActions.New:
                    this.TextBoxProjectName.Text = String.Empty;
                    this.TextBoxImageFilename.Text = String.Empty;
                    this.ComboBoxPalette.SelectedIndex = 0;
                    break;

                case ProjectActions.Modify:
                    this.TextBoxProjectName.Text = MainPage.Instance.Project.Title;
                    this.TextBoxImageFilename.Text = this.imageFile?.Path ?? String.Empty;
                    this.ComboBoxPalette.SelectedIndex = 0;
                    break;
            }
        }

        private async void ButtonAddImage_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".png");

            this.imageFile = await picker.PickSingleFileAsync();

            if(this.imageFile != null)
            {
                this.TextBoxImageFilename.Text = this.imageFile.Path;
                
                if(string.IsNullOrEmpty(this.TextBoxProjectName.Text))
                {
                    this.TextBoxProjectName.Text = this.imageFile.DisplayName;
                }
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
        }

        private async void ButtonCreateProject_Click(object sender, RoutedEventArgs e)
        {

            switch(this.ProjectAction)
            {
                case ProjectActions.New:
                    bool isLoaded = false;

                    if (this.imageFile != null)
                    {
                        string originalTitle = MainPage.Instance.Project.Title;
                        MainPage.Instance.Project.Title = this.TextBoxProjectName.Text;

                        isLoaded = await MainPage.Instance.LoadImageAsync(this.imageFile);

                        if (isLoaded == false)
                        {
                            var project = MainPage.Instance.Project;
                            project.Title = originalTitle;
                            MainPage.Instance.SetTitleBar();
                        }
                    }
                    else
                    {
                        MainPage.Instance.Project.Title = this.TextBoxProjectName.Text;

                        await MainPage.Instance.LoadEmptyProjectAsync();

                        isLoaded = true;
                    }

                    // on ferme uniquement si ca charge correctement
                    if (isLoaded == true)
                    {
                        var project = MainPage.Instance.Project;
                        project.Filename = null;
                        project.SelectedRasterIndex = 0;

                        this.IsOpen = false;
                    }
                    break;

                case ProjectActions.Modify:
                    
                    MainPage.Instance.Project.Title = this.TextBoxProjectName.Text;
                    MainPage.Instance.SetTitleBar();

                    this.IsOpen = false;

                    break;
            }

        }
    }

    public enum ProjectActions
    {
        New,
        Modify
    }
}
