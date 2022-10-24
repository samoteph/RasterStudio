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
        public ExportControl()
        {
            this.InitializeComponent();

            var tagManagerHeaderFooter = new TagManager();

            tagManagerHeaderFooter.AddTag(new Tag("Filename", () => MainPage.Instance.Project.Filename));
            tagManagerHeaderFooter.AddTag(new Tag("Year", () => DateTime.Now.Year.ToString()));
            tagManagerHeaderFooter.AddTag(new Tag("Month", () => DateTime.Now.Month.ToString()));
            tagManagerHeaderFooter.AddTag(new Tag("Day", () => DateTime.Now.Day.ToString()));

            this.TagTextBoxHeader.TagManager = tagManagerHeaderFooter;
            this.TagTextBoxFooter.TagManager = tagManagerHeaderFooter;

            var tagManagerRasters = new TagRasterManager();

            tagManagerRasters.AddTag(new TagRaster("Color Address",(raster) => raster.ColorAddress));
            tagManagerRasters.AddTag(new TagRaster("Color Index", (raster) => raster.ColorIndex.ToString()));
            tagManagerRasters.AddTag(new TagRaster("Color Hexa Value", (raster, line) => raster.Colors[line].Color.ToString("x4")));

            this.TagTextBoxRasters.TagManager = tagManagerRasters;

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
            StringBuilder builder = new StringBuilder();

            string text = this.TagTextBoxHeader.GetText();

            if (string.IsNullOrWhiteSpace(text) == false)
            {
                builder.AppendLine(text);
            }

            var rasters = MainPage.Instance.Project.Rasters;

            TagRasterManager tagRasterManager = (TagRasterManager)this.TagTextBoxRasters.TagManager;

            int lineCount = rasters[0].Colors.Length;

            List<AtariRaster> usedRasters = new List<AtariRaster>(16);

            foreach (var raster in rasters)
            {
                if( MainPage.Instance.IsRasterColorModified(raster.ColorIndex) == true)
                {
                    usedRasters.Add(raster);
                }
            }
            
            for (int line = 0; line < lineCount; line++)
            {
                tagRasterManager.Line = line;

                foreach (var raster in usedRasters)
                {
                    tagRasterManager.Raster = raster;

                    text = this.TagTextBoxRasters.GetText();

                    if (string.IsNullOrWhiteSpace(text) == false)
                    {
                        builder.AppendLine(text);
                    }
                }
            }

            text = this.TagTextBoxFooter.GetText();

            if (string.IsNullOrWhiteSpace(text) == false)
            {
                builder.AppendLine(text);
            }

            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.TextBlockPreview.Text = builder.ToString();
            });

            // il y a eu modifciation depuis la dernière fois
            if (this.needRefreshPreview == true)
            {
                this.needRefreshPreview = false;
                this.ExecuteUpdatePreview();
                }

            this.isUpdatingPreview = false;
        }
    }
}
