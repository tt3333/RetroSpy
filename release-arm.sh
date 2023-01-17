#!/bin/bash
 
rm -rf bin/Release/net7.0
rm -rf bin/Release/RetroSpy-Linux
rm -rf RetroSpy-Linux-arm64.tar.gz

dotnet publish RetroSpyX/RetroSpyX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0/ -r linux-arm64 --self-contained

if [ $? -ne 0 ] 
then 
   echo "Aborting release. Error during RetroSpyX build."
else
   dotnet publish GBPemuX/GBPemuX.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0/ -r linux-arm64 --self-contained
   if [ $? -ne 0 ] 
   then 
     echo "Aborting release. Error during GBPemuX build."
   else
     dotnet publish GBPUpdaterX2/GBPUpdaterX2.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0/ -r linux-arm64 --self-contained
     if [ $? -ne 0 ] 
     then 
       echo "Aborting release. Error during GBPUpdater build."
     else
       dotnet publish UsbUpdaterX2/UsbUpdaterX2.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=../bin/Release/net7.0/ -r linux-arm64 --self-contained
       if [ $? -ne 0 ] 
       then 
         echo "Aborting release. Error during GBPUpdater build."
       else
         cd bin/Release
	     mkdir RetroSpy-Linux
         mkdir RetroSpy-Linux/bin
         cp -r net7.0/publish/* RetroSpy-Linux/bin
         mv RetroSpy-Linux/bin/skins RetroSpy-Linux
         mv RetroSpy-Linux/bin/keybindings.xml RetroSpy-Linux
         mv RetroSpy-Linux/bin/game_palettes.cfg RetroSpy-Linux
         mkdir RetroSpy-Linux/MiSTer
         mkdir RetroSpy-Linux/bin
         cp ../../MiSTer/update-retrospy.sh RetroSpy-Linux/MiSTer
	     mv RetroSpy-Linux/bin/RetroSpy RetroSpy-Linux/bin/retrospy
	     mv RetroSpy-Linux/bin/GBPemu RetroSpy-Linux/bin/pixelview
	     mv RetroSpy-Linux/bin/GBPUpdater RetroSpy-Linux/bin/pixelupdate
	     mv RetroSpy-Linux/bin/UsbUpdater RetroSpy-Linux/bin/visionusbupdate
	     tar -zcvf ../../RetroSpy-Linux-arm64.tar.gz RetroSpy-Linux
         if [ -d "/mnt/src/upload" ]
         then
           cp ../../RetroSpy-Linux-arm64.tar.gz /mnt/src/upload  
         fi
         cd ../..
	   fi
     fi
   fi
fi

