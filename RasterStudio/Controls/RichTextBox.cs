using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RasterStudio.Controls
{
    public class RichTextBox : RichEditBox
    {
        public RichTextBox() : base()
        {
            // plus de menu contextuelle avec Bold et italic
            this.ContextFlyout = null;
        }

        // En virant la touche CTRL on empeche les raccourcis clavier
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);

            if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
            {
                switch (e.Key)
                {
                    case VirtualKey.X:
                    case VirtualKey.C:
                    case VirtualKey.V:
                    case VirtualKey.Y:
                    case VirtualKey.Z:
                        // on continue
                        break;
                    default:
                        return;
                }
            }

            base.OnKeyDown(e);
        }
    }
}
