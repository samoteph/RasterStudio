using RasterStudio.Models;
using RasterStudio.Models.Templates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Popups;
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
        private TemplateExporter currentTemplateExporter;
        private TemplateExporterFileManager templateExporterfileManager = new TemplateExporterFileManager();
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
            await templateExporterfileManager.LoadCompleteTemplateExportersAsync();

            this.ComboBoxTemplates.SelectionChanged += OnTemplateChanged;
            this.ComboBoxTemplates.ItemsSource = templateExporterfileManager.CompleteTemplateExporters;
        }

        /// <summary>
        /// Changement dans la ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void OnTemplateChanged(object sender, SelectionChangedEventArgs e)
        {
            var templateExporter = this.ComboBoxTemplates.SelectedItem as TemplateExporter;

            if (templateExporter != null)
            {
                this.currentTemplateExporter = templateExporter;

                templateExporter.CopyTo(project);

                this.ApplyTemplateExporter(templateExporter);
            }
        }

        /// <summary>
        /// Apply Exporter
        /// </summary>

        private void ApplyTemplateExporter(TemplateExporter exporter)
        {
            this.TagTextBoxHeader.TextCommand = exporter.PaletteHeader.TextCommand;
            this.TagTextBoxFooter.TextCommand = exporter.PaletteFooter.TextCommand;

            this.ItemsControlRastersControl.ItemsSource = project.Exporter.TextRasterExporters;

            /*
            this.ExportRasterControl.HeaderTextCommand = exporter.PaletteRaster.HeaderTextCommand;
            this.ExportRasterControl.FooterTextCommand = exporter.PaletteRaster.FooterTextCommand;
            this.ExportRasterControl.ColorTextCommand = exporter.PaletteRaster.ColorTextCommand;

            this.ExportRasterControl.LineSelector = exporter.LineSelector;
            this.ExportRasterControl.OrientationSelector = exporter.OrientationSelector;
            this.ExportRasterControl.Separator = exporter.Separator;
            this.ExportRasterControl.ColorSelector = exporter.ColorSelector;
            */

            this.TextBoxExtension.Text = exporter.Extension ?? String.Empty;
        }

        private void ExportControl_GotFocus(object sender, RoutedEventArgs e)
        {
            this.UpdatePreview();
        }

        bool isUpdatingPreview;
        bool needRefreshPreview;

        private async void ButtonDeleteTemplate_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog($"Do you want to remove the template \"{this.currentTemplateExporter.Name}\"?", "Remove template");

            UICommand buttonOK = new UICommand("OK");
            dialog.Commands.Add(buttonOK);
            UICommand buttonCancel = new UICommand("Cancel");
            dialog.Commands.Add(buttonCancel);

            var buttonResult = await dialog.ShowAsync();
            
            if(buttonResult == buttonOK)
            {
                await this.templateExporterfileManager.DeleteWriteableTemplateExporterAsync(this.currentTemplateExporter);
                this.ComboBoxTemplates.SelectedIndex = 0;
            }        
        }

        private async void ButtonSaveTemplate_Click(object sender, RoutedEventArgs e)
        {
            // Template par defaut dont on veut faire une copie
            if (this.currentTemplateExporter.IsEditable == false)
            {
                var dialog = MainPage.Instance.DialogTemplate;

                dialog.TemplateExporterName = this.currentTemplateExporter.Name;
                dialog.DialogAction = DialogActions.New;

                await dialog.ShowDialogAsync();

                if (dialog.Result != DialogResult.ButtonCancel)
                {
                    await this.CreateTemplateExporterFromCurrentTemplateAsync(dialog.TemplateExporterName);
                }
            }
            else
            {
                var dialog = MainPage.Instance.DialogTemplate;

                dialog.TemplateExporterName = this.currentTemplateExporter.Name;
                dialog.DialogAction = DialogActions.Modify;

                await dialog.ShowDialogAsync();

                if (dialog.Result == DialogResult.ButtonSave)
                {
                    this.currentTemplateExporter.Name = dialog.TemplateExporterName;
                    this.currentTemplateExporter.Extension = this.TextBoxExtension.Text;

                    this.currentTemplateExporter.CopyFrom(project.Exporter);

                    // ici on doit cloner le TextExporter (constructeur de TemplateExporter)

                    await this.templateExporterfileManager.SaveWriteableTemplateExporterAsync(this.currentTemplateExporter);

                    this.ComboBoxTemplates.SelectedItem = this.currentTemplateExporter;
                }
                // Button OK (Save As)
                else
                {
                    await this.CreateTemplateExporterFromCurrentTemplateAsync(dialog.TemplateExporterName);
                }
            }
        }

        private async Task CreateTemplateExporterFromCurrentTemplateAsync(string templateExporterName)
        {
            var newTemplateExporter = new TemplateExporter(this.project.Exporter);

            newTemplateExporter.Name = templateExporterName;
            newTemplateExporter.Extension = this.TextBoxExtension.Text;
            newTemplateExporter.IsEditable = true;

            await this.templateExporterfileManager.SaveWriteableTemplateExporterAsync(newTemplateExporter);

            this.currentTemplateExporter = newTemplateExporter;
            this.ComboBoxTemplates.SelectedItem = newTemplateExporter;
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
                try
                {
                    this.ExecuteUpdatePreview();
                }
                catch
                {
                    // peut arriver en cas de suppression;
                }
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
            await MainPage.Instance.Project.ExportProjectAsync(this.TextBoxExtension.Text);
        }

        private void TextBoxExtension_LostFocus(object sender, RoutedEventArgs e)
        {
            string extension = this.TextBoxExtension.Text.Trim();
        
            if(string.IsNullOrEmpty(extension) && extension.StartsWith(".") == false)
            {
                extension = "." + extension;
                this.TextBoxExtension.Text = extension;
            }
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(this.TextBlockPreview.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void ButtonLoadTemplate_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
