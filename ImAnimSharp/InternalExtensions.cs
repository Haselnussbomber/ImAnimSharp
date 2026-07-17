using Dalamud.Bindings.ImGui;

namespace ImAnimSharp;

internal static class InternalExtensions
{
    extension(ImU8String str)
    {
        public uint GetId() => ImGuiP.ImHashStr(str);
    }
}
