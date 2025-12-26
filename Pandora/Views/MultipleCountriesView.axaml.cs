using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Pandora.ViewModels;

namespace Pandora.Views;

public partial class MultipleCountriesView : UserControl
{
    private bool _isDragging;
    private Point _lastPosition;

    public MultipleCountriesView()
    {
        InitializeComponent();

        MapCanvas.PointerPressed += OnPointerPressed;
        MapCanvas.PointerMoved += OnPointerMoved;
        MapCanvas.PointerReleased += OnPointerReleased;
        MapCanvas.PointerWheelChanged += (_, e) =>
        {
            if (DataContext is not MultipleCountriesViewModel viewModel) return;
            var delta = e.Delta.Y;
            var pointerPosition = e.GetPosition(MapCanvas);
            var zoomFactor = delta > 0 ? 1.1 : 0.9;
            viewModel.ChangeScale(zoomFactor, pointerPosition);
        };
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isDragging = true;
        _lastPosition = e.GetPosition(MapCanvas);
        e.Pointer.Capture(MapCanvas);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (DataContext is not MultipleCountriesViewModel viewModel) return;
        var currentPosition = e.GetPosition(MapCanvas);
        viewModel.CursorLocation = currentPosition;
        
        if (!_isDragging ) return;

        var deltaX = currentPosition.X - _lastPosition.X;
        var deltaY = currentPosition.Y - _lastPosition.Y;
        viewModel.PanCanvas(deltaX, -deltaY);

        _lastPosition = currentPosition;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        e.Pointer.Capture(MapCanvas);
    }
    
    private void OnPolygonClicked(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Polygon polygon) return;
        if (polygon.DataContext is not CountryViewModel countryViewModel) return;
        if (DataContext is not MultipleCountriesViewModel viewModel) return;
        viewModel.Countries.ToList().ForEach(c =>
        {
            if (c.Country.Name == countryViewModel.Country.Name) c.ClickEventHandlerCommand.Execute(null);
        });
        viewModel.ClickedCountryName = countryViewModel.Country.Name;
        e.Handled = true;
    }
}