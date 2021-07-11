@echo off

IF "%~1"=="" GOTO release

IF NOT "%~1"=="" set sub=1

:release
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" RetroSpy.csproj /p:Configuration=Release /p:Platform=x86 /p:OutputPath=bin\x86\Release

if errorlevel 0 goto x64Build
echo Aborting release. Error during x86 build.
goto end

:x64Build
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" RetroSpy.csproj /p:Configuration=Release /p:Platform=x64 /p:OutputPath=bin\x64\Release

if errorlevel 0 goto AnyCPUBuild
echo Aborting release. Error during x64 build.
goto end

:AnyCPUBuild
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" RetroSpy.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=bin\Release

if errorlevel 0 goto :releasegb
echo Aborting release. Error during AnyCPU build.
goto end

:releasegb
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" GBemu\GBPemu.csproj /p:Configuration=Release /p:Platform=x86 /p:OutputPath=..\bin\x86\Release

if errorlevel 0 goto x64Buildgb
echo Aborting release. Error during x86 build.
goto end

:x64Buildgb
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" GBemu\GBPemu.csproj /p:Configuration=Release /p:Platform=x64 /p:OutputPath=..\bin\x64\Release

if errorlevel 0 goto AnyCPUBuildgb
echo Aborting release. Error during x64 build.
goto end

:AnyCPUBuildgb
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" GBemu\GBPemu.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=..\bin\Release

if errorlevel 0 goto releaseuu
echo Aborting release. Error during AnyCPU build.
goto end

:releaseuu
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" UsbUpdater\UsbUpdater.csproj /p:Configuration=Release /p:Platform=x86 /p:OutputPath=..\bin\x86\Release

if errorlevel 0 goto x64Builduu
echo Aborting release. Error during x86 build.
goto end

:x64Builduu
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" UsbUpdater\UsbUpdater.csproj /p:Configuration=Release /p:Platform=x64 /p:OutputPath=..\bin\x64\Release

if errorlevel 0 goto AnyCPUBuilduu
echo Aborting release. Error during x64 build.
goto end

:AnyCPUBuilduu
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" UsbUpdater\UsbUpdater.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=..\bin\Release

if errorlevel 0 goto releasegpbu
echo Aborting release. Error during x64 build.
goto end

:releasegpbu
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" GBPUpdater\GBPUpdater.csproj /p:Configuration=Release /p:Platform=x86 /p:OutputPath=..\bin\x86\Release

if errorlevel 0 goto x64Buildgpbu
echo Aborting release. Error during x86 build.
goto end

:x64Buildgpbu
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" GBPUpdater\GBPUpdater.csproj /p:Configuration=Release /p:Platform=x64 /p:OutputPath=..\bin\x64\Release

if errorlevel 0 goto AnyCPUBuildgpbu
echo Aborting release. Error during x64 build.
goto end

:AnyCPUBuildgpbu
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" GBPUpdater\GBPUpdater.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=..\bin\Release


if errorlevel 0 goto :MiSTer
echo Aborting release. Error during AnyCPU build.
goto end

:MiSTer
cd MiSTer
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" MiSTer.vcxproj /p:Configuration=Release /p:Platform="Win32" /p:OutputPath=Release
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
copy RetroSpy.exe ..\..\RetroSpy-Setup

if exist "..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a GBPemu.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip GBPemu.exe
copy GBPemu.exe ..\..\RetroSpy-Setup

if exist "..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a UsbUpdater.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip UsbUpdater.exe
copy UsbUpdater.exe ..\..\RetroSpy-Setup

if exist "..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a GBPUpdater.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip GBPUpdater.exe
copy GBPUpdater.exe ..\..\RetroSpy-Setup

cd ..\..

cd bin\x64\Release
copy RetroSpy.exe RetroSpy.x64.exe
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a Retrospy.x64.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip RetroSpy.x64.exe
copy RetroSpy.x64.exe ..\..\..\RetroSpy-Setup\

copy GBPemu.exe GBPemu.x64.exe
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a GBPemu.x64.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip GBPemu.x64.exe
copy GBPemu.x64.exe ..\..\..\RetroSpy-Setup\

copy UsbUpdater.exe UsbUpdater.x64.exe
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a UsbUpdater.x64.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip UsbUpdater.x64.exe
copy UsbUpdater.x64.exe ..\..\..\RetroSpy-Setup\

copy GBPUpdater.exe GBPUpdater.x64.exe
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a GBPUpdater.x64.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip GBPUpdater.x64.exe
copy GBPUpdater.x64.exe ..\..\..\RetroSpy-Setup\

cd ..\..\..

cd bin\x86\Release
copy RetroSpy.exe RetroSpy.x86.exe
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a Retrospy.x86.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip RetroSpy.x86.exe
copy RetroSpy.x86.exe ..\..\..\RetroSpy-Setup\

copy GBPemu.exe GBPemu.x86.exe
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a GBPemu.x86.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip GBPemu.x86.exe
copy GBPemu.x86.exe ..\..\..\RetroSpy-Setup\

copy UsbUpdater.exe UsbUpdater.x86.exe
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a UsbUpdater.x86.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip UsbUpdater.x86.exe
copy UsbUpdater.x86.exe ..\..\..\RetroSpy-Setup\

copy GBPUpdater.exe GBPUpdater.x86.exe
if exist "..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a GBPUpdater.x86.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip GBPUpdater.x86.exe
copy GBPUpdater.x86.exe ..\..\..\RetroSpy-Setup\

cd ..\..\..

;cd SharpDX\net45
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip SharpDX.dll
;copy SharpDX.dll ..\..\RetroSpy-Setup
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip SharpDX.DirectInput.dll
;copy SharpDX.DirectInput.dll ..\..\RetroSpy-Setup
;cd ..\..

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
if exist "..\..\beaglebone\" (
FOR /F %%I IN ('DIR ..\..\beaglebone\*.xz /B /O:-D') DO COPY ..\..\beaglebone\%%I RetroSpy-Upload & goto end
)
:end
