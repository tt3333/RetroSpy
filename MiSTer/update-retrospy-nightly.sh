#!/bin/bash

echo "Installing Nightly Build of RetroSpy for MiSTer"
echo " "

echo "Updating and executing the RetroSpy installer script."
echo " "

wget -q -t 3 --no-check-certificate --output-file=/tmp/wget-log --show-progress -O /tmp/update-retrospy-installer.sh https://github.com/retrospy/RetroSpy/releases/download/nightly/update-retrospy-installer.sh

chmod +x /tmp/update-retrospy-installer.sh

/tmp/update-retrospy-installer.sh

rm -rf /tmp/update-retrospy-installer.sh

echo " " 
echo "Installation complete! Please go to http://www.retro-spy.com to download the newest Windows client application."