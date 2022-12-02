@echo off

IF "%~1"=="" GOTO release

IF NOT "%~1"=="" set sub=1

:release
"C:\Program Files\dotnet\dotnet.exe" build RetroSpy.csproj /p:Configuration=Release /p:Platform=x86 /p:OutputPath=bin\x86\Release

if errorlevel 0 goto x64Build
echo Aborting release. Error during x86 build.
goto end

:x64Build
"C:\Program Files\dotnet\dotnet.exe" build RetroSpy.csproj /p:Configuration=Release /p:Platform=x64 /p:OutputPath=bin\x64\Release

if errorlevel 0 goto AnyCPUBuild
echo Aborting release. Error during x64 build.
goto end

:AnyCPUBuild
"C:\Program Files\dotnet\dotnet.exe" build RetroSpy.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=bin\Release

if errorlevel 0 goto :releasegb
echo Aborting release. Error during AnyCPU build.
goto end

:releasegb
"C:\Program Files\dotnet\dotnet.exe" build GBemu\GBPemu.csproj /p:Configuration=Release /p:Platform=x86 /p:OutputPath=..\bin\x86\Release

if errorlevel 0 goto x64Buildgb
echo Aborting release. Error during x86 build.
goto end

:x64Buildgb
"C:\Program Files\dotnet\dotnet.exe" build GBemu\GBPemu.csproj /p:Configuration=Release /p:Platform=x64 /p:OutputPath=..\bin\x64\Release

if errorlevel 0 goto AnyCPUBuildgb
echo Aborting release. Error during x64 build.
goto end

:AnyCPUBuildgb
"C:\Program Files\dotnet\dotnet.exe" build GBemu\GBPemu.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=..\bin\Release

if errorlevel 0 goto releaseuu
echo Aborting release. Error during AnyCPU build.
goto end

:releaseuu
"C:\Program Files\dotnet\dotnet.exe" build UsbUpdater\UsbUpdater.csproj /p:Configuration=Release /p:Platform=x86 /p:OutputPath=..\bin\x86\Release

if errorlevel 0 goto x64Builduu
echo Aborting release. Error during x86 build.
goto end

:x64Builduu
"C:\Program Files\dotnet\dotnet.exe" build UsbUpdater\UsbUpdater.csproj /p:Configuration=Release /p:Platform=x64 /p:OutputPath=..\bin\x64\Release

if errorlevel 0 goto AnyCPUBuilduu
echo Aborting release. Error during x64 build.
goto end

:AnyCPUBuilduu
"C:\Program Files\dotnet\dotnet.exe" build UsbUpdater\UsbUpdater.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=..\bin\Release

if errorlevel 0 goto releasegpbu
echo Aborting release. Error during x64 build.
goto end

:releasegpbu
"C:\Program Files\dotnet\dotnet.exe" build GBPUpdater\GBPUpdater.csproj /p:Configuration=Release /p:Platform=x86 /p:OutputPath=..\bin\x86\Release

if errorlevel 0 goto x64Buildgpbu
echo Aborting release. Error during x86 build.
goto end

:x64Buildgpbu
"C:\Program Files\dotnet\dotnet.exe" build GBPUpdater\GBPUpdater.csproj /p:Configuration=Release /p:Platform=x64 /p:OutputPath=..\bin\x64\Release

if errorlevel 0 goto AnyCPUBuildgpbu
echo Aborting release. Error during x64 build.
goto end

:AnyCPUBuildgpbu
"C:\Program Files\dotnet\dotnet.exe" build GBPUpdater\GBPUpdater.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=..\bin\Release


if errorlevel 0 goto :MiSTer
echo Aborting release. Error during AnyCPU build.
goto end

:MiSTer
cd MiSTer
"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\msbuild.exe" MiSTer.vcxproj /p:Configuration=Release /p:Platform="Win32" /p:OutputPath=Release
cd ..

if errorlevel 0 goto buildOK
echo Aborting release. Error during MiSTer build.
goto end

:buildOK
del RetroSpy-release.zip
del RetroSpy-release.zip.*
rmdir /S /Q RetroSpy-Setup
del RetroSpy-Setup.exe
rmdir /S /Q RetroSpy-Upload

mkdir RetroSpy-Setup

if exist "..\..\certs\codesignpasswd.txt" (
    set /p codesignpasswd=<"..\..\certs\codesignpasswd.txt"
)

