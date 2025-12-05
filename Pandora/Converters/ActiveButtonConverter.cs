using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Pandora.ViewModels;

namespace Pandora.Converters;

public class ActiveButtonConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ViewModelBase selectedViewModel && parameter is string buttonName)
        {
            return buttonName.ToLower() switch
            {
                "home" => selectedViewModel is HomeViewModel,
                "about" => selectedViewModel is AboutViewModel,
                "multiplecountries" => selectedViewModel is MultipleCountriesViewModel,
                "settings" => selectedViewModel is SettingsViewModel,
                _ => false
            };
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
