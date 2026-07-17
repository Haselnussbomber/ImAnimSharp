using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin;

namespace ImAnimSharp;

public unsafe class ImAnimService : IDisposable
{
    private readonly IDalamudPluginInterface _pluginInterface;
    private nint _nativeHandle;
    private int _gcCounter;

    /// <summary>
    /// Initializes ImAnim, syncing it with the current ImGui context and allocators.
    /// </summary>
    public ImAnimService(IDalamudPluginInterface pluginInterface)
    {
        _pluginInterface = pluginInterface;

        NativeLibrary.SetDllImportResolver(typeof(ImAnimNative).Assembly, ResolveLibrary);

        ImAnim.SetImGuiContext(ImGui.GetCurrentContext());

        delegate*<nuint, void*, void*> pAllocFunc = null;
        delegate*<void*, void*, void> pFreeFunc = null;
        void* pUserData = null;

        ImGui.GetAllocatorFunctions(&pAllocFunc, &pFreeFunc, &pUserData);
        ImAnim.SetImGuiAllocatorFunctions(pAllocFunc, pFreeFunc, pUserData);

        _pluginInterface.UiBuilder.Draw += OnDraw;
    }

    private nint ResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (assembly == typeof(ImAnimNative).Assembly && libraryName == "cimanim")
        {
            return _nativeHandle != 0
                ? _nativeHandle
                : _nativeHandle = NativeLibrary.Load(libraryName, assembly, searchPath);
        }

        return 0;
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
        ImAnim.PoolClear();

        ImAnim.SetImGuiContext(null);
        ImAnim.SetImGuiAllocatorFunctions(null, null);

        if (_nativeHandle != 0)
        {
            NativeLibrary.Free(_nativeHandle);
            _nativeHandle = 0;
        }
    }
}
