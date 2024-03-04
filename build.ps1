msbuild src/LenovoController.sln /p:Configuration=Release /m /verbosity:normal /p:WarningLevel=0
Rename-Item -Path "src/bin/Release" -NewName "LenovoController"
Compress-Archive -Path "src/bin/LenovoController" -DestinationPath LenovoController.zip