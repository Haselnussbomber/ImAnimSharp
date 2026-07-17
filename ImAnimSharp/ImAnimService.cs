using System;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin;

namespace ImAnimSharp;

public unsafe class ImAnimService : IDisposable
{
    private readonly IDalamudPluginInterface _pluginInterface;
    private int _gcCounter;

    /// <summary>
    /// Initializes ImAnim, syncing it with the current ImGui context and allocators.
    /// </summary>
    public ImAnimService(IDalamudPluginInterface pluginInterface)
    {
        _pluginInterface = pluginInterface;

        ImAnim.SetImGuiContext(ImGui.GetCurrentContext());

        delegate*<nuint, void*, void*> pAllocFunc = null;
        delegate*<void*, void*, void> pFreeFunc = null;
        void* pUserData = null;

        ImGui.GetAllocatorFunctions(&pAllocFunc, &pFreeFunc, &pUserData);
        ImAnim.SetImGuiAllocatorFunctions(pAllocFunc, pFreeFunc, pUserData);

        _pluginInterface.UiBuilder.Draw += OnDraw;
    }

    private void OnDraw()
    {
        ImAnim.UpdateBeginFrame();
        ImAnim.ClipUpdate(ImGui.GetIO().DeltaTime);

        if (++_gcCounter >= 300)
        {
            _gcCounter = 0;
            ImAnim.Gc(600);
            ImAnim.ClipGc(600);
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        _pluginInterface.UiBuilder.Draw -= OnDraw;
        ImAnim.ClipShutdown();
        ImAnim.SetImGuiContext(null);
        ImAnim.SetImGuiAllocatorFunctions(null, null);
    }
}
