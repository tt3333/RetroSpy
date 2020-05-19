@echo off
"c:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe" RetroSpy.sln /p:Configuration=Release /p:Platform=x86

if errorlevel 0 goto x64Build
echo Aborting release. Error during x86 build.
goto end

:x64Build
"c:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe" RetroSpy.sln /p:Configuration=Release /p:Platform=x64

if errorlevel 0 goto AnyCPUBuild
echo Aborting release. Error during x64 build.
goto end

:AnyCPUBuild
"c:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe" RetroSpy.sln /p:Configuration=Release /p:Platform="Any CPU"

if errorlevel 0 goto buildOK
echo Aborting release. Error during AnyCPU build.
goto end

:buildOK
del RetroSpy-release.zip

if exist "E:\src\certs\codesignpasswd.txt" (
    set /p codesignpasswd=<"E:\src\certs\codesignpasswd.txt"
)

cd bin\Release
if exist "E:\src\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x86\SignTool" sign /f "E:\src\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a Retrospy.exe
)
"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip RetroSpy.exe
cd ..\..

cd bin\x64\Release
if exist "E:\src\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x86\SignTool" sign /f "E:\src\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a Retrospy.x64.exe
)
copy RetroSpy.exe RetroSpy.x64.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip RetroSpy.x64.exe
cd ..\..\..

cd bin\x86\Release
if exist "E:\src\certs\codesign.pfx" (
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x86\SignTool" sign /f "E:\src\certs\codesign.pfx" /p %codesignpasswd% /tr http://timestamp.comodoca.com  /td sha256 /a Retrospy.x86.exe
)
copy RetroSpy.exe RetroSpy.x86.exe
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip RetroSpy.x86.exe
cd ..\..\..

;cd SharpDX\net45
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip SharpDX.dll
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip SharpDX.DirectInput.dll
;cd ..\..

;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip Renci.SshNet.dll

;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip beagle.dll

;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip skins
;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip firmware
;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip experimental
;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip MiSTer\retrospy
;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip keybindings.xml

:end
pause
