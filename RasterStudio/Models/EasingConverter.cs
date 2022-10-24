using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace RasterStudio.Models
{
    public class EasingFunctionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (EasingFunction)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (EasingFunction)((int)value);
        }
    }

    public class EasingModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (EasingMode)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (EasingMode)((int)value);
        }
    }
}
