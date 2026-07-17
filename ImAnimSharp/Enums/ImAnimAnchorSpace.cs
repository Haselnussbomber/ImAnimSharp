namespace ImAnimSharp;

/// <summary>
/// Anchor Space (<see href="https://github.com/soufianekhiat/ImAnim/blob/0e28f28/docs/anchors.md">Docs</see>)
/// </summary>
public enum ImAnimAnchorSpace
{
    /// <summary>
    /// Reference size: <c>ImGui::GetContentRegionAvail()</c>
    /// </summary>
    WindowContent,

    /// <summary>
    /// Reference size: <c>ImGui::GetWindowSize()</c>
    /// </summary>
    Window,

    /// <summary>
    /// Reference size: <c>ImGui::GetWindowViewport()->Size</c>
    /// </summary>
    Viewport,

    /// <summary>
    /// Reference size: <c>ImGui::GetItemRectSize()</c>
    /// </summary>
    LastItem,
}
