using RasterStudio.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
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

            this.Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            string json = null;

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Templates/DefaultTemplate.json"));

            using (var inputStream = await file.OpenReadAsync())
            using (var classicStream = inputStream.AsStreamForRead())
            using (var streamReader = new StreamReader(classicStream))
            {
                json = streamReader.ReadToEnd();
            }

            var templateExporters = TemplateExporterSerializer.DeserializeCollection(json);

            this.ComboBoxTemplates.SelectionChanged += OnTemplateChanged;
            this.ComboBoxTemplates.ItemsSource = templateExporters;
        }

        /// <summary>
        /// Changement dans la ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void OnTemplateChanged(object sender, SelectionChangedEventArgs e)
        {
            var templateExporter = this.ComboBoxTemplates.SelectedItem as TemplateExporter;
            
            templateExporter.CopyTo(project);
            this.ApplyExporter();
        }

        /// <summary>
        /// Apply Exporter
        /// </summary>

        private void ApplyExporter()
        {
            var exporter = this.project.Exporter;

            this.TagTextBoxHeader.TextCommand = exporter.PaletteHeaderTagManager.TextCommand;
            this.TagTextBoxFooter.TextCommand = exporter.PaletteFooterTagManager.TextCommand;

            this.ExportRasterControl.HeaderTextCommand = exporter.RasterLineHeaderTagManager.TextCommand;
            this.ExportRasterControl.FooterTextCommand = exporter.RasterLineFooterTagManager.TextCommand;
            this.ExportRasterControl.ColorTextCommand = exporter.RasterColorTagManager.TextCommand;

            this.ExportRasterControl.LineSelector = exporter.LineSelector;
            this.ExportRasterControl.OrientationSelector = exporter.OrientationSelector;
            this.ExportRasterControl.Separator = exporter.Separator;
            this.ExportRasterControl.ColorSelector = exporter.ColorSelector;
        }

        private void ExportControl_GotFocus(object sender, RoutedEventArgs e)
        {
            this.UpdatePreview();
        }

        bool isUpdatingPreview;
        bool needRefreshPreview;

        private void ButtonSaveTemplate_Click(object sender, RoutedEventArgs e)
        {

            TemplateExporter templateExporter = new TemplateExporter(project.Exporter);

            templateExporter.Name = "ASM DevPac Data";
            templateExporter.IsEditable = false;

            string json = TemplateExporterSerializer.SerializeCollection(new List<TemplateExporter>() { templateExporter });
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

        private void ExportRastersControl_SelectorChanged(object sender, EventArgs e)
        {
            this.UpdatePreview();
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
                this.TextBlockPreview.Text = text ?? String.Empty;
            });

            // il y a eu modification depuis la dernière fois
            if (this.needRefreshPreview == true)
            {
                this.needRefreshPreview = false;
                this.ExecuteUpdatePreview();
            }

            this.isUpdatingPreview = false;
        }

        /// <summary>
        /// Exportation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            await MainPage.Instance.Project.ExportProjectAsync(".csv");
        }
    }
}
