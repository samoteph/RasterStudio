﻿#pragma checksum "C:\Dev\Uwp\Labo\RasterStudio\RasterStudio\UserControls\ColorControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4AA8EFC00A35BF61ED32032BD8E831B5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RasterStudio.UserControls
{
    partial class ColorControl : 
        global::Windows.UI.Xaml.Controls.UserControl, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // UserControls\ColorControl.xaml line 12
                {
                    this.LayoutRoot = (global::Windows.UI.Xaml.Controls.Grid)(target);
                    ((global::Windows.UI.Xaml.Controls.Grid)this.LayoutRoot).Tapped += this.ColorTapped;
                }
                break;
            case 3: // UserControls\ColorControl.xaml line 28
                {
                    this.TextBlockRGB = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 4: // UserControls\ColorControl.xaml line 45
                {
                    this.ColorPicker = (global::RasterStudio.UserControls.ColorPicker)(target);
                }
                break;
            case 5: // UserControls\ColorControl.xaml line 49
                {
                    this.GridColor = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 6: // UserControls\ColorControl.xaml line 50
                {
                    this.TextBlockText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

