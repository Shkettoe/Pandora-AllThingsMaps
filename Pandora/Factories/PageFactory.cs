using System;
using Pandora.Data.Enums;
using Pandora.ViewModels;

namespace Pandora.Factories;

public class PageFactory(Func<PageNamesEnum, ViewModelBase> factory)
{
    public ViewModelBase Create(PageNamesEnum name) => factory.Invoke(name);
}