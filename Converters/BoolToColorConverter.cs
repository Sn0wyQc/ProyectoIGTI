using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace SkillSwap.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool noLeida = (bool)value;

            var theme = Application.Current.RequestedTheme;

            if (theme == AppTheme.Dark)
            {
                return noLeida ? Color.FromArgb("#1E1E1E") : Color.FromArgb("#2A2A2A");
            }
            else
            {
                return noLeida ? Colors.White : Color.FromArgb("#F3F4F6");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}