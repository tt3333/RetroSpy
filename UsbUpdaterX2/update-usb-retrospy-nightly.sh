#!/bin/bash

echo "Installing RetroSpy for Beaglebone"
echo " "

echo "Updating and executing the RetroSpy installer script."
echo " "

response=$(curl -k -sH "Authorization: token ${1}" https://api.github.com/repos/retrospy/RetroSpy-private/releases/tags/nightly)

eval $(echo "$response" | grep -C3 "name.:.\+update-usb-retrospy-installer.sh" | grep -w id | tr : = | tr -cd '[[:alnum:]]=')
[ "$id" ] || { echo "Error: Failed to get asset id, response: $response" | awk 'length($0)<100' >&2; exit 1; }

GH_ASSET="https://${1}:@api.github.com/repos/retrospy/RetroSpy-private/releases/assets/${id}"

rm -rf /tmp/update-usb-retrospy-installer.sh

echo "Downloading installer script..."
wget -q -t 3 --no-check-certificate --output-file=/tmp/wget-log --show-progress --auth-no-challenge --header "Accept:application/octet-stream" ${GH_ASSET} -O /tmp/update-usb-retrospy-installer.sh

dos2unix /tmp/update-usb-retrospy-installer.sh
chmod +x /tmp/update-usb-retrospy-installer.sh

ls -l /tmp

/tmp/update-usb-retrospy-installer.sh $1

rm -rf /tmp/update-usb-retrospy-installer.sh

echo " " 
echo "Installation complete!  Please reboot your device."