using System.Drawing;
using Bones.Shared.Exceptions;
using Bones.WebUI.Leaflet.Enums;
using Bones.WebUI.Leaflet.Models;
using Bones.WebUI.Leaflet.Models.Events;
using Microsoft.JSInterop;
using Rectangle = Bones.WebUI.Leaflet.Models.Rectangle;

namespace Bones.WebUI.Leaflet.Data;

/// <summary>
///   Handles drawing
/// </summary>
public class DrawHandler : IDisposable
{
    private readonly Map _map;
    private readonly IJSRuntime _jsRuntime;
    private readonly Rectangle _rectangle = new();
    private readonly Circle _circle = new();
    private readonly Polygon _polygon = new();
    private readonly List<MouseEvent> _mouseClickEvents = [];
    private DrawState _drawState;

    /// <summary>
    ///   Handles when the drawing is finished
    /// </summary>
    public event EventHandler? DrawFinished;

    /// <summary>
    ///   Creates the handler for drawing
    /// </summary>
    /// <param name="map"></param>
    /// <param name="jsRuntime"></param>
    public DrawHandler(Map map, IJSRuntime jsRuntime)
    {
        _map = map;
        _jsRuntime = jsRuntime;
        _rectangle.StrokeColor = Color.Teal;
        _rectangle.StrokeWidth = 1;
        _rectangle.Fill = true;
        _rectangle.FillColor = Color.Orange;

        _circle.StrokeColor = Color.DarkSlateBlue;
        _circle.StrokeWidth = 1;
        _circle.Fill = true;
        _circle.FillColor = Color.Navy;

        _polygon.StrokeColor = Color.Black;
        _polygon.StrokeWidth = 1;
        _polygon.Fill = true;
        _polygon.FillColor = Color.Red;
    }

    /// <summary>
    ///   Handler for when the circle toggle is pressed
    /// </summary>
    /// <param name="isToggled"></param>
    public void OnDrawCircleToggle(bool isToggled)
    {
        _map.RemoveLayer(_circle);
        _drawState = DrawState.DrawingCircle;
        OnDrawToggle(isToggled);
    }

    /// <summary>
    ///   Handler for when the rectangle toggle is pressed
    /// </summary>
    /// <param name="isToggled"></param>
    public void OnDrawRectangleToggle(bool isToggled)
    {
        _map.RemoveLayer(_rectangle);
        _drawState = DrawState.DrawingRectangle;
        OnDrawToggle(isToggled);
    }

    /// <summary>
    ///   Handler for when the freeform polygon toggle is pressed
    /// </summary>
    /// <param name="isToggled"></param>
    public void OnDrawPolygonToggle(bool isToggled)
    {
        _map.RemoveLayer(_polygon);
        _polygon.Shape = null;
        _drawState = DrawState.DrawingPolygon;
        OnDrawToggle(isToggled);
    }

    private void OnDrawToggle(bool isToggled)
    {
        _mouseClickEvents.Clear();
        if (isToggled)
        {
            _map.OnClick += OnMapClickAsync;
            _map.OnMouseMove += OnMouseMoveAsync;
        }
        else
        {
            UnsubscribeFromMapEvents();
        }
    }

    private async Task OnMapClickAsync(object sender, MouseEvent e)
    {
        if (_drawState != DrawState.DrawingPolygon)
        {
            await AddClickAndUpdateShapeAsync(e);
            if (_mouseClickEvents.Count == 2)
            {
                // untoggle button
                DrawComplete();
            }
        }
        else
        {
            if (_mouseClickEvents.Count > 0)
            {
                PointF[][]? shape = _polygon.Shape;
                PointF? firstPoint = _mouseClickEvents[0].ContainerPoint;
                PointF? thisPoint = e.ContainerPoint;

                // finish a line
                if (shape is { Length: > 0 }
                    && shape[0] is { Length: > 2 }
                    && firstPoint.HasValue
                    && thisPoint.HasValue
                    && Math.Abs(firstPoint.Value.X - thisPoint.Value.X) < 10
                    && Math.Abs(firstPoint.Value.Y - thisPoint.Value.Y) < 10)
                {
                    // update the polygon without the last point (mouse move point)
                    // and we're finished
                    await UpdatePolygonAsync(null);
                    DrawComplete();
                }
                else
                {
                    await AddClickAndUpdateShapeAsync(e);
                }
            }
            else
            {
                await AddClickAndUpdateShapeAsync(e);
            }
        }
    }

