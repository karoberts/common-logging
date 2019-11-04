cd Common.Logging.Core.DotNetCore

dotnet pack -c Release --include-source --include-symbols

cd ..
cd Common.Logging.DotNetCore

dotnet pack -c Release --include-source --include-symbols

cd ..

cd Common.Logging.NLog453

dotnet pack -c Release --include-source --include-symbols

cd ..

cd Common.Logging.MicrosoftLogging

dotnet pack -c Release --include-source --include-symbols

cd ..

move Common.Logging.Core.DotNetCore\bin\Release\*.symbols.nupkg .
move Common.Logging.DotNetCore\bin\Release\*.symbols.nupkg .
move Common.Logging.NLog453\bin\Release\*.symbols.nupkg .
move Common.Logging.MicrosoftLogging\bin\Release\*.symbols.nupkg .
