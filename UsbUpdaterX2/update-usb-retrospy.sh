#!/bin/bash
 
echo "Installing RetroSpy for Beaglebone"
echo " "

echo "Updating and executing the RetroSpy installer script."
echo " "

rm -rf /tmp/update-usb-retrospy-installer.sh

echo "Downloading installer script..."
wget -q -t 3 --no-check-certificate --output-file=/tmp/wget-log --show-progress -O /tmp/update-usb-retrospy-installer.sh https://github.com/retrospy/RetroSpy/releases/latest/download/update-usb-retrospy-installer.sh

dos2unix /tmp/update-usb-retrospy-installer.sh
chmod +x /tmp/update-usb-retrospy-installer.sh

/tmp/update-usb-retrospy-installer.sh

rm -rf /tmp/update-usb-retrospy-installer.sh

echo " " 
echo "Installation complete!  Please reboot your device."