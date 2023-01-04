#!/bin/bash

rm -rf bin/Release/net7.0
rm -rf bin/Release/RetroSpy-macOS
rm -rf RetroSpy-macOS.zip

git pull

dotnet build RetroSpyX/RetroSpyX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0

if [ $? -ne 0 ] 
then 
    echo "Aborting release. Error during RetroSpyX build."
else
    dotnet build GBPemuX/GBPemuX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0
    if [ $? -ne 0 ] 
    then 
        echo "Aborting release. Error during GBPemuX build."
    else
        dotnet build GBPUpdaterX/GBPUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0
        if [ $? -ne 0 ] 
        then 
        echo "Aborting release. Error during GBPUpdater build."
        else
        dotnet build UsbUpdaterX/UsbUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0
        if [ $? -ne 0 ] 
        then 
            echo "Aborting release. Error during GBPUpdater build."
        else
            cd bin/Release/
            cp -r net7.0 RetroSpy-macOS
	        zip -r ../../RetroSpy-macOS.zip RetroSpy-macOS
            if [ -d "/Volumes/src/upload" ]
            then
            cp ../../RetroSpy-macOS.zip /Volumes/src/upload  
            fi
            cd ../..
	    fi
        fi
    fi
fi

