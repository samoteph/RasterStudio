using RasterStudio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
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
    public sealed partial class ExportControl : UserControl
    {
        private Project project;

        public ExportControl()
        {
            this.InitializeComponent();

            this.project = MainPage.Instance.Project;

            this.TagTextBoxHeader.TagManager = project.Exporter.PaletteHeaderTagManager;
            this.TagTextBoxFooter.TagManager = project.Exporter.PaletteFooterTagManager;

            this.GotFocus += ExportControl_GotFocus;
        }

        private void ExportControl_GotFocus(object sender, RoutedEventArgs e)
        {
            this.UpdatePreview();
        }

        bool isUpdatingPreview;
        bool needRefreshPreview;

        private void ButtonAddTemplate_Click(object sender, RoutedEventArgs e)
        {
        }

        private void TagTextBox_TextChanged(object sender, EventArgs e)
        {
            this.UpdatePreview();
        }

        private void TagTextBoxHeader_GotFocus(object sender, RoutedEventArgs e)
        {
            this.ScrollViewPreview.ChangeView(0, 0, 1, true);
        }

        private void TagTextBoxFooter_GotFocus(object sender, RoutedEventArgs e)
        {
            this.ScrollViewPreview.ChangeView(0, this.ScrollViewPreview.ExtentHeight, 1, true);
        }

        public async void UpdatePreview()
        {
            if(this.isUpdatingPreview == false)
            {
                this.isUpdatingPreview = true;

                await this.UpdatePreviewAsync();
            }
            else
            {
                this.needRefreshPreview = true;
            }
        }

        private Task UpdatePreviewAsync()
        {
            return Task.Run(() =>
            {
                this.ExecuteUpdatePreview();
            });
        }

        private async void ExecuteUpdatePreview()
        {
            string text = MainPage.Instance.Project.Exporter.GetExportText();

            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.TextBlockPreview.Text = text;
            });

            // il y a eu modification depuis la dernière fois
            if (this.needRefreshPreview == true)
            {
                this.needRefreshPreview = false;
                this.ExecuteUpdatePreview();
            }

            this.isUpdatingPreview = false;
        }
    }
}
