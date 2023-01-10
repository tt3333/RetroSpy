@echo off

IF "%~1"=="" GOTO release

IF NOT "%~1"=="" set sub=1

:release
rmdir /S /Q bin\Release\net7.0
"C:\Program Files\dotnet\dotnet.exe" build RetroSpyX\RetroSpyX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=..\bin\Release\net7.0

if errorlevel 0 goto :AnyCPUBuildgb
echo Aborting release. Error during AnyCPU build.
goto end

:AnyCPUBuildgb
"C:\Program Files\dotnet\dotnet.exe" build GBPemuX\GBPemuX.csproj /p:Configuration=Release /p:Platform="Any CPU"  /p:OutputPath=..\bin\Release\net7.0

if errorlevel 0 goto AnyCPUBuilduu
echo Aborting release. Error during AnyCPU build.
goto end

:AnyCPUBuilduu
"C:\Program Files\dotnet\dotnet.exe" build UsbUpdaterX2\UsbUpdaterX2.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=..\bin\Release\net7.0

if errorlevel 0 goto AnyCPUBuildgpbu
echo Aborting release. Error during AnyCPU build.
goto end

:AnyCPUBuildgpbu
"C:\Program Files\dotnet\dotnet.exe" build GBPUpdaterX2\GBPUpdaterX2.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=..\bin\Release\net7.0

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
;del RetroSpy-Windows.zip
;del RetroSpy-Windows.zip.*
;rmdir /S /Q RetroSpy-Setup
;del RetroSpy-Setup.exe
;rmdir /S /Q RetroSpy-Upload

mkdir RetroSpy-Setup

if exist "..\..\..\certs\codesignpasswd.txt" (
    set /p codesignpasswd=<"..\..\..\certs\codesignpasswd.txt"
)

cd "bin\Release\net7.0\"

REM Sign all 4 executables
cd bin\Release\net7.0
if exist "..\..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.22000.0\x86\SignTool" sign /f "..\..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /fd sha256 /a Retrospy.exe)

if exist "..\..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.22000.0\x86\SignTool" sign /f "..\..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /fd sha256 /a GBPemu.exe)

if exist "..\..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.22000.0\x86\SignTool" sign /f "..\..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /fd sha256 /a UsbUpdater.exe)

if exist "..\..\..\..\..\..\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.22000.0\x86\SignTool" sign /f "..\..\..\..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /fd sha256 /a GBPUpdater.exe)

xcopy /y /e /s * ..\..\..\RetroSpy-Setup

cd ..\..\..

cd RetroSpy-Setup
;"C:\Program Files\7-Zip\7z.exe" -r a ..\RetroSpy-Windows.zip *.*
cd ..

mkdir RetroSpy-Setup\MiSTer
if "%sub%" == "1" ( sed -e s/RELEASE_TAG/%~1/g MiSTer\update-retrospy-nightly.sh > RetroSpy-Setup\MiSTer\update-retrospy.sh) else (copy MiSTer\update-retrospy.sh RetroSpy-Setup\MiSTer)
;cd RetroSpy-Setup
;"C:\Program Files\7-Zip\7z.exe" a ..\RetroSpy-Windows.zip MiSTer\update-retrospy.sh
;cd ..
REM ;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-Windows.zip keybindings.xml
REM ;copy keybindings.xml RetroSpy-Setup

REM Copy Drivers
;mkdir RetroSpy-Setup\drivers\
;xcopy /y /e /s drivers RetroSpy-Setup\drivers\
;xcopy /y /e /s CH341SER RetroSpy-Setup\CH341SER\
;copy serial_install.exe RetroSpy-Setup\

REM if exist "C:\Program Files (x86)\Actual Installer\actinst.exe" (
REM "C:\Program Files (x86)\Actual Installer\actinst.exe" /S RetroSpy.aip
REM if exist "..\..\..\certs\codesign.pfx" (
REM "C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\SignTool" sign /f "..\..\..\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a Retrospy-Setup.exe
REM )
REM )

;mkdir RetroSpy-Upload
copy RetroSpy-Setup.exe RetroSpy-Upload
copy RetroSpy-Windows.zip RetroSpy-Upload
copy UsbUpdaterX2\update-usb-retrospy-installer.sh RetroSpy-Upload
if "%sub%" == "1" ( sed -e s/RELEASE_TAG/%~1/g MiSTer\update-retrospy-nightly-installer.sh > RetroSpy-Upload\update-retrospy-installer.sh) else (copy MiSTer\update-retrospy-installer.sh RetroSpy-Upload)
;copy MiSTer\Release\retrospy RetroSpy-Upload
if exist "..\..\..\GBP_Firmware\" (
del GBP_Firmware.zip
"C:\Program Files\7-Zip\7z.exe" a GBP_Firmware.zip ..\..\..\GBP_Firmware\avrdude.exe
"C:\Program Files\7-Zip\7z.exe" a GBP_Firmware.zip ..\..\..\GBP_Firmware\avrdude.conf
"C:\Program Files\7-Zip\7z.exe" a GBP_Firmware.zip ..\..\..\GBP_Firmware\firmware.ino.hex
"C:\Program Files\7-Zip\7z.exe" a GBP_Firmware.zip ..\..\..\GBP_Firmware\libusb0.dll
copy GBP_Firmware.zip RetroSpy-Upload
)
if exist "..\..\..\kernel\kernel.tar.gz" (
copy ..\..\..\kernel\kernel.tar.gz RetroSpy-Upload
)
if exist "..\..\..\upload\RetroSpy-Linux.tar.gz" (
copy ..\..\..\upload\RetroSpy-Linux.tar.gz RetroSpy-Upload
)
if exist "..\..\..\upload\RetroSpyInstall.dmg" (
copy ..\..\..\upload\RetroSpyInstall.dmg RetroSpy-Upload
)
if exist "..\..\..\beaglebone\" (
FOR /F %%I IN ('DIR ..\..\..\beaglebone\*.xz /B /O:-D') DO COPY ..\..\..\beaglebone\%%I RetroSpy-Upload & goto end
)
:end
