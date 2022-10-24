using RasterStudio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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

            tagManagerHeaderFooter.AddTag(new Tag("Year", () => DateTime.Now.Year.ToString()));
            tagManagerHeaderFooter.AddTag(new Tag("Month", () => DateTime.Now.Month.ToString()));
            tagManagerHeaderFooter.AddTag(new Tag("Day", () => DateTime.Now.Day.ToString()));
            tagManagerHeaderFooter.AddTag(new Tag("Filename",()=>MainPage.Instance.Project.Filename));

            this.TagTextBoxHeader.TagManager = tagManagerHeaderFooter;
            this.TagTextBoxFooter.TagManager = tagManagerHeaderFooter;

            var tagManagerRasters = new TagManager();

            tagManagerRasters.AddTag(new Tag("Palette Address", () => DateTime.Now.Year.ToString()));
            tagManagerRasters.AddTag(new Tag("Color Value", () => DateTime.Now.Month.ToString()));

            this.TagTextBoxRasters.TagManager = tagManagerRasters;
        }

        private void ButtonAddTemplate_Click(object sender, RoutedEventArgs e)
        {
        }

        private void TagTextBoxHeader_TextChanged(object sender, EventArgs e)
        {
            this.UpdatePreview();
        }

        private void TagTextBoxFooter_TextChanged(object sender, EventArgs e)
        {
            this.UpdatePreview();
        }

        private void TagTextBoxRasters_TextChanged(object sender, EventArgs e)
        {
            this.UpdatePreview();
        }

        public void UpdatePreview()
        {
            StringBuilder builder = new StringBuilder();

            if (string.IsNullOrWhiteSpace(this.TagTextBoxHeader.Text) == false)
            {
                builder.AppendLine(this.TagTextBoxHeader.Text);
            }

            var rasters = MainPage.Instance.Project.Rasters;

            foreach (var raster in rasters)
            {
                builder.AppendLine(this.TagTextBoxRasters.Text);
            }

            if (string.IsNullOrWhiteSpace(this.TagTextBoxFooter.Text) == false)
            {
                builder.AppendLine(this.TagTextBoxFooter.Text);
            }

            this.TextBlockPreview.Text = builder.ToString();

        }
    }
}