cd bin\Release
if exist "..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a Retrospy.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip RetroSpy.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip RetroSpy.exe.config
copy RetroSpy.exe ..\..\RetroSpy-Setup
copy RetroSpy.exe.config ..\..\RetroSpy-Setup

if exist "..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a GBPemu.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip GBPemu.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip GBPemu.exe.config
"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip game_palettes.cfg
copy GBPemu.exe ..\..\RetroSpy-Setup
copy GBPemu.exe.config ..\..\RetroSpy-Setup
copy game_palettes.cfg ..\..\RetroSpy-Setup

if exist "..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a UsbUpdater.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip UsbUpdater.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip UsbUpdater.exe.config
copy UsbUpdater.exe ..\..\RetroSpy-Setup
copy UsbUpdater.exe.config ..\..\RetroSpy-Setup

if exist "..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a GBPUpdater.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip GBPUpdater.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip GBPUpdater.exe.config
copy GBPUpdater.exe ..\..\RetroSpy-Setup
copy GBPUpdater.exe.config ..\..\RetroSpy-Setup

cd ..\..

cd bin\x64\Release
copy RetroSpy.exe RetroSpy.x64.exe
copy RetroSpy.exe.config RetroSpy.x64.exe.config
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a Retrospy.x64.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip RetroSpy.x64.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip RetroSpy.x64.exe.config
copy RetroSpy.x64.exe ..\..\..\RetroSpy-Setup\
copy RetroSpy.x64.exe.config ..\..\..\RetroSpy-Setup\

copy GBPemu.exe GBPemu.x64.exe
copy GBPemu.exe.config GBPemu.x64.exe.config
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a GBPemu.x64.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip GBPemu.x64.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip GBPemu.x64.exe.config
copy GBPemu.x64.exe ..\..\..\RetroSpy-Setup\
copy GBPemu.x64.exe.config ..\..\..\RetroSpy-Setup\


copy UsbUpdater.exe UsbUpdater.x64.exe
copy UsbUpdater.exe UsbUpdater.x64.exe.config
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a UsbUpdater.x64.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip UsbUpdater.x64.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip UsbUpdater.x64.exe.config
copy UsbUpdater.x64.exe ..\..\..\RetroSpy-Setup\
copy UsbUpdater.x64.exe.config ..\..\..\RetroSpy-Setup\

copy GBPUpdater.exe GBPUpdater.x64.exe
copy GBPUpdater.exe GBPUpdater.x64.exe.config
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a GBPUpdater.x64.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip GBPUpdater.x64.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip GBPUpdater.x64.exe.config
copy GBPUpdater.x64.exe ..\..\..\RetroSpy-Setup\
copy GBPUpdater.x64.exe.config ..\..\..\RetroSpy-Setup\

cd ..\..\..

cd bin\x86\Release
copy RetroSpy.exe RetroSpy.x86.exe
copy RetroSpy.exe.config RetroSpy.x86.exe.config
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a Retrospy.x86.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip RetroSpy.x86.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip RetroSpy.x86.exe.config
copy RetroSpy.x86.exe ..\..\..\RetroSpy-Setup\
copy RetroSpy.x86.exe.config ..\..\..\RetroSpy-Setup\

copy GBPemu.exe GBPemu.x86.exe
copy GBPemu.exe.config GBPemu.x86.exe.config
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a GBPemu.x86.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip GBPemu.x86.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip GBPemu.x86.exe.config
copy GBPemu.x86.exe ..\..\..\RetroSpy-Setup\
copy GBPemu.x86.exe.config ..\..\..\RetroSpy-Setup\

copy UsbUpdater.exe UsbUpdater.x86.exe
copy UsbUpdater.exe.config UsbUpdater.x86.exe.config
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a UsbUpdater.x86.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip UsbUpdater.x86.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip UsbUpdater.x86.exe.config
copy UsbUpdater.x86.exe ..\..\..\RetroSpy-Setup\
copy UsbUpdater.x86.exe.config ..\..\..\RetroSpy-Setup\

copy GBPUpdater.exe GBPUpdater.x86.exe
copy GBPUpdater.exe.config GBPUpdater.x86.exe.config
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a GBPUpdater.x86.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip GBPUpdater.x86.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip GBPUpdater.x86.exe.config
copy GBPUpdater.x86.exe ..\..\..\RetroSpy-Setup\
copy GBPUpdater.x86.exe.config ..\..\..\RetroSpy-Setup\

