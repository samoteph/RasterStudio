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
    public sealed partial class TagTextBox : UserControl
    {
        StringBuilder builder = new StringBuilder();

        public TagTextBox()
        {
            this.InitializeComponent();

            this.LayoutRoot.DataContext = this;

            this.TextBox.TextChanged += OnTextChanged;
            
        }

        public event EventHandler TextChanged;

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            if (this.TagManager != null)
            {
                this.TagManager.TextCommand = this.TextBox.Text;
                this.TextChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// TagManager pour gérer les remplacement de text
        /// </summary>

        public TagManager TagManager
        {
            get
            {
                return this.tagManager;
            }

            set
            {
                this.tagManager = value;

                this.TextCommand = value.TextCommand ?? String.Empty; 
                this.ItemsControlTags.ItemsSource = value.GetTags();
            }
        }

        private TagManager tagManager;

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TagTextBox), new PropertyMetadata(null));

        private void GridTag_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var grid = sender as Grid;

            var tag = grid.DataContext as Tag;

            builder.Clear();
            builder.Append(this.TextBox.Text);

            int cursorPosition = this.TextBox.SelectionStart;

            if (cursorPosition < builder.Length)
            {
                builder.Insert(cursorPosition, tag.TextCommand);
            }
            else
            {
                builder.Append(tag.TextCommand);
            }

            this.TextCommand = builder.ToString();

            this.TextBox.Focus(FocusState.Programmatic);

            this.TextBox.SelectionStart = cursorPosition + tag.TextCommand.Length;
        }



        public object TitleContent
        {
            get { return (object)GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleContentProperty =
            DependencyProperty.Register("TitleContent", typeof(object), typeof(TagTextBox), new PropertyMetadata(null));

        public string TextCommand
        {
            get { return (string)GetValue(TextCommandProperty); }
            set { SetValue(TextCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextCommandProperty =
            DependencyProperty.Register("TextCommand", typeof(string), typeof(TagTextBox), new PropertyMetadata(string.Empty, OnTextCommandChanged));

        private static void OnTextCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /*
        public string TextCommand
        {
            get
            {
                return this.TextBox.Text;
            }

            set
            {
                this.TextBox.Text = value;
            }
        }
        */

    }
}
