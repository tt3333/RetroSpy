#!/bin/bash

rm -rf bin/Release/
rm -rf bin/Release/RetroSpy-macOS
rm -rf RetroSpy-macOS.zip

dotnet msbuild RetroSpyX/RetroSpyX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:RuntimeIdentifier=osx-x64 /p:OutputPath=../bin/RetroSpy/Release/ /p:SelfContained=true -p:PublishSingleFile=true -t:BundleApp -p:CFBundleShortVersionString=6.0 -p:UseAppHost=true
if [ $? -ne 0 ] 
then 
    echo "Aborting release. Error during RetroSpyX build."
else
    dotnet msbuild GBPemuX/GBPemuX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:RuntimeIdentifier=osx-x64 /p:OutputPath=../bin/GBPemu/Release/ /p:SelfContained=true -p:PublishSingleFile=true -t:BundleApp -p:CFBundleShortVersionString=6.0 -p:UseAppHost=true
    if [ $? -ne 0 ] 
    then 
        echo "Aborting release. Error during GBPemuX build."
    else
        dotnet msbuild GBPUpdaterX/GBPUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:RuntimeIdentifier=osx-x64 /p:OutputPath=../bin/GBPUpdater/Release/ /p:SelfContained=true -p:PublishSingleFile=true -t:BundleApp -p:CFBundleShortVersionString=6.0 -p:UseAppHost=true
        if [ $? -ne 0 ] 
        then 
        echo "Aborting release. Error during GBPUpdater build."
        else
        dotnet msbuild UsbUpdaterX/UsbUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:RuntimeIdentifier=osx-x64 /p:OutputPath=../bin/UsbUpdater/Release/ /p:SelfContained=true -p:PublishSingleFile=true -t:BundleApp -p:CFBundleShortVersionString=6.0 -p:UseAppHost=true
        if [ $? -ne 0 ] 
        then 
            echo "Aborting release. Error during GBPUpdater build."
        else
            mv bin/RetroSpy/Release/publish/RetroSpy.app/Contents/MacOS/RetroSpy bin/RetroSpy/Release/publish/RetroSpy.app/Contents/MacOS/RetroSpy-x64
            mv bin/GBPemu/Release/publish/GBPemu.app/Contents/MacOS/GBPemu bin/GBPemu/Release/publish/GBPemu.app/Contents/MacOS/GBPemu-x64
            mv bin/GPBUpdater/Release/publish/GBPUpdater.app/Contents/MacOS/GBPUpdater bin/GPBUpdater/Release/publish/GBPUpdater.app/Contents/MacOS/GBPUpdater-x64
            mv bin/UsbUpdater/Release/publish/UsbUpdater.app/Contents/MacOS/UsbUpdater bin/UsbUpdater/Release/publish/UsbUpdater.app/Contents/MacOS/UsbUpdater-x64
	    fi
        fi
    fi
fi

dotnet msbuild RetroSpyX/RetroSpyX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:RuntimeIdentifier=osx-arm64 /p:OutputPath=../bin/RetroSpy/Release/ /p:SelfContained=true -p:PublishSingleFile=true -t:BundleApp -p:CFBundleShortVersionString=6.0 -p:UseAppHost=true
if [ $? -ne 0 ] 
then 
    echo "Aborting release. Error during RetroSpyX build."