    private async Task OnMouseMoveAsync(object sender, MouseEvent e)
    {
        if (_mouseClickEvents.Count != 0
            && e.LatLon != null)
        {
            await UpdateShapeAsync(e.LatLon);
        }
    }

    private async Task AddClickAndUpdateShapeAsync(MouseEvent e)
    {
        if (e.LatLon != null)
        {
            _mouseClickEvents.Add(e);
            await UpdateShapeAsync(e.LatLon);
        }
    }

    private async Task UpdateShapeAsync(LatLon latLon)
    {
        switch (_drawState)
        {
            case DrawState.DrawingRectangle:
                await UpdateRectangleAsync(latLon);
                break;
            case DrawState.DrawingCircle:
                await UpdateCircleAsync(latLon);
                break;
            case DrawState.DrawingPolygon:
                await UpdatePolygonAsync(latLon);
                break;
            default:
                throw new BonesException($"Invalid drawstate: {_drawState}");
        }
    }

    private async Task UpdateRectangleAsync(LatLon latLon)
    {
        LatLon? latLonFromEvent = _mouseClickEvents[0].LatLon;
        if (latLonFromEvent != null)
        {
            _rectangle.Shape = new(
                latLonFromEvent.Lon,
                latLonFromEvent.Lat,
                latLon.Lon - latLonFromEvent.Lon,
                latLon.Lat - latLonFromEvent.Lat
            );
            await AddOrUpdateShapeAsync(_rectangle);
        }
    }

    private async Task UpdateCircleAsync(LatLon latLon)
    {
        LatLon? latLonFromEvent = _mouseClickEvents[0].LatLon;
        if (latLonFromEvent != null)
        {
            _circle.Position = latLonFromEvent;
            // get a rough approximate for now: have to convert to meters - there should be better more precise algorithms out there
            _circle.Radius = Math.Max(Math.Abs(latLon.Lon - latLonFromEvent.Lon),
                Math.Abs(latLon.Lat - latLonFromEvent.Lat)) * 111320;
            await AddOrUpdateShapeAsync(_circle);
        }
    }

    private async Task UpdatePolygonAsync(LatLon? latLon)
    {
        // copy over previous points, add a new one if LatLon defined
        int size = _mouseClickEvents.Count;
        PointF[][] shape = new PointF[1][];
        shape[0] = new PointF[latLon == null ? size : size + 1];

        for (int i = 0; i < size; i++)
        {
            LatLon? latLonFromEvent = _mouseClickEvents[i].LatLon;
            if (latLonFromEvent != null)
            {
                shape[0][i] = latLonFromEvent.ToPointF();
            }
        }
        if (latLon != null)
        {
            shape[0][size] = latLon.ToPointF();
        }
        _polygon.Shape = shape;
        await AddOrUpdateShapeAsync(_polygon);
    }

    private async Task AddOrUpdateShapeAsync(Layer shape)
    {
        if (_map.GetLayers().Contains(shape))
        {
            await LeafletInterops.UpdateShapeAsync(_jsRuntime, _map.Id, shape);
        }
        else
        {
            _map.AddLayer(shape);
        }
    }

    private void DrawComplete()
    {
        UnsubscribeFromMapEvents();
        _drawState = DrawState.None;
        DrawFinished?.Invoke(this, EventArgs.Empty);
    }

    private void UnsubscribeFromMapEvents()
    {
        _map.OnClick -= OnMapClickAsync;
        _map.OnMouseMove -= OnMouseMoveAsync;
    }

    /// <summary>
    ///   Dispose it!
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Disposes of the object if we are disposing
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            UnsubscribeFromMapEvents();
        }
    }

    /// <summary>
    ///   Finalizer for the DrawHandler
    /// </summary>
    ~DrawHandler()
    {
        Dispose(false);
    }
}