cd ..\..\..

;cd SharpDX\net45
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip SharpDX.dll
;copy SharpDX.dll ..\..\RetroSpy-Setup
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip SharpDX.DirectInput.dll
;copy SharpDX.DirectInput.dll ..\..\RetroSpy-Setup
;cd ..\..

cd bin\Release\
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip System.Buffers.dll
;copy System.Buffers.dll ..\..\RetroSpy-Setup
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip System.Memory.dll
;copy System.Memory.dll ..\..\RetroSpy-Setup
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip System.Numerics.Vectors.dll
;copy System.Numerics.Vectors.dll ..\..\RetroSpy-Setup\
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip System.Resources.Extensions.dll
;copy System.Resources.Extensions.dll ..\..\RetroSpy-Setup\
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip System.Runtime.CompilerServices.Unsafe.dll
;copy System.Runtime.CompilerServices.Unsafe.dll ..\..\RetroSpy-Setup\
cd ..\..

;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip Renci.SshNet.dll
;copy Renci.SshNet.dll RetroSpy-Setup

;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip beagle.dll
;copy beagle.dll RetroSpy-Setup\

;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip Ookii.Dialogs.Wpf.dll
;copy Ookii.Dialogs.Wpf.dll RetroSpy-Setup\

;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip skins
;xcopy /y /e /s skins RetroSpy-Setup\skins\
;cd bin\Release\
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip firmware
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip experimental
;cd ..\..
;xcopy /y /e /s bin\Release\firmware RetroSpy-Setup\firmware\
;xcopy /y /e /s bin\Release\experimental RetroSpy-Setup\experimental\
mkdir RetroSpy-Setup\MiSTer
if "%sub%" == "1" ( CALL BatchSubstitute.bat "RELEASE_TAG" "%~1" MiSTer\update-retrospy-nightly.sh > RetroSpy-Setup\MiSTer\update-retrospy.sh) else (copy MiSTer\update-retrospy.sh RetroSpy-Setup\MiSTer)
;cd RetroSpy-Setup
;"C:\Program Files\7-Zip\7z.exe" a ..\RetroSpy-release.zip MiSTer\update-retrospy.sh
;cd ..
;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip keybindings.xml
;copy keybindings.xml RetroSpy-Setup

;mkdir RetroSpy-Setup\drivers\
;xcopy /y /e /s drivers RetroSpy-Setup\drivers\
;xcopy /y /e /s CH341SER RetroSpy-Setup\CH341SER\
;copy serial_install.exe RetroSpy-Setup\

if exist "C:\Program Files (x86)\Actual Installer\actinst.exe" (
"C:\Program Files (x86)\Actual Installer\actinst.exe" /S RetroSpy.aip
if exist "..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a Retrospy-Setup.exe
)
)

;mkdir RetroSpy-Upload
copy RetroSpy-Setup.exe RetroSpy-Upload
copy RetroSpy-release.zip RetroSpy-Upload
copy UsbUpdater\update-usb-retrospy-installer.sh RetroSpy-Upload
if "%sub%" == "1" ( CALL BatchSubstitute.bat "RELEASE_TAG" "%~1" MiSTer\update-retrospy-nightly-installer.sh > RetroSpy-Upload\update-retrospy-installer.sh) else (copy MiSTer\update-retrospy-installer.sh RetroSpy-Upload)
;copy MiSTer\Release\retrospy RetroSpy-Upload
if exist "..\..\GBP_Firmware\" (
del GBP_Firmware.zip
"C:\Program Files\7-Zip\7z.exe" a GBP_Firmware.zip ..\..\GBP_Firmware\avrdude.exe
"C:\Program Files\7-Zip\7z.exe" a GBP_Firmware.zip ..\..\GBP_Firmware\avrdude.conf
"C:\Program Files\7-Zip\7z.exe" a GBP_Firmware.zip ..\..\GBP_Firmware\firmware.ino.hex
"C:\Program Files\7-Zip\7z.exe" a GBP_Firmware.zip ..\..\GBP_Firmware\libusb0.dll
copy GBP_Firmware.zip RetroSpy-Upload
)
if exist "..\..\beaglebone\" (
FOR /F %%I IN ('DIR ..\..\beaglebone\*.xz /B /O:-D') DO COPY ..\..\beaglebone\%%I RetroSpy-Upload & goto end
)

:end
