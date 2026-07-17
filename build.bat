xmake f -m releasedbg
xmake
REM dotnet build ImAnimSharp/ImAnimSharp.csproj
dotnet pack ImAnimSharp/ImAnimSharp.csproj -o .
