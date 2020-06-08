@echo off
"c:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe" RetroSpy.csproj /p:Configuration=Release /p:Platform=x86 /p:OutputPath=bin\x86\Release

if errorlevel 0 goto x64Build
echo Aborting release. Error during x86 build.
goto end

:x64Build
"c:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe" RetroSpy.csproj /p:Configuration=Release /p:Platform=x64 /p:OutputPath=bin\x64\Release

if errorlevel 0 goto AnyCPUBuild
echo Aborting release. Error during x64 build.
goto end

:AnyCPUBuild
"c:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe" RetroSpy.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=bin\Release

if errorlevel 0 goto :MiSTer
echo Aborting release. Error during AnyCPU build.
goto end

:MiSTer
cd MiSTer
"c:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe" MiSTer.vcxproj /p:Configuration=Release /p:Platform="Win32" /p:OutputPath=Release
cd ..

if errorlevel 0 goto buildOK
echo Aborting release. Error during MiSTer build.
goto end

:buildOK
del RetroSpy-release.zip
del RetroSpy-release.zip.*
rmdir /S /Q RetroSpy-release
del RetroSpy-Setup.exe

mkdir RetroSpy-release

if exist "E:\src\certs\codesignpasswd.txt" (
    set /p codesignpasswd=<"E:\src\certs\codesignpasswd.txt"
)

cd bin\Release
if exist "E:\src\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x86\SignTool" sign /f "E:\src\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a Retrospy.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip RetroSpy.exe
copy RetroSpy.exe ..\..\RetroSpy-release
cd ..\..

cd bin\x64\Release
if exist "E:\src\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x86\SignTool" sign /f "E:\src\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a Retrospy.x64.exe
)
copy RetroSpy.exe RetroSpy.x64.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip RetroSpy.x64.exe
copy RetroSpy.x64.exe ..\..\..\RetroSpy-release\
cd ..\..\..

cd bin\x86\Release
if exist "E:\src\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x86\SignTool" sign /f "E:\src\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a Retrospy.x86.exe
)
copy RetroSpy.exe RetroSpy.x86.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip RetroSpy.x86.exe
copy RetroSpy.x86.exe ..\..\..\RetroSpy-release\
cd ..\..\..

;cd SharpDX\net45
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip SharpDX.dll
;copy SharpDX.dll ..\..\RetroSpy-release
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip SharpDX.DirectInput.dll
;copy SharpDX.DirectInput.dll ..\..\RetroSpy-release
;cd ..\..

;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip Renci.SshNet.dll
;copy Renci.SshNet.dll RetroSpy-release

;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip beagle.dll
;copy beagle.dll RetroSpy-release\

;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip skins
;xcopy /y /e /s skins\ RetroSpy-release\skins\
;cd bin\Release\
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip firmware
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip experimental
;cd ..\..
;xcopy /y /e /s bin\Release\firmware\ RetroSpy-release\firmware\
;xcopy /y /e /s bin\Release\experimental\ RetroSpy-release\experimental\
mkdir RetroSpy-release\MiSTer
;copy MiSTer\Release\retrospy RetroSpy-release\MiSTer
;cd RetroSpy-release
;"C:\Program Files\7-Zip\7z.exe" a ..\RetroSpy-release.zip MiSTer\retrospy
;cd ..
;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip keybindings.xml
;copy keybindings.xml RetroSpy-release

;mkdir RetroSpy-release\drivers\
;xcopy /y /e /s drivers\ RetroSpy-release\drivers\
;copy serial_install.exe RetroSpy-release\
;copy CH34x_Install_Windows_v3_4.EXE RetroSpy-release\

if exist "C:\Program Files (x86)\Actual Installer\actinst.exe" (
"C:\Program Files (x86)\Actual Installer\actinst.exe" /S RetroSpy.aip
if exist "E:\src\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x86\SignTool" sign /f "E:\src\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a Retrospy-Setup.exe
)
)

:end
pause
