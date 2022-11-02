using RasterStudio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

            this.LayoutRoot.DataContext = this;

            this.ComboBoxLines.SelectionChanged += OnLineSelectionChanged;
            this.ComboBoxColors.SelectionChanged += OnColorSelectionChanged;
            this.ComboBoxOrientation.SelectionChanged += OnOrientationChanged;
            this.TextBoxSeparator.TextChanged += OnSeparatorTextChanged;
        }



        public TextRasterExporter RasterExporter
        {
            get { return (TextRasterExporter)GetValue(RasterExporterProperty); }
            set { SetValue(RasterExporterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RasterExporter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RasterExporterProperty =
            DependencyProperty.Register("RasterExporter", typeof(TextRasterExporter), typeof(ExportRastersControl), new PropertyMetadata(null, OnRasterExporterChanged));

        private static void OnRasterExporterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as ExportRastersControl;
            var textRasterExporter = e.NewValue as TextRasterExporter;

            c.TagTextBoxHeader.TagManager = textRasterExporter.RasterLineHeaderTagManager;
            c.TagTextBoxFooter.TagManager = textRasterExporter.RasterLineFooterTagManager;
            c.TagTextBoxRasters.TagManager = textRasterExporter.RasterColorTagManager;

            // initialisation
            c.Separator = textRasterExporter.Separator;
            
            c.LineSelector = textRasterExporter.LineSelector;
            c.ColorSelector = textRasterExporter.ColorSelector;
            c.OrientationSelector = textRasterExporter.OrientationSelector;
        }

        private void OnSeparatorTextChanged(object sender, TextChangedEventArgs e)
        {
            this.RasterExporter.Separator = this.TextBoxSeparator.Text;
            SelectorChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnLineSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (this.ComboBoxLines.SelectedItem as ComboBoxItem);

            this.RasterExporter.LineSelector = (string)item.Tag == "All" ? LineSelector.All : LineSelector.Changing;
            
            SelectorChanged?.Invoke(this,EventArgs.Empty);
        }

        private void OnColorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (this.ComboBoxColors.SelectedItem as ComboBoxItem);

            switch((string)item.Tag)
            {
                case "All":
                    this.RasterExporter.ColorSelector = ColorSelector.All;
                    break;
                case "Used":
                    this.RasterExporter.ColorSelector = ColorSelector.Used;
                    break;
                case "Changing":
                    this.RasterExporter.ColorSelector = ColorSelector.Changing;
                    break;
            }

            SelectorChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnOrientationChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (this.ComboBoxOrientation.SelectedItem as ComboBoxItem);

            this.RasterExporter.OrientationSelector = ((string)item.Tag) == "Horizontal" ? OrientationSelector.Horizontal : OrientationSelector.Vertical;
            SelectorChanged?.Invoke(this, EventArgs.Empty);
        }

        private void TagTextBox_TextChanged(object sender, EventArgs e)
        {
            this.TextChanged?.Invoke(this,EventArgs.Empty);
        }

        public event EventHandler TextChanged;
        public event EventHandler SelectorChanged;



        public string HeaderTextCommand
        {
            get { return (string)GetValue(HeaderTextCommandProperty); }
            set { SetValue(HeaderTextCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTextCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTextCommandProperty =
            DependencyProperty.Register("HeaderTextCommand", typeof(string), typeof(ExportRastersControl), new PropertyMetadata(string.Empty));

        public string FooterTextCommand
        {
            get { return (string)GetValue(FooterTextCommandProperty); }
            set { SetValue(FooterTextCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FooterTextCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FooterTextCommandProperty =
            DependencyProperty.Register("FooterTextCommand", typeof(string), typeof(ExportRastersControl), new PropertyMetadata(string.Empty));



        public string ColorTextCommand
        {
            get { return (string)GetValue(ColorTextCommandProperty); }
            set { SetValue(ColorTextCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorTextCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorTextCommandProperty =
            DependencyProperty.Register("ColorTextCommand", typeof(string), typeof(ExportRastersControl), new PropertyMetadata(string.Empty));


        public LineSelector LineSelector
        {
            get { return (LineSelector)GetValue(LineSelectorProperty); }
            set { SetValue(LineSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineSelectorProperty =
            DependencyProperty.Register("LineSelector", typeof(LineSelector), typeof(ExportRastersControl), new PropertyMetadata(LineSelector.All, OnLineSelectorChanged));

        private static void OnLineSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as ExportRastersControl;
            var tag = e.NewValue.ToString();

            foreach( ComboBoxItem item in me.ComboBoxLines.Items)
            {
                if((string)item.Tag == tag)
                {
                    me.ComboBoxLines.SelectedItem = item;
                    break;
                }
            }
        }

        public OrientationSelector OrientationSelector
        {
            get { return (OrientationSelector)GetValue(OrientationSelectorProperty); }
            set { SetValue(OrientationSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OrientationSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationSelectorProperty =
            DependencyProperty.Register("OrientationSelector", typeof(OrientationSelector), typeof(ExportRastersControl), new PropertyMetadata(OrientationSelector.Vertical, OnOrientationSelectorChanged));

        private static void OnOrientationSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as ExportRastersControl;
            var tag = e.NewValue.ToString();

            foreach (ComboBoxItem item in me.ComboBoxOrientation.Items)
            {
                if ((string)item.Tag == tag)
                {
                    me.ComboBoxOrientation.SelectedItem = item;
                    break;
                }
            }
        }

        /// <summary>
        /// Color Selector
        /// </summary>

        public ColorSelector ColorSelector
        {
            get { return (ColorSelector)GetValue(ColorSelectorProperty); }
            set { SetValue(ColorSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorSelectorProperty =
            DependencyProperty.Register("ColorSelector", typeof(ColorSelector), typeof(ExportRastersControl), new PropertyMetadata(ColorSelector.All, OnColorSelectorChanged));

        private static void OnColorSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as ExportRastersControl;
            var tag = e.NewValue.ToString();

            foreach (ComboBoxItem item in me.ComboBoxColors.Items)
            {
                if ((string)item.Tag == tag)
                {
                    me.ComboBoxColors.SelectedItem = item;
                    break;
                }
            }
        }

        // Separator

        public string Separator
        {
            get { return (string)GetValue(SeparatorProperty); }
            set { SetValue(SeparatorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Separator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeparatorProperty =
            DependencyProperty.Register("Separator", typeof(string), typeof(ExportRastersControl), new PropertyMetadata(null));


    }
}
