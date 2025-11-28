using System;
using Avalonia;
using Avalonia.Controls;
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
        MapCanvas.PointerWheelChanged += (sender, e) =>
        {
            if (DataContext is not MultipleCountriesViewModel viewModel) return;

            var delta = e.Delta.Y;
            var pointerPosition = e.GetPosition(MapCanvas);

            var oldScale = viewModel.Scale;
            const double minScale = 0.1;
            var zoomFactor = delta > 0 ? 1.1 : 0.9;
            var newScale = Math.Max(minScale, oldScale * zoomFactor); // Prevent negative scale

            var scaleRatio = newScale / oldScale;

            viewModel.OffsetX = pointerPosition.X - (pointerPosition.X - viewModel.OffsetX) * scaleRatio;
            
            // Screen measures height from top down, while canvas measures it from bottom up
            var invertedPointerY = viewModel.CanvasHeight - pointerPosition.Y;
            viewModel.OffsetY = invertedPointerY - (invertedPointerY - viewModel.OffsetY) * scaleRatio;

            viewModel.Scale = newScale;
            viewModel.ResetPositions();
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
        if (!_isDragging) return;

        var currentPosition = e.GetPosition(MapCanvas);
        var deltaX = currentPosition.X - _lastPosition.X;
        var deltaY = currentPosition.Y - _lastPosition.Y;

        if (DataContext is MultipleCountriesViewModel viewModel)
        {
            viewModel.PanCanvas(deltaX, -deltaY);
        }

        _lastPosition = currentPosition;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        e.Pointer.Capture(MapCanvas);
    }
}