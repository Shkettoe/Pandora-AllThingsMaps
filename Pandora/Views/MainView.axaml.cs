using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data;

namespace Pandora.Views;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
        var dataSource = new ObservableCollection<string[]>
        {
            new []{ "A", "B", "C" },
            new []{ "C", "B", "A" },
        };
        
        foreach (var idx in dataSource[0].Select((_, index) => index))
        {
            AttributeGrid.Columns.Add(new DataGridTextColumn{Header = $"{idx + 1}. column", Binding = new Binding($"[{idx}]")});
        }

        // AttributeGrid.AutoGenerateColumns = false;
        // AttributeGrid.ItemsSource = dataSource;
    }
}
