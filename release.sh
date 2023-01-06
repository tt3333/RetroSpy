#!/bin/bash

rm -rf bin/Release/net7.0
rm -rf bin/Release/RetroSpy-Linux
rm -rf RetroSpy-Linux.tar.gz

git pull

dotnet build RetroSpyX/RetroSpyX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r linux-x64 --self-contained

if [ $? -ne 0 ] 
then 
   echo "Aborting release. Error during RetroSpyX build."
else
   dotnet build GBPemuX/GBPemuX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r linux-x64 --self-contained
   if [ $? -ne 0 ] 
   then 
     echo "Aborting release. Error during GBPemuX build."
   else
     dotnet build GBPUpdaterX/GBPUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r linux-x64 --self-contained
     if [ $? -ne 0 ] 
     then 
       echo "Aborting release. Error during GBPUpdater build."
     else
       dotnet build UsbUpdaterX/UsbUpdaterX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0 -r linux-x64 --self-contained
       if [ $? -ne 0 ] 
       then 
         echo "Aborting release. Error during GBPUpdater build."
       else
         cd bin/Release/
         cp -r net7.0 RetroSpy-Linux
	     tar -zcvf ../../RetroSpy-Linux.tar.gz RetroSpy-Linux
         if [ -d "/mnt/src/upload" ]
         then
           cp ../../RetroSpy-Linux.tar.gz /mnt/src/upload  
         fi
         cd ../..
	   fi
     fi
   fi
fi

