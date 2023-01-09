#!/bin/bash

rm -rf bin/Release/net7.0
rm -rf bin/Release/RetroSpy-macOS
rm -rf RetroSpy-macOS.zip

dotnet publish RetroSpyX/RetroSpyX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-x64 --self-contained -p:PublishSingleFile=true
if [ $? -ne 0 ] 
then 
    echo "Aborting release. Error during RetroSpyX build."
else
    dotnet publish GBPemuX/GBPemuX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-x64 --self-contained -p:PublishSingleFile=true
    if [ $? -ne 0 ] 
    then 
        echo "Aborting release. Error during GBPemuX build."
    else
        dotnet publish GBPUpdaterX/GBPUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-x64 --self-contained -p:PublishSingleFile=true
        if [ $? -ne 0 ] 
        then 
        echo "Aborting release. Error during GBPUpdater build."
        else
        dotnet publish UsbUpdaterX/UsbUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-x64 --self-contained -p:PublishSingleFile=true
        if [ $? -ne 0 ] 
        then 
            echo "Aborting release. Error during GBPUpdater build."
        else
            mv bin/Release/net7.0publish/RetroSpy bin/Release/net7.0publish/RetroSpy-x64
            mv bin/Release/net7.0publish/GBPemu bin/Release/net7.0publish/GBPemu-x64
            mv bin/Release/net7.0publish/GBPUpdater bin/Release/net7.0publish/GBPUpdater-x64
            mv bin/Release/net7.0publish/UsbUpdater bin/Release/net7.0publish/UsbUpdater-x64
	    fi
        fi
    fi
fi

dotnet publish RetroSpyX/RetroSpyX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-arm64 --self-contained -p:PublishSingleFile=true
if [ $? -ne 0 ] 
then 
    echo "Aborting release. Error during RetroSpyX build."
else
    dotnet publish GBPemuX/GBPemuX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-arm64 --self-contained -p:PublishSingleFile=true
    if [ $? -ne 0 ] 
    then 
        echo "Aborting release. Error during GBPemuX build."
    else
        dotnet publish GBPUpdaterX/GBPUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-arm64 --self-contained -p:PublishSingleFile=true
        if [ $? -ne 0 ] 
        then 
        echo "Aborting release. Error during GBPUpdater build."
        else
        dotnet publish UsbUpdaterX/UsbUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r osx-arm64 --self-contained -p:PublishSingleFile=true
        if [ $? -ne 0 ] 
        then 
            echo "Aborting release. Error during GBPUpdater build."
        else
            mv bin/Release/net7.0publish/RetroSpy bin/Release/net7.0publish/RetroSpy-arm64
            mv bin/Release/net7.0publish/GBPemu bin/Release/net7.0publish/GBPemu-arm64
            mv bin/Release/net7.0publish/GBPUpdater bin/Release/net7.0publish/GBPUpdater-arm64
            mv bin/Release/net7.0publish/UsbUpdater bin/Release/net7.0publish/UsbUpdater-arm64

            lipo -create -output bin/Release/net7.0publish/RetroSpy bin/Release/net7.0publish/RetroSpy-x64 bin/Release/net7.0publish/RetroSpy-arm64
            lipo -create -output bin/Release/net7.0publish/GBPemu bin/Release/net7.0publish/GBPemu-x64 bin/Release/net7.0publish/GBPemu-arm64
            lipo -create -output bin/Release/net7.0publish/GBPUpdater bin/Release/net7.0publish/GBPUpdater-x64 bin/Release/net7.0publish/GBPUpdater-arm64
            lipo -create -output bin/Release/net7.0publish/UsbUpdater bin/Release/net7.0publish/UsbUpdater-x64 bin/Release/net7.0publish/UsbUpdater-arm64

            rm bin/Release/net7.0publish/*-arm64
            rm bin/Release/net7.0publish/*-x64

            cd bin/Release/
            cp -r net7.0publish RetroSpy-macOS
            codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS/*.dylib
            codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS/osx.os
            codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS/RetroSpy
            codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS/GBPemu
            codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS/GBPUpdater
            codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS/UsbUpdater
	        ditto -c --sequesterRsrc -k RetroSpy-macOS/ ../../RetroSpy-macOS.zip
            xcrun notarytool submit ../../RetroSpy-macOS.zip --wait --apple-id "$apple_username" --password "$apple_password" --team-id "$apple_teamid" --output-format json
            if [ -d "/Volumes/src/upload" ]
            then
            cp ../../RetroSpy-macOS.zip /Volumes/src/upload  
            fi
            cd ../..
	    fi
        fi
    fi
fi
