﻿#pragma checksum "C:\Dev\Uwp\Labo\RasterStudio\RasterStudio\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "904D37314806C4BB356DE1BACE523E19"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RasterStudio
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
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
            case 2: // MainPage.xaml line 60
                {
                    this.PaletteControl = (global::RasterStudio.UserControls.PaletteControl)(target);
                }
                break;
            case 3: // MainPage.xaml line 62
                {
                    this.RasterControl = (global::RasterStudio.UserControls.RasterControl)(target);
                }
                break;
            case 4: // MainPage.xaml line 64
                {
                    this.RasterSlider = (global::RasterStudio.UserControls.RasterSlider)(target);
                }
                break;
            case 5: // MainPage.xaml line 68
                {
                    this.SlateView = (global::SamuelBlanchard.Xaml.Controls.SlateView)(target);
                }
                break;
            case 6: // MainPage.xaml line 35
                {
                    global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem element6 = (global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)element6).Click += this.MenuItemAllRasters_Click;
                }
                break;
            case 7: // MainPage.xaml line 36
                {
                    global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem element7 = (global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)element7).Click += this.MenuItemBlankScreen_Click;
                }
                break;
            case 8: // MainPage.xaml line 32
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element8 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element8).Click += this.MenuItemOpen_Click;
                }
                break;
            case 9: // MainPage.xaml line 25
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element9 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element9).Click += this.MenuItemOpen_Click;
                }
                break;
            case 10: // MainPage.xaml line 26
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element10 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element10).Click += this.MenuItemSave_Click;
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
