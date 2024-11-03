using Microsoft.AspNetCore.Components;

namespace Bones.WebUI.Leaflet;

/// <summary>
///   The component to use if you want to add a map to a page somewhere
/// </summary>
public partial class LeafletMap : ComponentBase
{
    /// <summary>
    ///   The map this component is tracking
    /// </summary>
    [Parameter]
    public required Map Map { get; set; }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LeafletInterops.CreateAsync(JsRuntime, Map);
            Map.RaiseOnInitialized();
        }
    }
}