add_rules("mode.debug", "mode.releasedbg")

if is_mode("debug") then
    set_runtimes("MDd") -- MultiThreadedDebugDLL
else
    set_runtimes("MD") -- MultiThreadedDLL
    add_ldflags("/OPT:REF", "/OPT:ICF") -- OptimizeReferences & EnableCOMDATFolding
end

target("imgui")
    set_kind("static")
    add_files(
      "lib/cimgui/imgui/imgui.cpp",
      "lib/cimgui/imgui/imgui_demo.cpp",
      "lib/cimgui/imgui/imgui_draw.cpp",
      "lib/cimgui/imgui/imgui_tables.cpp",
      "lib/cimgui/imgui/imgui_widgets.cpp")
    add_includedirs("lib/cimgui/imgui")

target("cimgui")
    set_kind("static")
    add_files("lib/cimgui/cimgui.cpp")
    add_includedirs("lib/cimgui", "lib/cimgui/imgui")
    add_deps("imgui")

target("ImAnim")
    set_kind("static")
    add_files(
      "lib/ImAnim/im_anim.cpp",
      "lib/ImAnim/im_anim_demo.cpp")
    add_includedirs("lib/cimgui/imgui")
    add_deps("imgui")

target("cimanim")
    set_kind("shared")
    add_headerfiles("cimanim/cimanim.h")
    add_files("cimanim/cimanim.cpp")
    add_includedirs("lib/cimgui/imgui", "lib/cimgui", "lib/ImAnim", "cimanim")
    add_deps("ImAnim")
    add_ldflags("/MANIFESTUAC:NO") -- EnableUAC=false
