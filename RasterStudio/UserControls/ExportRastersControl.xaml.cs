using RasterStudio.Models;
using System;
using System.Collections.Generic;
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
    public sealed partial class ExportRastersControl : UserControl
    {
        public ExportRastersControl()
        {
            this.InitializeComponent();

            this.TagTextBoxHeader.TagManager = MainPage.Instance.Project.Exporter.RasterLineHeaderTagManager;
            this.TagTextBoxFooter.TagManager = MainPage.Instance.Project.Exporter.RasterLineFooterTagManager;
            this.TagTextBoxRasters.TagManager = MainPage.Instance.Project.Exporter.RasterColorTagManager;
        }

        private void TagTextBox_TextChanged(object sender, EventArgs e)
        {
            this.TextChanged?.Invoke(this,EventArgs.Empty);
        }

        public event EventHandler TextChanged;
    }
}
