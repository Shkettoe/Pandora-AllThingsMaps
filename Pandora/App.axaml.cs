using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Diagnostics;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Pandora.Data.Enums;
using Pandora.Factories;
using Pandora.ViewModels;
using Pandora.Views;

namespace Pandora;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        this.AttachDevTools(new DevToolsOptions());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton<MainViewModel>();
        collection.AddTransient<AboutViewModel>();
        collection.AddTransient<HomeViewModel>();
        collection.AddTransient<MultipleCountriesViewModel>();
        collection.AddTransient<SettingsViewModel>();
        collection.AddSingleton<Func<PageNamesEnum, ViewModelBase>>(x => name => 
        name switch
        {
            PageNamesEnum.Home => x.GetRequiredService<HomeViewModel>(),
            PageNamesEnum.About => x.GetRequiredService<AboutViewModel>(),
            PageNamesEnum.MultipleCountries => x.GetRequiredService<MultipleCountriesViewModel>(),
            PageNamesEnum.Default => x.GetRequiredService<HomeViewModel>(),
            PageNamesEnum.Settings => x.GetRequiredService<SettingsViewModel>(),
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
        });
        collection.AddSingleton<PageFactory>();
        collection.AddSingleton<SidebarViewModel>();
        
        var provider = collection.BuildServiceProvider();
        
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainView
                {
                    DataContext = provider.GetRequiredService<MainViewModel>()
                };
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = provider.GetRequiredService<MainViewModel>()
                };
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
