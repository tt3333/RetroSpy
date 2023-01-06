#!/bin/bash

rm -rf bin/Release/net7.0
rm -rf bin/Release/RetroSpy-macOS-x64
rm -rf bin/Release/RetroSpy-macOS-arm64
rm -rf RetroSpy-macOS-x64.zip
rm -rf RetroSpy-macOS-arm64.zip

git pull

dotnet build RetroSpyX/RetroSpyX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-x64
if [ $? -ne 0 ] 
then 
    echo "Aborting release. Error during RetroSpyX build."
else
    dotnet build GBPemuX/GBPemuX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-x64
    if [ $? -ne 0 ] 
    then 
        echo "Aborting release. Error during GBPemuX build."
    else
        dotnet build GBPUpdaterX/GBPUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-x64 
        if [ $? -ne 0 ] 
        then 
        echo "Aborting release. Error during GBPUpdater build."
        else
        dotnet build UsbUpdaterX/UsbUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-x64
        if [ $? -ne 0 ] 
        then 
            echo "Aborting release. Error during GBPUpdater build."
        else
            cd bin/Release/
            cp -r net7.0 RetroSpy-macOS-x64
            codesign --force --verbose --timestamp --sign "USGM6XTVY2" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS-x64/*.dylib
            codesign --force --verbose --timestamp --sign "USGM6XTVY2" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS-x64/osx.os
            codesign --force --verbose --timestamp --sign "USGM6XTVY2" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS-x64/createdump
            codesign --force --verbose --timestamp --sign "USGM6XTVY2" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS-x64/RetroSpy
            codesign --force --verbose --timestamp --sign "USGM6XTVY2" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS-x64/GBPemu
            codesign --force --verbose --timestamp --sign "USGM6XTVY2" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS-x64/GBPUpdater
            codesign --force --verbose --timestamp --sign "USGM6XTVY2" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS-x64/USBUpdater
	    ditto -c --sequesterRsrc -k -V RetroSpy-macOS-x64/ ../../RetroSpy-macOS-x64.zip
            xcrun notarytool submit ../../RetroSpy-macOS-x64.zip --wait --apple-id "christopher.mallery@gmail.com" --password "icza-urwv-wmny-mvkf" --team-id "USGM6XTVY2" --output-format json
            if [ -d "/Volumes/src/upload" ]
            then
            cp ../../RetroSpy-macOS-x64.zip /Volumes/src/upload  
            fi
            cd ../..
	    fi
        fi
    fi
fi

rm -rf bin/Release/net7.0

dotnet build RetroSpyX/RetroSpyX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-arm64
if [ $? -ne 0 ] 
then 
    echo "Aborting release. Error during RetroSpyX build."
else
    dotnet build GBPemuX/GBPemuX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-arm64
    if [ $? -ne 0 ] 
    then 
        echo "Aborting release. Error during GBPemuX build."
    else
        dotnet build GBPUpdaterX/GBPUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-arm64 
        if [ $? -ne 0 ] 
        then 
        echo "Aborting release. Error during GBPUpdater build."
        else
        dotnet build UsbUpdaterX/UsbUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-arm64
        if [ $? -ne 0 ] 
        then 
            echo "Aborting release. Error during GBPUpdater build."
        else
            cd bin/Release/
            cp -r net7.0 RetroSpy-macOS-arm64
            codesign --force --verbose --timestamp --sign "USGM6XTVY2" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS-arm64/*.dylib
            codesign --force --verbose --timestamp --sign "USGM6XTVY2" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS-arm64/osx.os
            codesign --force --verbose --timestamp --sign "USGM6XTVY2" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS-arm64/createdump
            codesign --force --verbose --timestamp --sign "USGM6XTVY2" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS-arm64/RetroSpy
            codesign --force --verbose --timestamp --sign "USGM6XTVY2" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS-arm64/GBPemu
            codesign --force --verbose --timestamp --sign "USGM6XTVY2" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS-arm64/GBPUpdater
            codesign --force --verbose --timestamp --sign "USGM6XTVY2" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS-arm64/USBUpdater
	        ditto -c --sequesterRsrc -k -V RetroSpy-macOS-arm64/ ../../RetroSpy-macOS-arm64.zip
            xcrun notarytool submit ../../RetroSpy-macOS-arm64.zip --wait --apple-id "christopher.mallery@gmail.com" --password "icza-urwv-wmny-mvkf" --team-id "USGM6XTVY2" --output-format json
            if [ -d "/Volumes/src/upload" ]
            then
            cp ../../RetroSpy-macOS-arm64.zip /Volumes/src/upload  
            fi
            cd ../..
	    fi
        fi
    fi
fi
