#!/bin/bash

rm -rf bin/Release/RetroSpy-macOS
rm -rf RetroSpy-macOS.zip

mkdir bin/Release/RetroSpy-macOS

rm -rf bin/Release/net7.0
dotnet publish RetroSpyX/RetroSpyX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0/ /p:RuntimeIdentifier=osx-x64 /p:SelfContained=true -p:PublishSingleFile=true -p:UseAppHost=true
mv bin/Release/net7.0/publish/RetroSpy bin/Release/net7.0/publish/RetroSpy-x64
mv bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib-x64
dotnet publish RetroSpyX/RetroSpyX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0/ /p:RuntimeIdentifier=osx-arm64 /p:SelfContained=true -p:PublishSingleFile=true -p:UseAppHost=true
mv bin/Release/net7.0/publish/RetroSpy bin/Release/net7.0/publish/RetroSpy-arm64
mv bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib-arm64
lipo -create -output bin/Release/net7.0/publish/RetroSpy bin/Release/net7.0/publish/RetroSpy-arm64 bin/Release/net7.0/publish/RetroSpy-x64
lipo -create -output bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib-arm64 bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib-x64
rm bin/Release/net7.0/publish/*-arm64
rm bin/Release/net7.0/publish/*-x64

mkdir bin/Release/RetroSpy-macOS/RetroSpy.app
mkdir bin/Release/RetroSpy-macOS/RetroSpy.app/Contents
mkdir bin/Release/RetroSpy-macOS/RetroSpy.app/Contents/MacOS
mkdir bin/Release/RetroSpy-macOS/RetroSpy.app/Contents/Resources
cp RetroSpyX/Info.plist bin/Release/RetroSpy-macOS/RetroSpy.app/Contents/
cp RetroSpyX/RetroSpy.icns bin/Release/RetroSpy-macOS/RetroSpy.app/Contents/Resources/
cp -aR bin/Release/net7.0/publish/* bin/Release/RetroSpy-macOS/RetroSpy.app/Contents/MacOS
mv bin/Release/RetroSpy-macOS/RetroSpy.app/Contents/MacOS/skins bin/Release/RetroSpy-macOS/
mv bin/Release/RetroSpy-macOS/RetroSpy.app/Contents/MacOS/keybindings.xml bin/Release/RetroSpy-macOS/
cp -aR bin/Release/net7.0/firmware bin/Release/RetroSpy-macOS/
 
rm -rf bin/Release/net7.0
dotnet publish GBPemuX/GBPemuX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0/ /p:RuntimeIdentifier=osx-x64 /p:SelfContained=true -p:PublishSingleFile=true -p:UseAppHost=true
mv bin/Release/net7.0/publish/GBPemu bin/Release/net7.0/publish/GBPemu-x64
mv bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib-x64
dotnet publish GBPemuX/GBPemuX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0/ /p:RuntimeIdentifier=osx-arm64 /p:SelfContained=true -p:PublishSingleFile=true -p:UseAppHost=true
mv bin/Release/net7.0/publish/GBPemu bin/Release/net7.0/publish/GBPemu-arm64
mv bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib-arm64
lipo -create -output bin/Release/net7.0/publish/GBPemu bin/Release/net7.0/publish/GBPemu-arm64 bin/Release/net7.0/publish/GBPemu-x64
lipo -create -output bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib-arm64 bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib-x64
rm bin/Release/net7.0/publish/*-arm64
rm bin/Release/net7.0/publish/*-x64

mkdir bin/Release/RetroSpy-macOS/GBPemu.app
mkdir bin/Release/RetroSpy-macOS/GBPemu.app/Contents
mkdir bin/Release/RetroSpy-macOS/GBPemu.app/Contents/MacOS
mkdir bin/Release/RetroSpy-macOS/GBPemu.app/Contents/Resources
cp GBPemuX/Info.plist bin/Release/RetroSpy-macOS/GBPemu.app/Contents/
cp GBPemuX/GBPemu.icns bin/Release/RetroSpy-macOS/GBPemu.app/Contents/Resources/
cp -aR bin/Release/net7.0/publish/* bin/Release/RetroSpy-macOS/GBPemu.app/Contents/MacOS
mv bin/Release/RetroSpy-macOS/GBPemu.app/Contents/MacOS/game_palettes.cfg bin/Release/RetroSpy-macOS/

rm -rf bin/Release/net7.0
dotnet publish GBPUpdaterX/GBPUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0/ /p:RuntimeIdentifier=osx-x64 /p:SelfContained=true -p:PublishSingleFile=true -p:UseAppHost=true
mv bin/Release/net7.0/publish/GBPUpdater bin/Release/net7.0/publish/GBPUpdater-x64
mv bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib-x64
dotnet publish GBPUpdaterX/GBPUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0/ /p:RuntimeIdentifier=osx-arm64 /p:SelfContained=true -p:PublishSingleFile=true -p:UseAppHost=true
mv bin/Release/net7.0/publish/GBPUpdater bin/Release/net7.0/publish/GBPUpdater-arm64
mv bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib-arm64
lipo -create -output bin/Release/net7.0/publish/GBPUpdater bin/Release/net7.0/publish/GBPUpdater-arm64 bin/Release/net7.0/publish/GBPUpdater-x64
lipo -create -output bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib-arm64 bin/Release/net7.0/publish/libSystem.IO.Ports.Native.dylib-x64
rm bin/Release/net7.0/publish/*-arm64
rm bin/Release/net7.0/publish/*-x64

mkdir bin/Release/RetroSpy-macOS/GBPUpdater.app
mkdir bin/Release/RetroSpy-macOS/GBPUpdater.app/Contents
mkdir bin/Release/RetroSpy-macOS/GBPUpdater.app/Contents/MacOS
mkdir bin/Release/RetroSpy-macOS/GBPUpdater.app/Contents/Resources
cp GBPUpdaterX/Info.plist bin/Release/RetroSpy-macOS/GBPUpdater.app/Contents/
#cp GBPUpdaterX/GBPUpdater.icns bin/Release/RetroSpy-macOS/GBPUpdater.app/Contents/Resources/
cp -aR bin/Release/net7.0/publish/* bin/Release/RetroSpy-macOS/GBPUpdater.app/Contents/MacOS
#chflags hidden bin/Release/RetroSpy-macOS/GBPUpdater.app/
cp GBPUpdaterX/GBPUpdater bin/Release/RetroSpy-macOS/

rm -rf bin/Release/net7.0
dotnet publish UsbUpdaterX/UsbUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0/ /p:RuntimeIdentifier=osx-x64 /p:SelfContained=true -p:PublishSingleFile=true -p:UseAppHost=true
mv bin/Release/net7.0/publish/UsbUpdater bin/Release/net7.0/publish/UsbUpdater-x64
dotnet publish UsbUpdaterX/UsbUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0/ /p:RuntimeIdentifier=osx-arm64 /p:SelfContained=true -p:PublishSingleFile=true -p:UseAppHost=true
mv bin/Release/net7.0/publish/UsbUpdater bin/Release/net7.0/publish/UsbUpdater-arm64
lipo -create -output bin/Release/net7.0/publish/UsbUpdater bin/Release/net7.0/publish/UsbUpdater-arm64 bin/Release/net7.0/publish/UsbUpdater-x64
rm bin/Release/net7.0/publish/*-arm64
rm bin/Release/net7.0/publish/*-x64

mkdir bin/Release/RetroSpy-macOS/UsbUpdater.app
mkdir bin/Release/RetroSpy-macOS/UsbUpdater.app/Contents
mkdir bin/Release/RetroSpy-macOS/UsbUpdater.app/Contents/MacOS
mkdir bin/Release/RetroSpy-macOS/UsbUpdater.app/Contents/Resources
cp UsbUpdaterX/Info.plist bin/Release/RetroSpy-macOS/UsbUpdater.app/Contents/
#cp UsbUpdaterX/UsbUpdater.icns bin/Release/RetroSpy-macOS/UsbUpdater.app/Contents/Resources/
cp -aR bin/Release/net7.0/publish/* bin/Release/RetroSpy-macOS/UsbUpdater.app/Contents/MacOS
#chflags hidden bin/Release/RetroSpy-macOS/UsbUpdater.app/
cp UsbUpdaterX/UsbUpdater bin/Release/RetroSpy-macOS/

cd bin/Release/RetroSpy-macOS/

find "RetroSpy.app/Contents/MacOS/"|while read fname; do
  if [[ -f $fname ]]; then
    echo "[INFO] Signing $fname"
    codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../../entitlements.plist "$fname"
  fi
done
echo "[INFO] Signing app file"
codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../../entitlements.plist RetroSpy.app

find "GBPemu.app/Contents/MacOS/"|while read fname; do
  if [[ -f $fname ]]; then
    echo "[INFO] Signing $fname"
    codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../../entitlements.plist "$fname"
  fi
done
echo "[INFO] Signing app file"
codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../../entitlements.plist GBPemu.app

find "GBPUpdater.app/Contents/MacOS/"|while read fname; do
  if [[ -f $fname ]]; then
    echo "[INFO] Signing $fname"
    codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../../entitlements.plist "$fname"
  fi
done
echo "[INFO] Signing app file"
codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../../entitlements.plist GBPUpdater.app
codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../../entitlements.plist GBPUpdater

find "UsbUpdater.app/Contents/MacOS/"|while read fname; do
  if [[ -f $fname ]]; then
    echo "[INFO] Signing $fname"
    codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../../entitlements.plist "$fname"
  fi
done
echo "[INFO] Signing app file"
codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../../entitlements.plist UsbUpdater.app
codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../../entitlements.plist UsbUpdater

cd ..
ditto -c --sequesterRsrc -k RetroSpy-macOS/ ../../RetroSpy-macOS.zip
xcrun notarytool submit ../../RetroSpy-macOS.zip --wait --apple-id "$apple_username" --password "$apple_password" --team-id "$apple_teamid" --output-format json

xcrun stapler staple RetroSpy-macOS/RetroSpy.app
xcrun stapler staple RetroSpy-macOS/GBPemu.app
xcrun stapler staple RetroSpy-macOS/GBPUpdater.app
xcrun stapler staple RetroSpy-macOS/UsbUpdater.app

rm ../../RetroSpy-macOS.zip
#ditto -c --sequesterRsrc -k RetroSpy-macOS/ ../../RetroSpy-macOS.zip

rm -rf RetroSpyInstall
rm -rf ../../RetroSpyInstall.dmg
mkdir RetroSpyInstall
mkdir RetroSpyInstall/RetroSpy
cp -aR RetroSpy-macOS/* RetroSpyInstall/RetroSpy/

hdiutil create /tmp/tmp.dmg -ov -volname "RetroSpyInstall" -fs HFS+ -srcfolder "RetroSpyInstall"
hdiutil convert /tmp/tmp.dmg -format UDZO -o ../../RetroSpyInstall.dmg 

rm -rf RetroSpyInstall

xcrun notarytool submit ../../RetroSpyInstall.dmg --wait --apple-id "$apple_username" --password "$apple_password" --team-id "$apple_teamid" --output-format json
xcrun stapler staple ../../RetroSpyInstall.dmg


if [ -d "/Volumes/src/upload" ]
then
  cp ../../RetroSpyInstall.dmg /Volumes/src/upload  
fi
cd ../..

