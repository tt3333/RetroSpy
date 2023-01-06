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
	        zip -r ../../RetroSpy-macOS-x64.zip RetroSpy-macOS-x64
            if [ -d "/Volumes/src/upload" ]
            then
            cp ../../RetroSpy-macOS-x64.zip /Volumes/src/upload  
            fi
            cd ../..
	    fi
        fi
    fi
fi

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
	        zip -r ../../RetroSpy-macOS-arm64.zip RetroSpy-macOS-arm64
            if [ -d "/Volumes/src/upload" ]
            then
            cp ../../RetroSpy-macOS-arm64.zip /Volumes/src/upload  
            fi
            cd ../..
	    fi
        fi
    fi
fi