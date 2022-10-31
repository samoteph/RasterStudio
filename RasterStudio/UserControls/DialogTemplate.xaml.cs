using RasterStudio.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class DialogTemplate : UserControl
    {
        public DialogTemplate()
        {
            this.InitializeComponent();
            this.DialogContainer.Visibility = Visibility.Collapsed;
        }

        public DialogActions DialogAction
        {
            get { return (DialogActions)GetValue(DialogActionProperty); }
            set { SetValue(DialogActionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProjectAction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogActionProperty =
            DependencyProperty.Register("DialogAction", typeof(DialogActions), typeof(DialogTemplate), new PropertyMetadata(DialogActions.New, OnDialogActionChange));

        private static void OnDialogActionChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as DialogTemplate;

            switch((DialogActions)e.NewValue)
            {
                case DialogActions.New:
                    c.TextBlockTitle.Text = "New template";
                    c.ButtonSave.Visibility = Visibility.Collapsed;
                    c.ButtonOK.Content = "Create";
                    break;
                
                case DialogActions.Modify:
                    c.TextBlockTitle.Text = "Modify template";
                    c.ButtonSave.Visibility = Visibility.Visible;
                    c.ButtonOK.Content = "Save As";
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
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DialogTemplate), new PropertyMetadata(false, OnIsOpenChange));

        private static void OnIsOpenChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as DialogTemplate;

            bool isOpen = ((bool)e.NewValue);

            c.DialogContainer.Visibility = isOpen ? Visibility.Visible : Visibility.Collapsed;

            MainPage.Instance.OpenTemplateDialog(isOpen);

        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
            this.Result = DialogResult.ButtonCancel;
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(this.TextBoxName.Text) == true)
            {
                return;
            }

            this.IsOpen = false;

            this.Result = DialogResult.ButtonOK;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.TextBoxName.Text) == true)
            {
                return;
            }
            
            this.IsOpen = false;
            this.Result = DialogResult.ButtonSave;
        }

        public Task ShowDialogAsync()
        {
            this.IsOpen = true;

            TaskCompletionSource<object> source = new TaskCompletionSource<object>();

            RoutedEventHandler onButtonWaitClick = null;

            onButtonWaitClick = (s, e) =>
            {
                if (this.IsOpen == false)
                {
                    this.ButtonOK.Click -= onButtonWaitClick;
                    this.ButtonSave.Click -= onButtonWaitClick;
                    this.ButtonCancel.Click -= onButtonWaitClick;

                    source.TrySetResult(null);
                }
            };

            this.ButtonSave.Click += onButtonWaitClick;
            this.ButtonOK.Click += onButtonWaitClick;
            this.ButtonCancel.Click += onButtonWaitClick;

            return source.Task;
        }

        public string TemplateExporterName
        {
            get
            {
                return this.TextBoxName.Text;
            }

            set
            {
                this.TextBoxName.Text = value;
                this.TextBoxName.Select(value.Length, 0);
            }
        }

        public DialogResult Result 
        { 
            get; 
            private set; 
        }
    }

    public enum DialogResult
    {
        ButtonOK,
        ButtonSave,
        ButtonCancel
    }
}
