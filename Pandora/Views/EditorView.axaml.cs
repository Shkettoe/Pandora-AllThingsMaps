using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Pandora.ViewModels;

namespace Pandora.Views;

public partial class EditorView : UserControl
{
    public EditorView()
    {
        InitializeComponent();
   }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not EditorViewModel vm) return;
        if(sender is ListBoxItem listBoxItem) vm.LoadSelectedFileCommand.Execute(listBoxItem.Content);
    }
}