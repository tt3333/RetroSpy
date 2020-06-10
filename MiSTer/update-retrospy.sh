#!/bin/bash

echo "Installing RetroSpy for MiSTer"
echo " "

echo "Updating and executing the RetroSpy installer script."
echo " "

wget -q -t 3 --no-check-certificate --output-file=/tmp/wget-log --show-progress -O /tmp/RetroSpy-Mister.zip https://github.com/retrospy/RetroSpy/releases/latest/download/RetroSpy-Mister.zip

unzip -o /tmp/RetroSpy-Mister.zip -d /tmp/retrospy

chmod +x /tmp/retrospy/update-retrospy-installer.sh

/tmp/retrospy/update-retrospy-installer.sh

rm -rf /tmp/retrospy
rm -rf /tmp/RetroSpy-Mister.zip

echo " " 
echo "Installation complete! Please go to http://www.retro-spy.com to download the newest Windows client application."