else
    dotnet msbuild GBPemuX/GBPemuX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:RuntimeIdentifier=osx-arm64 /p:OutputPath=../bin/GBPemu/Release/ /p:SelfContained=true -p:PublishSingleFile=true -t:BundleApp -p:CFBundleShortVersionString=6.0 -p:UseAppHost=true
    if [ $? -ne 0 ] 
    then 
        echo "Aborting release. Error during GBPemuX build."
    else
        dotnet msbuild GBPUpdaterX/GBPUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:RuntimeIdentifier=osx-arm64 /p:OutputPath=../bin/GBPUpdater/Release/ /p:SelfContained=true -p:PublishSingleFile=true -t:BundleApp -p:CFBundleShortVersionString=6.0 -p:UseAppHost=true
        if [ $? -ne 0 ] 
        then 
        echo "Aborting release. Error during GBPUpdater build."
        else
        dotnet msbuild UsbUpdaterX/UsbUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:RuntimeIdentifier=osx-arm64 /p:OutputPath=../bin/UsbUpdater/Release/ /p:SelfContained=true -p:PublishSingleFile=true -t:BundleApp -p:CFBundleShortVersionString=6.0 -p:UseAppHost=true
        if [ $? -ne 0 ] 
        then 
            echo "Aborting release. Error during GBPUpdater build."
        else
            mv bin/RetroSpy/Release/publish/RetroSpy.app/Contents/MacOS/RetroSpy bin/RetroSpy/Release/publish/RetroSpy.app/Contents/MacOS/RetroSpy-arm64
            mv bin/GBPemu/Release/publish/GBPemu.app/Contents/MacOS/GBPemu bin/GBPemu/Release/publish/GBPemu.app/Contents/MacOS/GBPemu-arm64
            mv bin/GBPUpdater/Release/publish/GBPUpdater.app/Contents/MacOS/GBPUpdater bin/GBPUpdater/Release/publish/GBPUpdater.app/Contents/MacOS/GBPUpdater-arm64
            mv bin/UsbUpdater/Release/publish/UsbUpdater.app/Contents/MacOS/UsbUpdater bin/UsbUpdater/Release/publish/UsbUpdater.app/Contents/MacOS/UsbUpdater-arm64

            lipo -create -output bin/RetroSpy/Release/publish/RetroSpy.app/Contents/MacOS/RetroSpy bin/RetroSpy/Release/publish/RetroSpy.app/Contents/MacOS/RetroSpy-x64 bin/RetroSpy/Release/publish/RetroSpy.app/Contents/MacOS/RetroSpy-arm64
            lipo -create -output bin/GBPemu/Release/publish/GBPemu.app/Contents/MacOS/GBPemu bin/GBPemu/Release/publish/GBPemu.app/Contents/MacOS/GBPemu-x64 bin/GBPemu/Release/publish/GBPemu.app/Contents/MacOS/GBPemu-arm64
            lipo -create -output bin/GBPUpdater/Release/publish/GBPUpdater.app/Contents/MacOS/GBPUpdater bin/GBPUpdater/Release/publish/GBPUpdater.app/Contents/MacOS/GBPUpdater-x64 bin/GBPUpdater/Release/publish/GBPUpdater.app/Contents/MacOS/GBPUpdater-arm64
            lipo -create -output bin/UsbUpdater/Release/publish/UsbUpdater.app/Contents/MacOS/UsbUpdater bin/UsbUpdater/Release/publish/UsbUpdater.app/Contents/MacOS/UsbUpdater-x64 bin/UsbUpdater/Release/publish/UsbUpdater.app/Contents/MacOS/UsbUpdater-arm64

            rm bin/RetroSpy/Release/publish/RetroSpy.app/Contents/MacOS/*-arm64
            rm bin/RetroSpy/Release/publish/RetroSpy.app/Contents/MacOS/*-x64
            rm bin/GBPemu/Release/publish/GBPemu.app/Contents/MacOS/*-arm64
            rm bin/GBPemu/Release/publish/GBPemu.app/Contents/MacOS/*-x64
            rm bin/GBPUpdater/Release/publish/GBPUpdater.app/Contents/MacOS/*-arm64
            rm bin/GBPUpdater/Release/publish/GBPUpdater.app/Contents/MacOS/*-x64
            rm bin/UsbUpdater/Release/publish/UsbUpdater.app/Contents/MacOS/*-arm64
            rm bin/UsbUpdater/Release/publish/UsbUpdater.app/Contents/MacOS/*-x64

            #cd bin/Release/
            #cp -r publish RetroSpy-macOS
            #codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS/*.dylib
            #codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS/osx.os
            #codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS/RetroSpy
            #codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS/GBPemu
            #codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS/GBPUpdater
            #codesign --force --verbose --timestamp --sign "$apple_teamid" --options=runtime --entitlements ../../entitlements.plist RetroSpy-macOS/UsbUpdater
	        #ditto -c --sequesterRsrc -k RetroSpy-macOS/ ../../RetroSpy-macOS.zip
            #xcrun notarytool submit ../../RetroSpy-macOS.zip --wait --apple-id "$apple_username" --password "$apple_password" --team-id "$apple_teamid" --output-format json
            #if [ -d "/Volumes/src/upload" ]
            #then
            #cp ../../RetroSpy-macOS.zip /Volumes/src/upload  
            #fi
            #cd ../..
	    fi
        fi
    fi
fi
