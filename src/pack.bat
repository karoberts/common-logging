cd Common.Logging.Core.DotNetCore

dotnet pack -c Release --include-source --include-symbols

cd ..
cd Common.Logging.DotNetCore

dotnet pack -c Release --include-source --include-symbols

cd ..

cd Common.Logging.NLog453

dotnet pack -c Release --include-source --include-symbols

cd ..

copy Common.Logging.Core.DotNetCore\bin\Release\*.symbols.nupkg .
copy Common.Logging.DotNetCore\bin\Release\*.symbols.nupkg .
copy Common.Logging.NLog453\bin\Release\*.symbols.nupkg .
